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
    
    public abstract class Cmd_ArrowReadPoints
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
    public class Cmd_ArrowReadReal : Cmd_ArrowReadPoints
    {
       
        [CommandMethod("ARROWREADREAL", CommandFlags.UsePickSet)]
        public void ExportToFile()
        {
            ArrowData.AddRealPoints(ReadPoints());
        }

    }
    public class Cmd_FitCircle : Cmd_ArrowReadPoints
    {

        [CommandMethod("FITCIRCLE2", CommandFlags.UsePickSet)]
        public void ExportToFile()
        {
            ArrowData.AddTheoryPoints(ReadPoints(countPoints: false));
            ArrowData.Circle();
            
        }
    }
    public class Cmd_FitTube : Cmd_ArrowReadPoints
    {

        [CommandMethod("FITPLANE", CommandFlags.UsePickSet)]
        public void ExportToFile()
        {
            ArrowData.AddTheoryPoints(ReadPoints(countPoints: false));
            ArrowData.BestFitPlane();

        }
    }

    public class Cmd_Rotation : Cmd_ArrowReadPoints
    {

        [CommandMethod("ROTATION", CommandFlags.UsePickSet)]
        public void ExportToFile()
        {
            ArrowData.AddTheoryPoints(ReadPoints(countPoints: false));
            ArrowData.Rotation();

        }
    }



    public class Cmd_ArrowReadTheory : Cmd_ArrowReadPoints
    {

        [CommandMethod("ARROWREADTHEORY", CommandFlags.UsePickSet)]
        public void ExportToFile()
        {
            ArrowData.AddTheoryPoints(ReadPoints());
            
        }

    }

    public class Cmd_DrawArrows
    {

        [CommandMethod("ARROWDRAW", CommandFlags.UsePickSet)]
        public void ExportToFile()
        {
            ArrowData.DrawArrow();
            
        }

    }
    public class Cmd_EXPORTPOINTSTOFILE2 : Cmd_ArrowReadPoints
    {

        [CommandMethod("/*/*EXPORTPOINTSTOFILE*/*/2", CommandFlags.UsePickSet)]
        public void ExportToFile()
        {
            ArrowData.AddTheoryPoints(ReadPoints());
            
            ArrowData.AddTheoryToBase();
           
        }
    }

    public class Cmd_Leveling : Cmd_ArrowReadPoints
    {

        [CommandMethod("LEVELING1", CommandFlags.UsePickSet)]
        public void ExportToFile()
        {
            ArrowData.AddTheoryPoints(ReadPoints());
            ArrowData.DrawArrow2();

        }

    }

    public class Cmd_nowy : Cmd_ArrowReadPoints
    {

        [CommandMethod("OWY", CommandFlags.UsePickSet)]
        public void ExportToFile()
        {
            ArrowData.AddTheoryPoints(ReadPoints());
            ArrowData.owy();

        }

    }

    static class ArrowData
    {
        
        private const string A_PX = "+X";
        private const string A_NX = "-X";
        private const string A_PY = "+Y";
        private const string A_NY = "-Y";
        private const string A_PZ = "+Z";
        private const string A_NZ = "-Z";
        private const string A_PZ0 = "Z0";
        private const string A_PZ02D = "Z0";
        private const string A_PX2D = "+X2D";
        private const string A_NX2D = "-X2D";
        private const string A_PY2D = "+Y2D";
        private const string A_NY2D = "-Y2D";
        private const string A_PZ2D = "+Z2D";
        private const string A_NZ2D = "-Z2D";
        private const string A_PZNIV = "+Zniw";
        private const string A_NZNIV = "-Zniw";

        private const string B_Radius = "Radius"; //dodać
        static bool A_3DArrow = false;
        static double Zteor = 0;

        static List<MyPoint3D> realPoints = new List<MyPoint3D>();
        static List<MyPoint3D> theoryPoints = new List<MyPoint3D>();
        static Matrix<double> CCircle = Matrix<double>.Build.Dense(3, 1);
        static List<MyPoint3D> fittedPlane = new List<MyPoint3D>();

        public static void AddRealPoints(List<MyPoint3D> points)
        {
            // TODO: Avoid duplication
            realPoints.AddRange(points);
        }
        public static void AddTheoryPoints(List<MyPoint3D> points)
        {
            // TODO: Avoid duplication
            theoryPoints.AddRange(points); //co to robi?
            
        }

        public static void AddTheoryToBase()
        {
            if (theoryPoints != null)

            Points.TheoryPoints = theoryPoints.ToArray();
             
        }

        public static void owy()
        {

           
        
        }
        

            public static void DrawArrow()
        {
            string box_msg = "Add X DATA?";
            string box_msg2 = "Add Y DATA?";
            string box_msg3 = "Add Z DATA?";
            string box_title = "Arrows";

            double defaultTableSize = 100;
            MessageBox.Show("Enter scale", "Scale");
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            PromptStringOptions Zdat = new PromptStringOptions("\nEnter scale");
            Zdat.AllowSpaces = true;
            PromptResult Zdat2 = acDoc.Editor.GetString(Zdat);
            try
            {
                defaultTableSize = Convert.ToDouble(Zdat2.StringResult) * 100;
            }
            catch

            {
                MessageBox.Show("Wrong scale: default scale = 1", "Scale");

            }

            // addblock br.ScaleFactors = new Scale3d(defaultTableSize / 150.0);

            PointCalculator pc = new PointCalculator(); //tworzy odowłanie do point calklatora jako programu liczącego w dany sposób
            var matched = pc.Calculate(theoryPoints.ToArray(), realPoints.ToArray(), Double.MaxValue); //twrzy zbior mached opraty na formule pc (dane jako lista point3D)

            //Application.ShowAlertDialog($"{PointListToString(matched)}");

            var db = ZwSoft.ZwCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Database; //1

            var result2D = System.Windows.Forms.MessageBox.Show("Draw Arrow 3D(YES) if (NO) then Draw Arrow 2D", "Draw Aroow 3D", MessageBoxButtons.YesNo);

            var bt = AppUtils.EnsureBlocks(db, new[] { A_PX2D, A_NX2D, A_PY2D, A_NY2D, A_PZ2D, A_NZ2D, A_PZ02D });

            if (result2D == DialogResult.Yes)
            {
                bt = AppUtils.EnsureBlocks(db, new[] { A_PX, A_NX, A_PY, A_NY, A_PZ, A_NZ, A_PZ0 });
                A_3DArrow = true;
            }

            else
            {
                bt = AppUtils.EnsureBlocks(db, new[] { A_PX2D, A_NX2D, A_PY2D, A_NY2D, A_PZ2D, A_NZ2D, A_PZ02D });
                A_3DArrow = false;
            }

           

            using (var tr = db.TransactionManager.StartTransaction()) //auocad start transmisji
            {
                var resultX = System.Windows.Forms.MessageBox.Show(box_msg, box_title, MessageBoxButtons.YesNo);
                var resultY = System.Windows.Forms.MessageBox.Show(box_msg2, box_title, MessageBoxButtons.YesNo);
                var resultZ = System.Windows.Forms.MessageBox.Show(box_msg3, box_title, MessageBoxButtons.YesNo); //boxy to wiem:)

                foreach (var matchedPoint in matched) //dla każdego dopasowanego punktu w mached robić :
                // if(Application.ShowModalDialog(Messages.)

                {
                    var blockInsertionPoint = new Point3d(matchedPoint.TheoryPoint.X, matchedPoint.TheoryPoint.Y, matchedPoint.TheoryPoint.Z); //wstaw blok na wsp zmaczowanych


                    var blockNameX = FindBlockNameX(matchedPoint);
                    var blockNameY = FindBlockNameY(matchedPoint);
                    var blockNameZ = FindBlockNameZ(matchedPoint); // pobierz blok



                    if (resultX == DialogResult.Yes)
                    {

                        AddBlock(blockNameX); //wstaw blok
                    }

                    if (resultY == DialogResult.Yes)
                        {
                            AddBlock(blockNameY);

                        }
                    
                    if (resultZ == DialogResult.Yes)
                    {
                        AddBlock(blockNameZ);
                    }



                    //using (var br = new BlockReference(blockInsertionPoint, bt[blockName]))
                    //{
                    //    var space = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                    //    space.AppendEntity(br);
                    //    tr.AddNewlyCreatedDBObject(br, true);
                    //}

                    void AddBlock((string BlockName, double val) data)
                    {
                        var blockId = bt[data.BlockName];
                        using (var br = new BlockReference(blockInsertionPoint, blockId))
                        {
                            br.ScaleFactors = new Scale3d(defaultTableSize / 150.0);
                            var space = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            space.AppendEntity(br);
                            tr.AddNewlyCreatedDBObject(br, true);

                            foreach (ObjectId id3 in tr.GetObject(blockId, OpenMode.ForWrite) as BlockTableRecord)
                            {
                                AttributeDefinition definition = tr.GetObject(id3, OpenMode.ForRead) as AttributeDefinition;
                                if (definition != null)
                                {
                                    using (AttributeReference reference2 = new AttributeReference())
                                    {
                                        reference2.SetAttributeFromBlock(definition, br.BlockTransform);
                                        reference2.TextString = string.Format(CultureInfo.InvariantCulture, "{0:0}", Math.Abs(data.val)); //definition.TextString;
                                        br.AttributeCollection.AppendAttribute(reference2);
                                        tr.AddNewlyCreatedDBObject(reference2, true);
                                    }
                                }
                            }






                            //foreach (ObjectId id in br.AttributeCollection)
                            //{
                            //    AttributeReference attRef = (AttributeReference)tr.GetObject(id, OpenMode.ForWrite);
                            //    if (attRef.Tag == "VAL")
                            //    {
                            //        attRef.TextString = string.Format(CultureInfo.InvariantCulture, "{0:0}", Math.Abs(data.val)); 
                            //    }
                            //    //EntityBaseUtils.UpdateRangeAttribute(attRef, Points.GetRangeForX(theoryPoint.Xo));
                            //}
                        }
                    }
                }

                tr.Commit();
            }

            Clean();
        }

        
        public static void DrawArrow2() //rysuje wartości Z nawiązane do zadanej wysokości
        {
            MessageBox.Show("Enter leveling vlaue", "Leveling Vlaue");

            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            PromptStringOptions Zdata = new PromptStringOptions("\nEnter leveling vlaue");
            Zdata.AllowSpaces = true;
            PromptResult Zdata2 = acDoc.Editor.GetString(Zdata);

            MessageBox.Show("Enter Scale vlaue", "Scale Vlaue");
            double defaultTableSize = 100;
            PromptStringOptions Zdat = new PromptStringOptions("\nEnter scale");
            Zdat.AllowSpaces = true;
            PromptResult Zdat2 = acDoc.Editor.GetString(Zdat);
            try
            {
                defaultTableSize = Convert.ToDouble(Zdat2.StringResult) * 100;
            }
            catch

            {
                MessageBox.Show("Wrong scale: default scale = 1", "Scale");

            }

            Zteor = Convert.ToDouble(Zdata2.StringResult);

            
            PointCalculator pc = new PointCalculator(); //tworzy odowłanie do point calklatora jako programu liczącego w dany sposób
                var matched = pc.Calculate(theoryPoints.ToArray(), theoryPoints.ToArray(), Double.MaxValue); //twrzy zbior mached opraty na formule pc (dane jako lista point3D)

                //Application.ShowAlertDialog($"{PointListToString(matched)}");

                var db = ZwSoft.ZwCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Database; //1
                var bt = AppUtils.EnsureBlocks(db, new[] { A_PZNIV, A_NZNIV, A_PZ0 });//2 - 1i2 pobierają bloki do uzupełnienia

                using (var tr = db.TransactionManager.StartTransaction()) //auocad start transmisji
                {
                    
                    foreach (var matchedPoint2 in matched) //dla każdego dopasowanego punktu w mached robić :
                                                          // if(Application.ShowModalDialog(Messages.)

                    {
                        var blockInsertionPoint = new Point3d(matchedPoint2.TheoryPoint.X, matchedPoint2.TheoryPoint.Y, matchedPoint2.TheoryPoint.Z); //wstaw blok na wsp zmachowanych


                        var blockNameZ = FindBlockNameZ2(matchedPoint2); // pobierz blok
                                        
                    {
                            AddBlock(blockNameZ);
                        }


                        void AddBlock((string BlockName, double val) data) //dodawanie bloków
                        {
                        
                         var blockId = bt[data.BlockName];
                            using (var br = new BlockReference(blockInsertionPoint, blockId))
                            {
                                br.ScaleFactors = new Scale3d(defaultTableSize / 150.0);
                                var space = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                                space.AppendEntity(br);
                                tr.AddNewlyCreatedDBObject(br, true);
                            
                                foreach (ObjectId id3 in tr.GetObject(blockId, OpenMode.ForWrite) as BlockTableRecord)
                                {
                                    AttributeDefinition definition = tr.GetObject(id3, OpenMode.ForRead) as AttributeDefinition;
                                    if (definition != null)
                                    {
                                        using (AttributeReference reference2 = new AttributeReference())
                                        {
                                            reference2.SetAttributeFromBlock(definition, br.BlockTransform);

                                         reference2.TextString = string.Format(CultureInfo.InvariantCulture, "{0:0}", Math.Abs(data.val)); //definition.TextString;
                                        //reference2.TextString = string.Format(CultureInfo.InvariantCulture, "{0:#.0}", Math.Abs(data.val)); //definition.TextString;

                                        br.AttributeCollection.AppendAttribute(reference2);
                                            tr.AddNewlyCreatedDBObject(reference2, true);
                                        }
                                    }
                                }

                            }
                        }
                    }

                    tr.Commit();
                }

                Clean();
            }

       
        public static void Circle()
        {
            double defaultTableSize = 100;
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            MessageBox.Show("Enter scale", "Scale");
            PromptStringOptions Zdat = new PromptStringOptions("\nEnter scale");
            Zdat.AllowSpaces = true;
            PromptResult Zdat2 = acDoc.Editor.GetString(Zdat);

            try
            {
                defaultTableSize = Convert.ToDouble(Zdat2.StringResult) * 100;
            }
            catch

            {
                MessageBox.Show("Wrong scale: default scale = 1", "Scale"); 
                
            }

            // addblock br.ScaleFactors = new Scale3d(defaultTableSize / 150.0);

            int licznik = theoryPoints.Count();
            if (licznik > 2)
            {
                
                Matrix<double> MA = Matrix<double>.Build.Dense(licznik, 3);
                for (int i = 0; i < theoryPoints.Count(); i++)
                {
                    MA[i, 0] = theoryPoints.ElementAt(i).X;
                    MA[i, 1] = theoryPoints.ElementAt(i).Y;
                    MA[i, 2] = 1;
                }
               
                Matrix<double> MB = Matrix<double>.Build.Dense(licznik, 2);
                for (int i = 0; i < theoryPoints.Count(); i++)
                {

                    MB[i, 0] = theoryPoints.ElementAt(i).X * theoryPoints.ElementAt(i).X;
                    MB[i, 1] = theoryPoints.ElementAt(i).Y * theoryPoints.ElementAt(i).Y;

                }
                
                var B = MA.Solve(MB);
                double Xs = B.At(0, 0) + B.At(0, 1), //macierz.at(wiersz, kolumna)
                     Ys = B.At(1, 0) + B.At(1, 1),
                     Xr = B.At(2, 0) + B.At(2, 1),
                     r = Math.Round(Math.Sqrt(4 * Xr + (Xs * Xs) + (Ys * Ys)) / 2, MidpointRounding.AwayFromZero);
                
                List<MyPoint3D> pointc = new List<MyPoint3D>();

                Matrix<double> MZ = Matrix<double>.Build.Dense(licznik, 3);
                for (int i = 0; i < theoryPoints.Count(); i++)
                {
                    MZ[i, 0] = theoryPoints.ElementAt(i).X;
                    MZ[i, 1] = theoryPoints.ElementAt(i).Y;
                    MZ[i, 2] = theoryPoints.ElementAt(i).Z;
                }
                double MZH = 0;
                
                for (int i = 0; i < theoryPoints.Count(); i++)
                {
                    MZH += MZ[i, 2];
                }
                  
                    CCircle[0, 0] = Xs / 2;
                    CCircle[1, 0] = Ys / 2;
                    CCircle[2, 0] = r;
                

                pointc.Add(new MyPoint3D(Xs / 2, Ys / 2, MZH/theoryPoints.Count(), "1"));
                string MZH2 = r.ToString();

                var resultX = System.Windows.Forms.MessageBox.Show("Calculated Radius =" + MZH2 + ". Use this to calculate (YES) or set new (NO)?", "Radius", MessageBoxButtons.YesNo);
                
                if (resultX == DialogResult.No)
                {
                    Document acDoc2 = Application.DocumentManager.MdiActiveDocument;
                    PromptStringOptions Zdata = new PromptStringOptions("\nEnter New Circle");
                    Zdata.AllowSpaces = true;
                    PromptResult Zdata2 = acDoc2.Editor.GetString(Zdata);

                    CCircle[2,0] = Convert.ToDouble(Zdata2.StringResult);
                    
                }
                CadDrawPoints.Draw(pointc.ToArray(), "circle", Color.FromColor(System.Drawing.Color.DarkMagenta), deleteLayerIfExist: false);

                var db = ZwSoft.ZwCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Database;
                var bt = AppUtils.EnsureBlocks(db, new[] { B_Radius });
                using (var tr2 = db.TransactionManager.StartTransaction())
                {
                    
                    for (int i = 0; i < licznik; i++)
                    {
                        var blockInsertionPoint = new Point3d(theoryPoints.ElementAt(i).X, theoryPoints.ElementAt(i).Y, theoryPoints.ElementAt(i).Z);
                        //var blockNameX = FindBlockNameX(matchedPoint);
                    }
                     

                    tr2.Commit();
                }

                
                Database acCurDb = acDoc.Database;

                using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
                {
                    BlockTable acBlkTbl;
                    acBlkTbl = acTrans.GetObject(acCurDb.BlockTableId, OpenMode.ForRead) as BlockTable;

                    // Open the Block table record Model space for write
                    BlockTableRecord acBlkTblRec;
                    acBlkTblRec = acTrans.GetObject(acBlkTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                    Circle acCirc = new Circle();
                    acCirc.SetDatabaseDefaults();
                    acCirc.Center = new Point3d(Xs / 2, Ys / 2, MZH / theoryPoints.Count());
                    acCirc.Radius = r;

                    acBlkTblRec.AppendEntity(acCirc);
                    acTrans.AddNewlyCreatedDBObject(acCirc, true);

                    // Save the new object to the database
                    acTrans.Commit();
                }




                PointCalculator pc = new PointCalculator(); //tworzy odowłanie do point calklatora jako programu liczącego w dany sposób
                var matched2 = pc.Calculate(theoryPoints.ToArray(), theoryPoints.ToArray(), Double.MaxValue); //twrzy zbior mached opraty na formule pc (dane jako lista point3D)

                //Application.ShowAlertDialog($"{PointListToString(matched)}");

                var db2 = ZwSoft.ZwCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Database; //1
                var bt2 = AppUtils.EnsureBlocks(db, new[] { A_PZ0 });//2 - 1i2 pobierają bloki do uzupełnienia

                using (var tr2 = db2.TransactionManager.StartTransaction()) //auocad start transmisji
                {

                    foreach (var matchedPoint2 in matched2) //dla każdego dopasowanego punktu w mached robić :
                                                           // if(Application.ShowModalDialog(Messages.)

                    {
                        var blockInsertionPoint = new Point3d(matchedPoint2.TheoryPoint.X, matchedPoint2.TheoryPoint.Y, matchedPoint2.TheoryPoint.Z); //wstaw blok na wsp zmachowanych


                        var blockNameZ = FindBlockNameR(matchedPoint2); // pobierz blok

                        {
                            AddBlock(blockNameZ);
                        }


                        void AddBlock((string BlockName, double val) data) //dodawanie bloków
                        {

                            var blockId = bt2[data.BlockName];
                            using (var br2 = new BlockReference(blockInsertionPoint, blockId))
                            {
                                br2.ScaleFactors = new Scale3d(defaultTableSize / 150.0);
                                var space = (BlockTableRecord)tr2.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                                space.AppendEntity(br2);
                                tr2.AddNewlyCreatedDBObject(br2, true);

                                foreach (ObjectId id3 in tr2.GetObject(blockId, OpenMode.ForWrite) as BlockTableRecord)
                                {
                                    AttributeDefinition definition = tr2.GetObject(id3, OpenMode.ForRead) as AttributeDefinition;
                                    if (definition != null)
                                    {
                                        using (AttributeReference reference2 = new AttributeReference())
                                        {
                                            reference2.SetAttributeFromBlock(definition, br2.BlockTransform);

                                            reference2.TextString = string.Format(CultureInfo.InvariantCulture, "{0:0}", data.val); //definition.TextString;

                                            br2.AttributeCollection.AppendAttribute(reference2);
                                            tr2.AddNewlyCreatedDBObject(reference2, true);
                                        }
                                    }
                                }

                            }
                        }
                    }

                    tr2.Commit();
                }
                double refArgument = r;
                theoryPoints.Clear();
                

            }
            else
            {
                Application.ShowAlertDialog("Select at least 3 points before use this button");
            }
            
        }
        public static double xyz()
        {
            
            double x = 2;
            return x;
            
        }
        
        public static void BestFitPlane()
        {

            int licznik = theoryPoints.Count();
            if (licznik > 2)
            {
                Matrix<double> MA = Matrix<double>.Build.Dense(theoryPoints.Count(), 3);
                for (int i = 0; i < theoryPoints.Count(); i++)
                {
                    MA[i, 0] = theoryPoints.ElementAt(i).X;
                    MA[i, 1] = theoryPoints.ElementAt(i).Y;
                    MA[i, 2] = 1;
                }
                Matrix<double> CenterSum = Matrix<double>.Build.Dense(3, 1);
                Matrix<double> MB = Matrix<double>.Build.Dense(theoryPoints.Count(), 1);
                for (int i = 0; i < theoryPoints.Count(); i++)
                {
                    MB[i, 0] = theoryPoints.ElementAt(i).Z;

                    CenterSum[0, 0] += theoryPoints.ElementAt(i).X;
                    CenterSum[1, 0] += theoryPoints.ElementAt(i).Y;
                    CenterSum[2, 0] += theoryPoints.ElementAt(i).Z;
                }
                
                var B = MA.Solve(MB);

                double a0 = B[0, 0];
                double a1 = B[1, 0];
                double b = B[2, 0];

                //Application.ShowAlertDialog(B.ToString());

                //double D = xi2 * yi2 - xiyi * xiyi;
                //double a = yihi * xiyi - xihi * yi2;
                //double b = xiyi * xihi - xi2 * yihi;

                //string Ds = D.ToString();
                //string ass = a.ToString();
                //string bs = b.ToString();
                //ZwSoft.ZwCAD.ApplicationServices.Application.ShowAlertDialog(Ds + " "+ ass + " "+ bs + " ");

                List<MyPoint3D> calculate = new List<MyPoint3D>();

                calculate.Add(new MyPoint3D(0, 0, b, "2"));
                calculate.Add(new MyPoint3D(100, 0, a0*100+b, "3"));
                calculate.Add(new MyPoint3D(0, 100,a1*100+ b, "1"));

                                
                double azimuthB1 = 100;
                double azimuthB2 = calculate[0].Z - calculate[1].Z;


                double A11=0;
                double B11=0;

                
                double azimuthB11 = 100;
                double azimuthB22 = calculate[0].Z - calculate[2].Z;
               
                A11 =  Math.Atan(azimuthB1 / azimuthB2);
                B11 =  Math.Atan(azimuthB11 / azimuthB22);

                if(A11<0)

                    A11 =  Math.PI / 2 + Math.Atan(azimuthB1 / azimuthB2);
                else
                    A11 = - Math.PI / 2 + Math.Atan(azimuthB1 / azimuthB2);

                if (B11 < 0)

                    B11 =  Math.PI/2 + Math.Atan(azimuthB11 / azimuthB22);
                else
                    B11 = - Math.PI / 2 + Math.Atan(azimuthB11 / azimuthB22);

                                
                List<MyPoint3D> calculate2 = new List<MyPoint3D>();
                for(int i = 0; i < theoryPoints.Count(); i++)
                {
                    calculate2.Add(new MyPoint3D(theoryPoints.ElementAt(i).X*Math.Cos(-A11)- theoryPoints.ElementAt(i).Z * Math.Sin(-A11),
                        theoryPoints.ElementAt(i).Y,
                        theoryPoints.ElementAt(i).X * Math.Sin(-A11) + theoryPoints.ElementAt(i).Z * Math.Cos(-A11),
                        "i"));
                    
                }
                List<MyPoint3D> calculate3 = new List<MyPoint3D>();
                for (int i = 0; i < theoryPoints.Count(); i++)
                {
                    calculate3.Add(new MyPoint3D(calculate2.ElementAt(i).X,
                        calculate2.ElementAt(i).Y * Math.Cos(-B11) - calculate2.ElementAt(i).Z * Math.Sin(-B11),
                        calculate2.ElementAt(i).Y * Math.Sin(-B11) + calculate2.ElementAt(i).Z * Math.Cos(-B11),
                        "i"));

                }
                Matrix<double> h0 = Matrix<double>.Build.Dense(1, 1);
                for (int i = 0; i < theoryPoints.Count(); i++)
                {
                    h0 += calculate3.ElementAt(i).Z;
                }

                double h00 = h0[0, 0] / theoryPoints.Count();

                List<MyPoint3D> calculate4 = new List<MyPoint3D>();
                for (int i = 0; i < theoryPoints.Count(); i++)
                {
                    calculate4.Add(new MyPoint3D(calculate3.ElementAt(i).X,
                        calculate3.ElementAt(i).Y,
                        (calculate3.ElementAt(i).Z - h00),
                        "i"));

                }
                
                CadDrawPoints.Draw(calculate4.ToArray(), "bestfitplane", Color.FromColor(System.Drawing.Color.DeepSkyBlue), deleteLayerIfExist: false);
               

                theoryPoints.Clear();
                fittedPlane.Clear();

            }
            else
            {
                Application.ShowAlertDialog("Select at least 3 points before use this button");
            }

        }

        public static void Rotation()
        {
            var resultX = MessageBox.Show("(YES) Set rotation angle for X and Y, (NO) set rotation only for X", "Rotation in Degree , separator)", MessageBoxButtons.YesNo);
            double A11 = 0;
            double B11 = 0;
            if (resultX == DialogResult.Yes)
            {
                Document acDoc2 = Application.DocumentManager.MdiActiveDocument;
                PromptStringOptions Zdata = new PromptStringOptions("\nEnter New Angle for X ROTATION");
                Zdata.AllowSpaces = true;
                PromptResult Zdata2 = acDoc2.Editor.GetString(Zdata);

                B11 = Convert.ToDouble(Zdata2.StringResult) * Math.PI / 180;
                
            }

            Document acDoc20 = Application.DocumentManager.MdiActiveDocument;
            PromptStringOptions Zdata0 = new PromptStringOptions("\nEnter New Angle for Y ROTATION");
            Zdata0.AllowSpaces = true;
            PromptResult Zdata20 = acDoc20.Editor.GetString(Zdata0);

            A11 = Convert.ToDouble(Zdata20.StringResult) * Math.PI / 180;


            List<MyPoint3D> rotation1 = new List<MyPoint3D>();
            for (int i = 0; i < theoryPoints.Count(); i++)
            {
                rotation1.Add(new MyPoint3D(theoryPoints.ElementAt(i).X * Math.Cos(-A11) - theoryPoints.ElementAt(i).Z * Math.Sin(-A11),
                    theoryPoints.ElementAt(i).Y,
                    theoryPoints.ElementAt(i).X * Math.Sin(-A11) + theoryPoints.ElementAt(i).Z * Math.Cos(-A11),
                    "i"));

            }

            List<MyPoint3D> rotation2 = new List<MyPoint3D>();
            for (int i = 0; i < theoryPoints.Count(); i++)
            {
                rotation2.Add(new MyPoint3D(rotation1.ElementAt(i).X,
                    rotation1.ElementAt(i).Y * Math.Cos(-B11) - rotation1.ElementAt(i).Z * Math.Sin(-B11),
                    rotation1.ElementAt(i).Y * Math.Sin(-B11) + rotation1.ElementAt(i).Z * Math.Cos(-B11),
                    "i"));
            }

            CadDrawPoints.Draw(rotation2.ToArray(), "RotatedPoints", Color.FromColor(System.Drawing.Color.PaleVioletRed), deleteLayerIfExist: false);


            theoryPoints.Clear();


        }
            private static (string BlockName, double val) FindBlockNameX(MatchedPoint matchedPoint)
        {
            double val = matchedPoint.RealPoint.X - matchedPoint.TheoryPoint.X;

            if (A_3DArrow == true)
            {
                if (val < 0)
                {
                    return (A_NX, val);
                }

                return (A_PX, val);
        }

            else
            {
                if (val< 0)
                {
                    return (A_NX2D, val);
                }

                return (A_PX2D, val);
            }
        }

        private static (string BlockName, double val) FindBlockNameY(MatchedPoint matchedPoint)
        {
            double val = matchedPoint.RealPoint.Y - matchedPoint.TheoryPoint.Y;
            if (A_3DArrow)
            {
                if (val < 0)
                {
                    return (A_NY, val);
                }

                return (A_PY, val);
            }

            else
            {
                if (val < 0)
                {
                    return (A_NY2D, val);
                }

                return (A_PY2D, val);
            }
        }

        private static (string BlockName, double val) FindBlockNameZ(MatchedPoint matchedPoint)
        {
            double val = Math.Round( matchedPoint.RealPoint.Z - matchedPoint.TheoryPoint.Z, MidpointRounding.AwayFromZero); //zaokrąglenie AwayFromZero - geodzyjne zaokrąglenie
            if (A_3DArrow)
            {
                if (val == 0)
                {
                    return (A_PZ, val);
                }
                if (val < 0)
                {
                    return (A_NZ, val);
                }
                return (A_PZ, val);
            }
            else
            {
                if (val == 0)
                {
                    return (A_PZ02D, val);
                }
                if (val < 0)
                {
                    return (A_NZ2D, val);
                }
                return (A_PZ2D, val);

            }
        }
        private static (string BlockName, double val) FindBlockNameZ2(MatchedPoint matchedPoint)
        {
            double val = Math.Round(matchedPoint.RealPoint.Z - Zteor, MidpointRounding.AwayFromZero); //zaokrąglenie AwayFromZero - geodzyjne zaokrąglenie
           // A_PZ2D, A_NZ2D, A_PZ0

                if (val == 0)
                {
                    return (A_PZ0, val);
                }
                if (val < 0)
                {
                    return (A_NZNIV, val);
                }
                return (A_PZNIV, val);
                      
        }

        private static (string BlockName, double val) FindBlockNameR(MatchedPoint matchedPoint2)
        {

                double deltax2 =  (matchedPoint2.TheoryPoint.X - CCircle[0, 0]) * (matchedPoint2.TheoryPoint.X - CCircle[0, 0]);
                double deltay2 =  (matchedPoint2.TheoryPoint.Y - CCircle[1, 0]) * (matchedPoint2.TheoryPoint.Y - CCircle[1, 0]);

                double mdata =  Math.Sqrt(deltax2 + deltay2) - CCircle[2, 0];

                double val = Math.Round(mdata, MidpointRounding.AwayFromZero);

                return (A_PZ0, val);
            
        }
        private static (string BlockName, double val) FindBlockNameP(MatchedPoint matchedPoint)
        {

            //double deltaZ = matchedPoint.FittedPlane.Z;


            double mdata = 0;

            double val = Math.Round(mdata, MidpointRounding.AwayFromZero);

            if (val == 0)
            {
                return (A_PZ02D, val);
            }
            if (val < 0)
            {
                return (A_NZ, val);
            }
            return (A_PZ, val);

        }


        private static string PointListToString(List<MatchedPoint> points)
        {
            StringBuilder sb = new StringBuilder();

            foreach (MatchedPoint point in points)
            {
                sb.Append("Real Point: ").Append(point.RealPoint).Append("   Theory Point: ").Append(point.TheoryPoint)
                    .Append(System.Environment.NewLine);
            }

            return sb.ToString();
        }

        public static void InsertBlock(Point3d insPt, string blockName)
        {
            var doc = ZwSoft.ZwCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            using (var tr = db.TransactionManager.StartTransaction())
            {
                // check if the block table already has the 'blockName'" block
                var bt = (BlockTable)tr.GetObject(db.BlockTableId, OpenMode.ForRead);
                if (!bt.Has(blockName))
                {
                    try
                    {
                        // search for a dwg file named 'blockName' in AutoCAD search paths
                        var filename = HostApplicationServices.Current.FindFile(blockName + ".dwg", db, FindFileHint.Default);
                        // add the dwg model space as 'blockName' block definition in the current database block table
                        using (var sourceDb = new Database(false, true))
                        {
                            sourceDb.ReadDwgFile(filename, FileOpenMode.OpenForReadAndAllShare, true, "");
                            db.Insert(blockName, sourceDb, true);
                        }
                    }
                    catch
                    {
                        ed.WriteMessage($"\nBlock '{blockName}' not found.");
                        return;
                    }
                }

                // create a new block reference
                using (var br = new BlockReference(insPt, bt[blockName]))
                {
                    var space = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                    space.AppendEntity(br);
                    tr.AddNewlyCreatedDBObject(br, true);
                }
                tr.Commit();
            }
        }



        private static void Clean()
        {
            realPoints.Clear();
            theoryPoints.Clear();
        }
        
        }

     }


