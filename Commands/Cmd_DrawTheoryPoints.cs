﻿

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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


    public static class CadDrawPoints
    {
        public static void Draw(MyPoint3D[] points, string layerName = null, Color color = null, bool deleteLayerIfExist = true) //bool deleteLayerIfExist = warunek dla innego programu 
        {
           
            if (!points.Any())
                return;
          

            Document mdiActiveDocument = Application.DocumentManager.MdiActiveDocument;
            Database database = mdiActiveDocument.Database;

            if (deleteLayerIfExist == true)

                SetLayerLock(database, layerName, false);

            if (deleteLayerIfExist == true)
            DeleteExistingTheoryLayer(database, layerName);

            string layerForPoint = null;

            if (!string.IsNullOrEmpty(layerName))
            {
                layerForPoint = GetOrCreateTheoryLayer(database, layerName);
            }
            

            using (Transaction transaction = mdiActiveDocument.Database.TransactionManager.StartTransaction())
            {
                var acBlkTbl = transaction.GetObject(database.BlockTableId, OpenMode.ForRead) as BlockTable;

                // Open the Block table record Model space for write
                var acBlkTblRec = transaction.GetObject(acBlkTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;


                foreach (var point3D in points)
                {

                    var point = new DBPoint(new Point3d(point3D.Xo, point3D.Yo, point3D.Zo))
                    {
                        Color = Color.FromColor(System.Drawing.Color.Yellow)
                    };

                    point.SetDatabaseDefaults();
                    if (color != null)
                    {
                        point.Color = color;
                    }

                    if (!string.IsNullOrEmpty(layerForPoint))
                    {
                        point.Layer = layerForPoint;
                    }

                    acBlkTblRec.AppendEntity(point);
                    transaction.AddNewlyCreatedDBObject(point, true);

                }

                //// Set the style for all point objects in the drawing
                //database.Pdmode = 34;
                //database.Pdsize = 1;
                // http://docs.autodesk.com/ACD/2010/ENU/AutoCAD%20.NET%20Developer%27s%20Guide/index.html?url=WS1a9193826455f5ff2566ffd511ff6f8c7ca-415b.htm,topicNumber=d0e15219

                transaction.Commit();
            }

            if (deleteLayerIfExist == true)

                SetLayerLock(database, layerName, true);
        }

        private static string GetOrCreateTheoryLayer(Database database, string layerName)
        {
            using (Transaction acTrans = database.TransactionManager.StartTransaction())
            {
                // Open the Layer table for read
                var acLyrTbl = acTrans.GetObject(database.LayerTableId, OpenMode.ForRead) as LayerTable;

                if (acLyrTbl.Has(layerName) == false)
                {
                    LayerTableRecord acLyrTblRec = new LayerTableRecord();

                    acLyrTblRec.Color = Color.FromColor(System.Drawing.Color.Yellow);
                    acLyrTblRec.Name = layerName;
                    acLyrTblRec.IsHidden = false;
                    //acLyrTblRec.IsLocked = true;

                    // Upgrade the Layer table for write
                    acLyrTbl.UpgradeOpen();

                    // Append the new layer to the Layer table and the transaction
                    acLyrTbl.Add(acLyrTblRec);
                    acTrans.AddNewlyCreatedDBObject(acLyrTblRec, true);
                }

                // Save the changes and dispose of the transaction
                acTrans.Commit();
            }

            return layerName;
        }

        private static void SetLayerLock(Database database, string layerName, bool lockLayer)
        {
            using (Transaction acTrans = database.TransactionManager.StartTransaction())
            {
                // Open the Layer table for read
                var acLyrTbl = acTrans.GetObject(database.LayerTableId, OpenMode.ForRead) as LayerTable;
               // var acLyrTblRec = acTrans.GetObject(acLyrTbl[layerName], OpenMode.ForWrite) as LayerTableRecord;
               // acLyrTblRec.IsLocked = lockLayer;

                acTrans.Commit();
            }
        }

        private static void DeleteExistingTheoryLayer(Database database, string layerName)
        {
            var objToRemove = GetEntitiesOnLayer(layerName);
            if (objToRemove.Count > 0)
            {
                using (Transaction acTrans = database.TransactionManager.StartTransaction())
                {
                    var acLyrTbl = acTrans.GetObject(database.LayerTableId, OpenMode.ForRead) as LayerTable;
                    var acLyrTblRec = acTrans.GetObject(acLyrTbl[layerName], OpenMode.ForWrite) as LayerTableRecord;
                    acLyrTblRec.IsLocked = false;


                    foreach (ObjectId id in objToRemove)
                    {
                        var o = acTrans.GetObject(id, OpenMode.ForWrite);
                        o.Erase(true);
                    }
                    acLyrTblRec.IsLocked = true;
                    acTrans.Commit();
                }
            }

            return;
            using (Transaction acTrans = database.TransactionManager.StartTransaction())
            {
                // Open the Layer table for read
                var acLyrTbl = acTrans.GetObject(database.LayerTableId, OpenMode.ForRead) as LayerTable;
                if (acLyrTbl.Has(layerName) == false)
                {
                    return;
                }

                ObjectIdCollection acObjIdColl = new ObjectIdCollection();
                acObjIdColl.Add(acLyrTbl[layerName]);
                //database.Purge(acObjIdColl);

                if (acObjIdColl.Count > 0)
                {

                    var acLyrTblRec = acTrans.GetObject(acObjIdColl[0], OpenMode.ForWrite) as LayerTableRecord;

                    try
                    {
                        // Erase the unreferenced layer
                        acLyrTblRec.Erase(true);

                        // Save the changes and dispose of the transaction
                        acTrans.Commit();
                    }
                    catch (System.Exception ex)
                    {
                        // Layer could not be deleted
                        Application.ShowAlertDialog("Error:\n" + ex.Message);
                    }
                }


            }
        }

        private static ObjectIdCollection GetEntitiesOnLayer(string layerName)
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Editor ed = doc.Editor;

            // Build a filter list so that only entities

            // on the specified layer are selected

            TypedValue[] tvs = new TypedValue[1] { new TypedValue((int)DxfCode.LayerName, layerName) };
            SelectionFilter sf = new SelectionFilter(tvs);
            PromptSelectionResult psr = ed.SelectAll(sf);

            if (psr.Status == PromptStatus.OK)
                return new ObjectIdCollection(psr.Value.GetObjectIds());
            else
                return new ObjectIdCollection();
        }
    }

    public class Cmd_DrawTheoryPoints
    {
        // DKO: Find MTEXT and POINT vertex
        [CommandMethod("DRAWTHEORYPOINTS", CommandFlags.UsePickSet)]
        public void DrawTheoryPoints()
        {
            var theoryPoints = CalculationUtils.ReadTheoryPointsFromCad().ToArray();

            var res = theoryPoints.Distinct(new MyPoint3DComparer());

            if (res.Count() != theoryPoints.Length)
            {
                var diff = theoryPoints.Except(res);

                var sb = new StringBuilder();
                sb.Append("If you want use BESTFIT delete duplicate points:");
                foreach (var point3D in diff)
                {
                    sb.Append(System.Environment.NewLine).Append(point3D);
                }

                MessageBox.Show(sb.ToString());
            }


            CadDrawPoints.Draw(theoryPoints, theoryLayerName, Color.FromColor(System.Drawing.Color.YellowGreen));
            CadDrawPoints.Draw(theoryPoints, theoryLayerName, Color.FromColor(System.Drawing.Color.YellowGreen));
        }

        private const string theoryLayerName = "TheoryPoints";
    }

    public class MyPoint3DComparer : IEqualityComparer<MyPoint3D>
    {
        public bool Equals(MyPoint3D x, MyPoint3D y)
        {
            return x?.HasTheSameCoordsAs(y) ?? false;
        }

        public int GetHashCode(MyPoint3D obj)
        {
            return Convert.ToInt32(obj.X + obj.Y + obj.Z);
        }
    }
}