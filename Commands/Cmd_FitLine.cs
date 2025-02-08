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

// jak rozibć okienko z długością 

// zrobić automat do uzupełniania danych ( kątowych i długości)

namespace GeoLib.Entities.RectBlanking
{

    public class Cmd_FitLine : Cmd_ArrowReadPoints
    {

        [CommandMethod("FITLINE", CommandFlags.UsePickSet)]
        public void ExportToFile()
        {
            
            ArrowData2.AddTheoryPoints(ReadPoints(countPoints: false));
            ArrowData2.FitLine();

        }

      
    }
    static class ArrowData2
    {
        
        private const string A_PZ0 = "Z0";
        static List<MyPoint3D> theoryPoints = new List<MyPoint3D>();
        

        public static void AddTheoryPoints(List<MyPoint3D> points)
        {
            // TODO: Avoid duplication
            theoryPoints.AddRange(points); 
        }

        public static void FitLine()
        {
            double Sxx = 0;
            double Syy = 0;
            double Sxy = 0;
            double Sx = 0;
            double Sy = 0;
            double a = 0;
            double b = 0;
            double Delta = 0;
            
           

            var Pt1 = new Point3d();
            var Pt2 = new Point3d();
            Matrix<double> u = Matrix<double>.Build.Dense(theoryPoints.Count(), 1);
            Matrix<double> V4 = Matrix<double>.Build.Dense(theoryPoints.Count(), 2);
            Matrix<double> rzedna = Matrix<double>.Build.Dense(theoryPoints.Count(), 1);
            MyPoint3D[] inverted = new MyPoint3D[theoryPoints.Count()];
            int S = theoryPoints.Count();
            Matrix<double> CP = Matrix<double>.Build.Dense(2, 1);
            for (int i = 0; i < theoryPoints.Count(); i++)
            {
                CP[0, 0] += theoryPoints.ElementAt(i).X;
                CP[1, 0] += theoryPoints.ElementAt(i).Y;

            }
           
            double dX = CP[0, 0] / theoryPoints.Count();
            double dY = CP[1, 0] / theoryPoints.Count();

            Matrix<double> CP2 = Matrix<double>.Build.Dense(theoryPoints.Count(), 2);
            for (int i = 0; i < theoryPoints.Count(); i++)
            {
                CP2[i, 0] = Math.Abs(theoryPoints.ElementAt(i).X - dX);
                CP2[i, 1] = Math.Abs(theoryPoints.ElementAt(i).Y - dY);
            }

            Matrix<double> CPsum = Matrix<double>.Build.Dense(2, 1);
            for (int i = 0; i < theoryPoints.Count(); i++)
            {
                CPsum[0, 0] += CP2[i, 0];
                CPsum[1, 0] += CP2[i, 1];
            }

            if (CPsum[1, 0] > CPsum[0, 0])
            { 
                for (int i = 0; i < theoryPoints.Count(); ++i)
                {
                    inverted[i] = new MyPoint3D(theoryPoints[i].Y, theoryPoints[i].X, theoryPoints[i].Z, theoryPoints[i].Id);
                }
            }

            else
            {
                for (int i = 0; i < theoryPoints.Count(); ++i)
                {
                    inverted[i] = new MyPoint3D(theoryPoints[i].X, theoryPoints[i].Y, theoryPoints[i].Z, theoryPoints[i].Id);
                }

            }
            for (int i = 0; i < theoryPoints.Count(); i++)
            {
                Sx += inverted.ElementAt(i).X;
                Sy += inverted.ElementAt(i).Y;
                Sxx += inverted.ElementAt(i).X * inverted.ElementAt(i).X;
                Syy += inverted.ElementAt(i).Y * inverted.ElementAt(i).Y;
                Sxy += inverted.ElementAt(i).X * inverted.ElementAt(i).Y;
            }

            Delta = S * Sxx - (Sx * Sx);
            a = (S * Sxy - Sx * Sy) / Delta;
            b = (Sxx*Sy - Sx*Sxy) / Delta;

            //Application.ShowAlertDialog(theoryPoints.Count().ToString());
            //Application.ShowAlertDialog(a.ToString()+"  "+ b.ToString());

            int licznik = theoryPoints.Count();


            if (licznik > 2)
            {
                
                               
                Document doc = Application.DocumentManager.MdiActiveDocument;
                Database db = doc.Database;
                Editor edit = doc.Editor;

                using (Transaction tr = db.TransactionManager.StartTransaction())
                {
                    using (var br = new BlockTableRecord())
                    {

                        try
                        {

                            BlockTable bt;
                            bt = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;

                            BlockTableRecord btr;

                            btr = tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                            var space = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);

                           
                            var Pt3 = new Line();

                            if (CPsum[1, 0] > CPsum[0, 0])
                            {
                                Pt1 = new Point3d( a * dY*10 + b, dY*10, 0);

                                Pt2 = new Point3d( a * -dY*10 + b, -dY*10, 0);

                                Pt3 = new Line(Pt1, Pt2);
                            }
                            else
                            {
                                Pt1 = new Point3d(dX*10, a* dX*10 + b, 0);

                                Pt2 = new Point3d(-dX*10, a * -dX*10 + b, 0);

                                Pt3 = new Line(Pt1, Pt2);
                            }

                            Document mdiActiveDocument = Application.DocumentManager.MdiActiveDocument;
                            Database database = mdiActiveDocument.Database;
                            var acBlkTbl = tr.GetObject(database.BlockTableId, OpenMode.ForRead) as BlockTable;
                            var acBlkTblRec = tr.GetObject(acBlkTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                            acBlkTblRec.AppendEntity(Pt3); // zeby ominąć błąd 

                            tr.AddNewlyCreatedDBObject(Pt3, true);

                            //acBlkTblRec.AppendEntity(Pt2);

                            //tr.AddNewlyCreatedDBObject(Pt2, true);

                            for (int i = 0; i < inverted.Count(); i++)
                            {

                                u[i,0] = ((theoryPoints.ElementAt(i).X- Pt1.X)*(Pt2.X-Pt1.X)+(theoryPoints.ElementAt(i).Y-Pt1.Y)*(Pt2.Y-Pt1.Y))/((Pt1.X-Pt2.X)* (Pt1.X - Pt2.X)+ (Pt1.Y - Pt2.Y)* (Pt1.Y - Pt2.Y));
                                
                            }

                            for (int i = 0; i < inverted.Count(); i++)
                            {
                                V4[i, 0] = Pt1.X + (Pt2.X - Pt1.X) * u[i, 0];
                                V4[i, 1] = Pt1.Y + (Pt2.Y - Pt1.Y) * u[i, 0];
                            }

                            for (int i = 0; i < inverted.Count(); i++)
                            {
                                rzedna[i, 0] = Math.Sqrt((theoryPoints.ElementAt(i).X - V4[i, 0]) * (theoryPoints.ElementAt(i).X - V4[i, 0]) + (theoryPoints.ElementAt(i).Y - V4[i, 1]) * (theoryPoints.ElementAt(i).Y - V4[i, 1]));
                               // Application.ShowAlertDialog(rzedna[i,0].ToString());
                            }

                           
                            tr.Commit();
                        }
                        catch (System.Exception ex)
                        {
                            edit.WriteMessage("Error: " + ex.Message);
                            tr.Abort();
                        }
                    }
                                       
                }

                theoryPoints.Clear();
               
            }


            else
            {
                ZwSoft.ZwCAD.ApplicationServices.Application.ShowAlertDialog("Select at least 3 points before use this button");
            }

        }
    }
}