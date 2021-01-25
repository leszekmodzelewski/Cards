using System;
using System.Collections.Generic;
using System.Windows.Media.Media3D;

using ZwSoft.ZwCAD.ApplicationServices;
using ZwSoft.ZwCAD.Colors;
using ZwSoft.ZwCAD.DatabaseServices;
using ZwSoft.ZwCAD.Geometry;
using ZwSoft.ZwCAD.Runtime;
using Application = ZwSoft.ZwCAD.ApplicationServices.Application;

namespace GeoLib.Entities.Table
{
    public class Cmd_DwgCreate
    {
        [CommandMethod("CARDSDWGCREATE", CommandFlags.UsePickSet)]
        public void DwgCreate()
        {
            try
            {
                //Application.DocumentManager.MdiActiveDocument.Database

                if (FileCoordsDataUtils.Points == null || FileCoordsDataUtils.Points.Count == 0)
                {
                    Application.ShowAlertDialog("Please load file with points first.");
                    return;
                }

                // TODO: DKO: protect against MdiActiveDocument.Database empty

                string strTemplatePath = "acad.dwt";

                var sourceDb = Application.DocumentManager.MdiActiveDocument.Database;

                DocumentCollection acDocMgr = Application.DocumentManager;
                Document acDoc = acDocMgr.Add(strTemplatePath);

                Application.Idle += delegate
                {
                    AddPointsToDrawing(acDocMgr.MdiActiveDocument, sourceDb, FileCoordsDataUtils.Points);
                };


                acDocMgr.MdiActiveDocument = acDoc;



               

                //LockDoc();
            }
            catch (System.Exception e)
            {
                Console.WriteLine(e);
            }
        }




        private void SetPointType(Database database)
        {
            database.Pdmode = 35;
            database.Pdsize = 50;
        }

        private static bool k = false;
        private void AddPointsToDrawing(Document destinationDocument, Database sourceDocument, List<Point3D> pointsFromFile)
        {
            //CreateLayer("")
            if (k == false)
            {
                k = true;
                using (DocumentLock acLckDoc = destinationDocument.LockDocument())
                {
                    AddPointsToDatabase(destinationDocument.Database, pointsFromFile);
                }
            }
        }

        public void AddPointsToDatabase(Database acCurDb, List<Point3D> points)
        {


            // Start a transaction
            using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
            {
                // Open the Block table for read
                BlockTable acBlkTbl;
                acBlkTbl = acTrans.GetObject(acCurDb.BlockTableId,
                    OpenMode.ForRead) as BlockTable;

                // Open the Block table record Model space for write
                BlockTableRecord acBlkTblRec;
                acBlkTblRec = acTrans.GetObject(acBlkTbl[BlockTableRecord.ModelSpace],
                    OpenMode.ForWrite) as BlockTableRecord;

                foreach (var point in points)
                {
                    AddPointToDb(acBlkTblRec, acTrans, point);
                }

                SetPointType(acCurDb);
                // Save the new object to the database
                acTrans.Commit();
            }
        }

        private static void AddPointToDb(BlockTableRecord acBlkTblRec, Transaction acTrans, Point3D point)
        {
            DBPoint acPoint = new DBPoint(new Point3d(point.X, point.Y, point.Z));

            acPoint.SetDatabaseDefaults();

            // Add the new object to the block table record and the transaction
            acBlkTblRec.AppendEntity(acPoint);
            acTrans.AddNewlyCreatedDBObject(acPoint, true);
        }

        private void CreateLayer(string layerName, Database acCurDb)
        {
            using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
            {
                // Open the Layer table for read
                LayerTable acLyrTbl;
                acLyrTbl = acTrans.GetObject(acCurDb.LayerTableId,
                    OpenMode.ForRead) as LayerTable;

                string sLayerName = "Center";

                if (acLyrTbl.Has(sLayerName) == false)
                {
                    LayerTableRecord acLyrTblRec = new LayerTableRecord();

                    // Assign the layer the ACI color 1 and a name
                    acLyrTblRec.Color = Color.FromColorIndex(ColorMethod.ByAci, 1);
                    acLyrTblRec.Name = sLayerName;

                    // Upgrade the Layer table for write
                    acLyrTbl.UpgradeOpen();

                    // Append the new layer to the Layer table and the transaction
                    acLyrTbl.Add(acLyrTblRec);
                    acTrans.AddNewlyCreatedDBObject(acLyrTblRec, true);
                }

                // Open the Block table for read
                BlockTable acBlkTbl;
                acBlkTbl = acTrans.GetObject(acCurDb.BlockTableId,
                    OpenMode.ForRead) as BlockTable;

                // Open the Block table record Model space for write
                BlockTableRecord acBlkTblRec;
                acBlkTblRec = acTrans.GetObject(acBlkTbl[BlockTableRecord.ModelSpace],
                    OpenMode.ForWrite) as BlockTableRecord;

                // Create a circle object
                Circle acCirc = new Circle();
                acCirc.SetDatabaseDefaults();
                acCirc.Center = new Point3d(2, 2, 0);
                acCirc.Radius = 1;
                acCirc.Layer = sLayerName;

                acBlkTblRec.AppendEntity(acCirc);
                acTrans.AddNewlyCreatedDBObject(acCirc, true);

                // Save the changes and dispose of the transaction
                acTrans.Commit();
            }
        }
    }
}