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
using AcAp = ZwSoft.ZwCAD.ApplicationServices.Application;
using System;
using System.Numerics;

namespace GeoLib.Commands
{
    public class Commands
    {

        //    public static void GetObjects()
        //    {
        //        var doc = AcAp.DocumentManager.MdiActiveDocument;
        //        var db = doc.Database;
        //        var ed = doc.Editor;

        //           foreach (var item in GetEntities(db))
        //        {
        //            ed.WriteMessage($"\n{item.Key.Handle,-6} {item.Value}");
        //        }

        //    }

        //    static Dictionary<ObjectId, string> GetEntities(Database db)
        //    {

        //        var dict = new Dictionary<ObjectId, string>();
        //        using (var tr = db.TransactionManager.StartTransaction())
        //        {
        //            var bt = (BlockTable)tr.GetObject(db.LinetypeTableId, OpenMode.ForRead);
        //            foreach (var btrId in bt)
        //            {
        //                var btr = (BlockTableRecord)tr.GetObject(btrId, OpenMode.ForRead);
        //                if (btr.IsLayout)
        //                {
        //                    foreach (var id in btr)
        //                    {
        //                        dict.Add(id, id.ObjectClass.Name);
        //                    }
        //                }
        //            }
        //            tr.Commit();
        //        }
        //        return dict;
        //    }
        //}
    //    public static void ExplodeBlockByNameCommand(string blockToExplode)
    //{
    //    bool explodeResult = true;
    //    Document bDwg = Application.DocumentManager.MdiActiveDocument;
    //    Editor ed = bDwg.Editor;
    //    Database db = bDwg.Database;
    //    ZwSoft.ZwCAD.DatabaseServices.TransactionManager bTransMan = bDwg.TransactionManager;
    //    using (Transaction bTrans = bTransMan.StartTransaction())
    //    {
    //        try
    //        {
    //            BlockTable bt = (BlockTable)bTrans.GetObject(db.BlockTableId, OpenMode.ForRead);

    //            ed.WriteMessage("nProcessing {0}", blockToExplode);

    //            if (bt.Has(blockToExplode))
    //            {
    //                ObjectId blkId = bt[blockToExplode];
    //                BlockTableRecord btr = (BlockTableRecord)bTrans.GetObject(blkId, OpenMode.ForRead);
    //                ObjectIdCollection blkRefs = btr.GetBlockReferenceIds(true, true);

    //                foreach (ObjectId blkXId in blkRefs)
    //                {
    //                    //create collection for exploded objects
    //                    DBObjectCollection objs = new DBObjectCollection();

    //                    //handle as entity and explode
    //                    Entity ent = (Entity)bTrans.GetObject(blkXId, OpenMode.ForRead);
    //                    ent.Explode(objs);
    //                    ed.WriteMessage("nExploded an Instance of {0}", blockToExplode);

    //                    //erase Block
    //                    ent.UpgradeOpen();
    //                    ent.Erase();

    //                    BlockTableRecord btrCs = (BlockTableRecord)bTrans.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);

    //                    foreach (DBObject obj in objs)
    //                    {
    //                        Entity ent2 = (Entity)obj;
    //                        btrCs.AppendEntity(ent2);
    //                        bTrans.AddNewlyCreatedDBObject(ent2, true);
    //                    }

    //                }
    //            }

    //            bTrans.Commit();
    //        }
    //        catch
    //        {
    //            ed.WriteMessage("nSomething went wrong");
    //            explodeResult = false;
    //        }
    //        finally
    //        {
    //        }
    //        ed.WriteMessage("n");
    //        bTrans.Dispose();
    //        bTransMan.Dispose();
    //    }

        //return explodeResult; //return wheter the method was succesful or not

    //}
}
}
