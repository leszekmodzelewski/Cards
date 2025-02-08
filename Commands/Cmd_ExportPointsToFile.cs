

using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GeoLib.Entities.RectBlanking
{
    using System;
    using SWF = System.Windows.Forms;
    using ZwSoft.ZwCAD.ApplicationServices;
    using ZwSoft.ZwCAD.DatabaseServices;
    using ZwSoft.ZwCAD.EditorInput;
    using ZwSoft.ZwCAD.Geometry;
    using ZwSoft.ZwCAD.Runtime;
    

    public struct Point3dWithId
    {
        public Point3dWithId(Point3d point, string textId)
        {
            Point = point;
            TextId = textId;
            
        }

        public Point3d Point { get; }
        public string TextId { get; }

        public override string ToString()
        {
            return $"{TextId},{Point.X:N3},{Point.Y:N3},{Point.Z:N3}";
        }
    }

    public static class CardsData
    {
        public static Vector3d? VectorBetweenTextAndPoint { get; set; }
        public static List<Point3dWithId> RecentlyExportedPoints { get; set; }
    }
    public static class CardsData2
    {
       public static List<Point3dWithId> RecentlyExportedPoints2 { get; set; }

    }

    public class Cmd_ExportPointsToFile
    {
        // DKO: Export to file selected points
        [CommandMethod("EXPORTPOINTSTOFILE", CommandFlags.UsePickSet)]
        public void ExportToFile()
        {
            Document mdiActiveDocument = Application.DocumentManager.MdiActiveDocument;

            PromptSelectionResult acSSPrompt = mdiActiveDocument.Editor.SelectImplied();

            SelectionSet acSSet;

            var points = new List<Point3d>();
            var textForPoints = new List<Point3dWithId>();

            // If the prompt status is OK, objects were selected before
            // the command was started
            if (acSSPrompt.Status == PromptStatus.OK)
            {
                acSSet = acSSPrompt.Value;

                using (Transaction transaction = mdiActiveDocument.Database.TransactionManager.StartTransaction())
                {
                    foreach (SelectedObject acSSObj in acSSet)
                    {
                        // Check to make sure a valid SelectedObject object was returned
                        if (acSSObj != null)
                        {
                            // Open the selected object for write
                            Entity acEnt = transaction.GetObject(acSSObj.ObjectId, OpenMode.ForRead) as Entity;

                            if (acEnt != null)
                            {
                                switch (acEnt)
                                {
                                    case DBPoint point:
                                        points.Add(point.Position);
                                        break;

                                    case DBText text:
                                        textForPoints.Add(new Point3dWithId(text.Position, text.TextString));
                                        break;

                                }
                            }
                        }
                    }
                }
            }


            if (points.Any())
            {
                List<Point3dWithId> pointsToSave = null;
                if (textForPoints.Any())
                {
                    pointsToSave = TryMatchPointWithText(textForPoints, points, CardsData.VectorBetweenTextAndPoint.Value);
                }
                pointsToSave = pointsToSave ?? points.Select((m, i) => new Point3dWithId(m, i.ToString())).ToList();
                CardsData.RecentlyExportedPoints = pointsToSave;

                Logic.Points.RealPoints = CardsData.RecentlyExportedPoints?.Select(m => new PointCalc.MyPoint3D(m.Point.X, m.Point.Y, m.Point.Z, m.TextId)).ToArray();

                if (pointsToSave.Any())
                {
                    Application.ShowAlertDialog($"{pointsToSave.Count} points has been saved.");
                }
               // var resultX = SWF.MessageBox.Show("save to file?", "Point save", SWF.MessageBoxButtons.YesNo);

                //if (resultX == SWF.DialogResult.Yes)

               //     SaveToFile(pointsToSave);
                var mc = new Commands.Cmd_Fit(); // Błąd kompilatora CS0120
                mc.Execute();
            }
            else
            {
                Application.ShowAlertDialog($"Pleas select points before pressing the button.");
            }


            // if (points.Any())
            // {
            //     List<Point3dWithId> pointsToSave = null;
            //     if (textForPoints.Any())
            //     {
            //         pointsToSave = FindClosest(textForPoints, points);
            //     }
            //
            //     if (pointsToSave?.Any() ?? false)
            //     {
            //         SaveToFile(pointsToSave);
            //         Application.ShowAlertDialog($"{pointsToSave.Count} points has been saved.");
            //     }
            //
            // }
            // else
            // {
            //     Application.ShowAlertDialog($"Pleas select points before pressing the button.");
            // }



            // Document mdiActiveDocument = ZwSoft.ZwCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            // Editor editor = mdiActiveDocument.Editor;
            // ObjectId blockId = AppUtils.EnsureRectBlanking(mdiActiveDocument.Database);
            // if (!blockId.IsNull)
            // {
            //     PromptPointResult point = editor.GetPoint("Specify first point.");
            //     if (point.Status == PromptStatus.OK)
            //     {
            //         PromptPointResult result2 = null;
            //         using (new TransientRectBlanking(point.Value))
            //         {
            //             result2 = editor.GetPoint("Specify second point.");
            //             if (result2.Status != PromptStatus.OK)
            //             {
            //                 return;
            //             }
            //         }
            //         double x = Math.Min(point.Value.X, result2.Value.X);
            //         double num2 = Math.Max(point.Value.X, result2.Value.X);
            //         double y = Math.Min(point.Value.Y, result2.Value.Y);
            //         Point3d start = new Point3d(x, y, 0.0);
            //         RectBlankingUtils.CreateBlockReference(mdiActiveDocument.Database, blockId, start, num2 - x, Math.Max(point.Value.Y, result2.Value.Y) - y);
            //     }
            // }
        }

        
        private List<Point3dWithId> FindClosest(List<Point3dWithId> textForPoints, List<Point3d> points)
        {
            if (points.Count > textForPoints.Count)
            {
                Application.ShowAlertDialog($"Unable to match text with points because points number is greater than texts - Points:{points.Count}:Texts:{textForPoints.Count}");
                return null;
            }

            var textForPointsForCalc = new List<Point3dWithId>();
            textForPointsForCalc.AddRange(textForPoints);

            var usedText = new Dictionary<string, int>();

            List<Point3dWithId> pointsWithText = new List<Point3dWithId>();
            foreach (var point in points)
            {
                int textPointIndex = MinIndex(point, textForPointsForCalc);
                pointsWithText.Add(new Point3dWithId(point, textForPointsForCalc[textPointIndex].TextId));
                //textForPointsForCalc.RemoveAt(textPointIndex);

                if (usedText.ContainsKey(textForPointsForCalc[textPointIndex].TextId))
                {
                    usedText[textForPointsForCalc[textPointIndex].TextId] += 1;
                }
                else
                {
                    usedText[textForPointsForCalc[textPointIndex].TextId] = 1;
                }
            }


            var moreThanOne = usedText.Where(m => m.Value > 1).ToList();
            var unusedText = textForPointsForCalc.Select(m => m.TextId).Except(usedText.Keys).ToList();

            if (moreThanOne.Any() || unusedText.Any())
            {
                string msg = "Exported but....";
                if (moreThanOne.Any())
                {
                    msg += Environment.NewLine + "Some texts has been used more than one:" + Environment.NewLine;
                    msg += string.Join(" | ", moreThanOne.Select(w => $"text:'{w.Key}' count:{w.Value}"));
                }
                if (unusedText.Any())
                {
                    msg += Environment.NewLine + "Also, there are texts that were not assigned to any points:" + Environment.NewLine;
                    msg += string.Join(" | ", unusedText.Select(w => $"text:'{w}'"));
                }

                Application.ShowAlertDialog(msg);
            }
            return pointsWithText;
        }

        private int MinIndex(Point3d point, List<Point3dWithId> textForPoints)
        {
            int index = -1;
            double minDistance = double.MaxValue;
            for (int i = 0; i < textForPoints.Count; i++)
            {
                var distance = point.DistanceTo(textForPoints[i].Point);
                if (distance < minDistance)
                {
                    index = i;
                    minDistance = distance;
                }
            }
            return index;
        }

        private List<Point3dWithId> TryMatchPointWithText(List<Point3dWithId> textForPoints, List<Point3d> points, Vector3d vector)
        {
            double tolerance = 0.001;

            if (points.Count > textForPoints.Count)
            {
                Application.ShowAlertDialog($"Unable to match text with points because points number is greater than texts - Points:{points.Count}:Texts:{textForPoints.Count}");
                return null;
            }

            List<Point3dWithId> pointsWithText = new List<Point3dWithId>();

            var textForPointsForCalc = new List<Point3dWithId>();
            textForPointsForCalc.AddRange(textForPoints);

            foreach (var point in points)
            {
                var textPointIndex = textForPointsForCalc.FindIndex(m => (point.GetVectorTo(m.Point) - vector).Length < tolerance);
                if (textPointIndex >= 0)
                {
                    pointsWithText.Add(new Point3dWithId(point, textForPointsForCalc[textPointIndex].TextId));
                    textForPointsForCalc.RemoveAt(textPointIndex);
                }
                else
                {
                    Application.ShowAlertDialog($"Unable to match text with point - point: X:{point.X}; Y:{point.Y}; Z:{point.Z}");
                    pointsWithText.Clear();
                    break;
                }
            }

            if (pointsWithText.Any())
            {
                var r = pointsWithText.GroupBy(x => x.TextId).Where(g => g.Count() > 1).Select(y => y.Key).ToList();
                if (r.Count <= 0)
                {
                    return pointsWithText;
                }
                Application.ShowAlertDialog($"Unable to match text with points because matched texts has the same Id's, i.e. {r[0]}");
            }
            return null;
        }


        public void SaveToFile(List<Point3dWithId> data)
        {
            var path = @"c:\Users\lmodzelewski\Documents\points.txt";

            using (StreamWriter outputFile = new StreamWriter(path))
            {
                foreach (var d in data)
                {
                    outputFile.WriteLine($"{d.TextId};{string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0:0.0}", d.Point.X)};{string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0:0.0}", d.Point.Y)};{string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0:0.0}", d.Point.Z)}");
                }

            }

        }
    }
}

