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

    public abstract class Cmd_FitCylinder
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
   
 
    public class Cmd_BestFitTube : Cmd_ArrowReadPoints
    {

        [CommandMethod("FITTUBE", CommandFlags.UsePickSet)]
        public void ExportToFile()
        {
            Data.AddTheoryPoints(ReadPoints(countPoints: true));
            Data.BestFitTube();

        }
    }



    static class Data
    {

        private const string A_PX = "+X";
        private const string A_NX = "-X";
        private const string A_PY = "+Y";
        private const string A_NY = "-Y";
        private const string A_PZ = "+Z";
        private const string A_NZ = "-Z";
        private const string A_PZ0 = "Z0";
        
        static List<MyPoint3D> theoryPoints = new List<MyPoint3D>();
               
        public static void AddTheoryPoints(List<MyPoint3D> points)
        {
            // TODO: Avoid duplication
            theoryPoints.AddRange(points);
        }

        public static void BestFitTube()
        {
            int L = theoryPoints.Count();

            double Dx = 0;
            double Dy = 0;
            double Dz = 0;

            for (int i = 0; i < L; i++)
            {
                Dx += theoryPoints.ElementAt(i).X;
                Dy += theoryPoints.ElementAt(i).Y;
                Dz += theoryPoints.ElementAt(i).Z;
            }
           
            List<MyPoint3D> pointc = new List<MyPoint3D>();
            pointc.Add(new MyPoint3D(Dx/L, Dy/L , Dz / L, "1"));

            CadDrawPoints.Draw(pointc.ToArray(), "Srednia", Color.FromColor(System.Drawing.Color.DarkMagenta), deleteLayerIfExist: false);
            
            
            theoryPoints.Clear();
            
        }
    }
}


