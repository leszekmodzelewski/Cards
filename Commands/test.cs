using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using GeoLib.Logic;
using PointCalc;
using ZwSoft.ZwCAD.ApplicationServices;
using ZwSoft.ZwCAD.DatabaseServices;
using ZwSoft.ZwCAD.EditorInput;
using ZwSoft.ZwCAD.Geometry;
using ZwSoft.ZwCAD.Runtime;
using System.Windows.Forms;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using ZwSoft.ZwCAD.Colors;
using GeoLib.Entities.TableRef;
using System.Windows.Media.Media3D;
using GeoLib.Entities.Table;
using Application = ZwSoft.ZwCAD.ApplicationServices.Application;



// to do:
//wyłączyć clear z średnic
// jak rozibć okienko z długością 
// zrobić automat do uzupełniania danych ( kątowych i długości)

namespace GeoLib.Entities.RectBlanking
{

    public abstract class Cmd_FitCylinder2
    {

        protected List<MyPoint3D> ReadPoints(bool countPoints = true)
        {

            Document mdiActiveDocument = ZwSoft.ZwCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;

            PromptSelectionResult acSSPrompt = mdiActiveDocument.Editor.SelectImplied();

            SelectionSet acSSet;

            var points = new List<MyPoint3D>();

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
                                        var pos = point.Position;
                                        points.Add(new MyPoint3D(pos.X, pos.Y, pos.Z, point.Handle.Value.ToString(CultureInfo.InvariantCulture)));
                                        break;

                                }
                            }
                        }
                    }
                }
            }

            if (countPoints == true)
                if (points.Any())
                {
                    ZwSoft.ZwCAD.ApplicationServices.Application.ShowAlertDialog($"{points.Count} points has been read.");

                }
                else
                {

                    ZwSoft.ZwCAD.ApplicationServices.Application.ShowAlertDialog($"Pleas select points before pressing the button.");
                }

            return points;
        }

    }


    public class Cmd_BestFitTube2 : Cmd_ArrowReadPoints
    {

        [CommandMethod("FT", CommandFlags.UsePickSet)]
        public void ExportToFile2()
        {
            Data2.AddTheoryPoints(ReadPoints(countPoints: true));
           
        }
    }



    static class Data2
    {

        static List<MyPoint3D> theoryPoints = new List<MyPoint3D>();

        public static void AddTheoryPoints(List<MyPoint3D> points)
        {
            // TODO: Avoid duplication
            theoryPoints.AddRange(points);
        }
    }

}