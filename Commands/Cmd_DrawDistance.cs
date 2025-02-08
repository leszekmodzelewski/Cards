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
    public class Cmd_DrawDistance
    {
        private const string A_PZ0 = "Z0";
        private static double Distance; // musi być static zeby działało :)
        
        [CommandMethod("DRAWDISTNACE", CommandFlags.UsePickSet)]
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

            string searchFor = "xyz";
            int index = 0;

            var coord = new List<Point3d>();

            while (index < 2)
            {
                PromptEntityResult entity = editor.GetEntity($"Specify point for {searchFor[index]} value");
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

            if (coord.Count == 2)
            {
                double dX = coord[0].X - coord[1].X;
                double dY = coord[0].Y - coord[1].Y;
                Distance = Math.Sqrt(dX * dX + dY * dY);
               
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
                                    reference2.TextString = string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0:0}", data.val); //definition.TextString;
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
            double val = Distance;


            return (A_PZ0, val);
        }
    }
}