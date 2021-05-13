

using System.Collections.Generic;
using System.Linq;
using System.Windows.Documents;
using System.Windows.Forms;
using GeoLib.Entities.RectBlanking;
using PointCalc;
using ZwSoft.ZwCAD.Colors;

namespace GeoLib.Entities.TableRef
{
    using ZwSoft.ZwCAD.ApplicationServices;
    using ZwSoft.ZwCAD.DatabaseServices;
    using ZwSoft.ZwCAD.EditorInput;
    using ZwSoft.ZwCAD.Geometry;
    using ZwSoft.ZwCAD.Runtime;
    using GeoLib;
    using GeoLib.Entities;
    using GeoLib.Entities.Table;
    using GeoLib.XData;
    using System;

    public class Cmd_DrawTheoryPoints
    {
        // DKO: Find MTEXT and POINT vertex
        [CommandMethod("DRAWTHEORYPOINTS", CommandFlags.UsePickSet)]
        public void DrawTheoryPoints()
        {
            var theoryPoints = CalculationUtils.ReadTheoryPointsFromCad().ToArray();

            if (!theoryPoints.Any())
                return;

            Document mdiActiveDocument = Application.DocumentManager.MdiActiveDocument;
            Database database = mdiActiveDocument.Database;

            string theoryLayer = GetOrCreateTheoryLayer(database);

            using (Transaction transaction = mdiActiveDocument.Database.TransactionManager.StartTransaction())
            {
                var acBlkTbl = transaction.GetObject(database.BlockTableId, OpenMode.ForRead) as BlockTable;

                // Open the Block table record Model space for write
                var acBlkTblRec = transaction.GetObject(acBlkTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                foreach (var point3D in theoryPoints)
                {

                    var point = new DBPoint(new Point3d(point3D.X, point3D.Y, point3D.Z))
                    {
                        Color = Color.FromColor(System.Drawing.Color.Yellow)
                    };

                    point.SetDatabaseDefaults();
                    point.Layer = theoryLayer;
                    acBlkTblRec.AppendEntity(point);
                    transaction.AddNewlyCreatedDBObject(point, true);

                }

                //// Set the style for all point objects in the drawing
                //database.Pdmode = 34;
                //database.Pdsize = 1;
                // http://docs.autodesk.com/ACD/2010/ENU/AutoCAD%20.NET%20Developer%27s%20Guide/index.html?url=WS1a9193826455f5ff2566ffd511ff6f8c7ca-415b.htm,topicNumber=d0e15219

                transaction.Commit();
            }
        }

        private const string theoryLayerName = "TheoryPoints";

        private string GetOrCreateTheoryLayer(Database database)
        {
            using (Transaction acTrans = database.TransactionManager.StartTransaction())
            {
                // Open the Layer table for read
                var acLyrTbl = acTrans.GetObject(database.LayerTableId, OpenMode.ForRead) as LayerTable;

                if (acLyrTbl.Has(theoryLayerName) == false)
                {
                    LayerTableRecord acLyrTblRec = new LayerTableRecord();

                    acLyrTblRec.Color = Color.FromColor(System.Drawing.Color.Yellow);
                    acLyrTblRec.Name = theoryLayerName;
                    acLyrTblRec.IsHidden = false;
                    acLyrTblRec.IsLocked = true;

                    // Upgrade the Layer table for write
                    acLyrTbl.UpgradeOpen();

                    // Append the new layer to the Layer table and the transaction
                    acLyrTbl.Add(acLyrTblRec);
                    acTrans.AddNewlyCreatedDBObject(acLyrTblRec, true);
                }

                // Save the changes and dispose of the transaction
                acTrans.Commit();
            }

            return theoryLayerName;
        }
    }
}

