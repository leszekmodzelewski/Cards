using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using PointCalc;
using ZwSoft.ZwCAD.DatabaseServices;
using ZwSoft.ZwCAD.Geometry;
using ZwSoft.ZwCAD.Runtime;
using Application = ZwSoft.ZwCAD.ApplicationServices.Application;
using ZwSoft.ZwCAD.ApplicationServices;
using ZwSoft.ZwCAD.EditorInput;
using System.Windows.Forms;
using GeoLib.Commands;





// todoo:

// jak rozibć okienko z długością 

// zrobić automat do uzupełniania danych ( kątowych i długości)

namespace GeoLib.Entities.RectBlanking
{
    public class Cmd_FitRails : Cmd_ArrowReadPoints
    {

        [CommandMethod("FITRAILS", CommandFlags.UsePickSet)]
        public void ExportToFile()
        {
            //Commands.Commands.ExplodeBlockByNameCommand();
            //FitRails2.AddTheoryPoints(ReadPoints(countPoints: false));
            //FitRails2.RailsArrow();

        }
    }

    public class FitRails2
    {
        private const string A_PY = "+Y";
        private const string A_NY = "-Y";
        private static double dist = 0;
        private static double defaultTableSize = 100;

        static List<MyPoint3D> theoryPoints = new List<MyPoint3D>();


        public static void AddTheoryPoints(List<MyPoint3D> points)
        {
            // TODO: Avoid duplication
            theoryPoints.AddRange(points);
        }

        public static void RailsArrow()
        {

            MessageBox.Show("Select line and Enter theoretical distance", "Line and distance");


            Document mdiActiveDocument = Application.DocumentManager.MdiActiveDocument;
            Editor editor = mdiActiveDocument.Editor;

            List<Line> linia = new List<Line>();

            var db2 = Application.DocumentManager.MdiActiveDocument.Database;
            int index = 0;
            while (index < 1)
            {
                PromptEntityResult entity = editor.GetEntity($"Spelect line ");
                if (entity.Status != PromptStatus.OK)
                {
                    return;
                }

                using (var tr = db2.TransactionManager.StartTransaction())
                {
                    Entity ent = tr.GetObject(db2.CurrentSpaceId, OpenMode.ForRead) as Entity;

                    Line L1 = new Line();

                    if (ent.GetType() == typeof(Line))

                    {

                        L1 = tr.GetObject(db2.CurrentSpaceId, OpenMode.ForRead) as Line; //?
                        Line L2 = new Line(L1.StartPoint, L1.EndPoint);
                        linia.Add(L2);

                    }

                   // if (L1 != null)

                        tr.AddNewlyCreatedDBObject(L1, true); //?

                    tr.Commit();
                }
            }

            MessageBox.Show(linia.ElementAt(0).ToString(), "Rails");

            MessageBox.Show("Enter theretical distance between rails", "Rails");
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            PromptStringOptions Zdata = new PromptStringOptions("\nEnter theretical distance between rails");
            Zdata.AllowSpaces = true;
            PromptResult Zdata2 = acDoc.Editor.GetString(Zdata);
            dist = Convert.ToDouble(Zdata2.StringResult);

            
            MessageBox.Show("Enter leveling vlaue", "Leveling Vlaue");
            PromptStringOptions Zdat = new PromptStringOptions("\nEnter scale");
            if (Zdat != null)
            {
                Zdat.AllowSpaces = true;
                PromptResult Zdat2 = acDoc.Editor.GetString(Zdat);
                defaultTableSize = Convert.ToDouble(Zdat2.StringResult) * 100;
            }
            var db = Application.DocumentManager.MdiActiveDocument.Database; //1
            var bt = AppUtils.EnsureBlocks(db, new[] { A_NY, A_PY });//2 - 1i2 pobierają bloki do uzupełnienia

            PointCalculator pc = new PointCalculator(); //tworzy odowłanie do point calklatora jako programu liczącego w dany sposób
            var matched = pc.Calculate(theoryPoints.ToArray(), theoryPoints.ToArray(), Double.MaxValue);

            foreach (var matchedPoint in matched)


                using (var tr = db.TransactionManager.StartTransaction()) //auocad start transmisji
                {
                    {
                        var blockInsertionPoint = new Point3d(matchedPoint.TheoryPoint.X, matchedPoint.TheoryPoint.Y, matchedPoint.TheoryPoint.Z);


                        var blockNameX = FindBlockNameX(matchedPoint);


                        AddBlock(blockNameX);




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
                        tr.Commit();
                    }

                }
        }

        private static (string BlockName, double val) FindBlockNameX(MatchedPoint matchedPoint)
        {

            double a = matchedPoint.TheoryPoint.Y;

            if (a < 0)
            {
                double val1 = Math.Abs(matchedPoint.TheoryPoint.Y) - Math.Abs(dist / 2);
                if (val1 < 0)
                {
                    return (A_PY, val1);
                }
                return (A_NY, val1);
            }

            double val = Math.Abs(dist / 2) - Math.Abs(matchedPoint.TheoryPoint.Y);
            if (val < 0)
            {
                return (A_PY, val);
            }

            return (A_NY, val);

        }
    }

}