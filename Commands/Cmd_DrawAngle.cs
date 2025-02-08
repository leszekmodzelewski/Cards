using GeoLib.Entities.Table;
using GeoLib.Entities.TableRef;
using PointCalc;
using System.Collections.Generic;
using ZwSoft.ZwCAD.ApplicationServices;
using ZwSoft.ZwCAD.Colors;
using ZwSoft.ZwCAD.DatabaseServices;
using ZwSoft.ZwCAD.EditorInput;
using ZwSoft.ZwCAD.Geometry;
using ZwSoft.ZwCAD.Runtime;
using Application = ZwSoft.ZwCAD.ApplicationServices.Application;
using System;
using System.Numerics;

namespace GeoLib.Commands
{
    public class Cmd_DrawAngle
    {
        private const string A_PZ0 = "Z0";
        private static double A1; // musi być static zeby działało :)
        private static double B1;
        [CommandMethod("DRAWANGLE", CommandFlags.UsePickSet)]
        public void Execute()
        {
            
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            PromptStringOptions Zdat = new PromptStringOptions("\nEnter scale");
            Zdat.AllowSpaces = true;
            PromptResult Zdat2 = acDoc.Editor.GetString(Zdat);
            double defaultTableSize = Convert.ToDouble(Zdat2.StringResult) * 100;
            // addblock br.ScaleFactors = new Scale3d(defaultTableSize / 150.0);

            Document mdiActiveDocument = Application.DocumentManager.MdiActiveDocument;
            Editor editor = mdiActiveDocument.Editor;
            Point3d origin = Point3d.Origin;
            TableWindowModel model = null;
            ObjectId objectId = ObjectId.Null;
            // PromptSelectionResult result = editor.SelectImplied();
            // if ((result.Value != null) && (result.Value.Count == 3))
            // {
            //     @null = result.Value[0].ObjectId;
            //     model = TryGetModel(mdiActiveDocument.Database, @null, out origin);
            // }

            string searchFor = "123";
            int index = 0;

            var coord = new List<Point3d>();

            while (index < 3)
            {
                PromptEntityResult entity = editor.GetEntity($"Specify {searchFor[index]} point for left angle");
                if (entity.Status != PromptStatus.OK)
                {
                    return;
                }
                objectId = entity.ObjectId;


                var point = TryGetPoint(mdiActiveDocument.Database, objectId);

                if (point.HasValue)
                {
                    coord.Add(point.Value);
                    index++;
                }
            }

            if (coord.Count == 3)
            {
                //STOPNIE(ARG.LICZBY.ZESP(LICZBA.ZESP(A6;B6)))


                double azimuthA1 = coord[0].X - coord[1].X;
                double azimuthA2 = coord[0].Y - coord[1].Y;
                double azimuthB1 = coord[1].X - coord[2].X;
                double azimuthB2 = coord[1].Y - coord[2].Y;

                if (azimuthA1 >= 0)
                {
                    if (azimuthA2 >= 0)
                    {
                        A1 = 180 + 180 / Math.PI * Math.Atan(azimuthA1 / azimuthA2);
                    }

                    if (azimuthA2 < 0)
                    {
                        A1 = 360 + 180 / Math.PI * Math.Atan(azimuthA1 / azimuthA2);
                    }

                }
                else
                {
                    if (azimuthA2 >= 0)
                    {
                        A1 = 180 + 180 / Math.PI * Math.Atan(azimuthA1 / azimuthA2);
                    }
                    else
                        A1 = 180 / Math.PI * Math.Atan(azimuthA1 / azimuthA2);
                }

                if (azimuthB1 >= 0)
                {
                    if (azimuthB2 >= 0)
                    {
                        B1 = 180 + 180 / Math.PI * Math.Atan(azimuthB1 / azimuthB2);
                    }

                    if (azimuthB2 < 0)
                    {
                        B1 =  180 / Math.PI * Math.Atan(azimuthB1 / azimuthB2);
                    }

                }
                else
                {
                    if (azimuthB2 >= 0)
                    {
                        B1 = 180 + 180 / Math.PI * Math.Atan(azimuthB1 / azimuthB2);
                    }
                    else
                        B1 = 180 / Math.PI * Math.Atan(azimuthB1 / azimuthB2); //???
                }
            }

            var db = Application.DocumentManager.MdiActiveDocument.Database;
            var bt = AppUtils.EnsureBlocks(db, new[] { A_PZ0 });//2 - 1i2 pobierają bloki do uzupełnienia


            using (var tr = db.TransactionManager.StartTransaction()) //auocad start transmisji
            {
                var blockInsertionPoint = new Point3d(coord[1].X, coord[1].Y, coord[1].Z);

                var blockNameZ = FindBlockNameA();

                AddBlock(blockNameZ);

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
                                    reference2.TextString = string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0:0.00°}", data.val); //definition.TextString;
                                    br.AttributeCollection.AppendAttribute(reference2);
                                    tr.AddNewlyCreatedDBObject(reference2, true);
                                }
                            }
                        }
                    }
                    
                }
                tr.Commit();
            }
        }

        private Point3d? TryGetPoint(Database database, ObjectId objectId)
        {
            using (Transaction acTrans = database.TransactionManager.StartTransaction())
            {
                var point = acTrans.GetObject(objectId, OpenMode.ForRead) as DBPoint;
                if (point != null)
                {
                    return point.Position;
                }
            }

            return null;
        }
        private static (string BlockName, double val) FindBlockNameA()
        {
            double val;
            if (A1 - B1 > 180) //stosujemy, gdy 0g ≤ AAB < 200g, a „–”, gdy 200g ≤ AAB < 400g
            {
                val = 180 + (360 - A1 + B1);
            }
            else

                val = 180 - A1 + B1;

            return (A_PZ0, val);
        }
    }
}