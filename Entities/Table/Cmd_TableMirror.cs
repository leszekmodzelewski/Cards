

namespace GeoLib.Entities.Table
{
    using ZwSoft.ZwCAD.ApplicationServices;
    using ZwSoft.ZwCAD.DatabaseServices;
    using ZwSoft.ZwCAD.EditorInput;
    using ZwSoft.ZwCAD.Geometry;
    using ZwSoft.ZwCAD.Runtime;
    using GeoLib;
    using GeoLib.Controls;
    using System;

    public class Cmd_TableMirror
    {
        [CommandMethod("TABLEMIRROR", CommandFlags.UsePickSet)]
        public void RecalculateSectionTableX()
        {
            Document mdiActiveDocument = ZwSoft.ZwCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Editor editor = mdiActiveDocument.Editor;
            PromptSelectionResult selection = editor.SelectImplied();
            if (selection.Value == null)
            {
                selection = editor.GetSelection();
            }
            if ((selection.Value != null) && (selection.Value.Count > 0))
            {
                PromptPointResult point = editor.GetPoint("Specify mirror point");
                if (point.Status == PromptStatus.OK)
                {
                    PromptPointOptions options1 = new PromptPointOptions("Specify mirror direction");
                    options1.BasePoint = point.Value;
                    options1.UseBasePoint = true;
                    PromptPointResult result3 = editor.GetPoint("Specify mirror direction");
                    if (result3.Status == PromptStatus.OK)
                    {
                        TableResources resources = AppUtils.EnsureTableBlocks(mdiActiveDocument.Database);
                        Vector3d vectorTo = point.Value.GetVectorTo(result3.Value);
                        bool flag = Math.Abs(vectorTo.X) > Math.Abs(vectorTo.Y);
                        using (Transaction transaction = mdiActiveDocument.Database.TransactionManager.StartTransaction())
                        {
                            BlockTableRecord modelspace = transaction.GetObject(SymbolUtilityServices.GetBlockModelSpaceId(mdiActiveDocument.Database), OpenMode.ForWrite) as BlockTableRecord;
                            ObjectId[] objectIds = selection.Value.GetObjectIds();
                            int index = 0;
                            while (true)
                            {
                                if (index >= objectIds.Length)
                                {
                                    transaction.Commit();
                                    break;
                                }
                                ObjectId id = objectIds[index];
                                BlockReference blockReference = transaction.GetObject(id, OpenMode.ForRead) as BlockReference;
                                if (blockReference != null)
                                {
                                    Point3d? nullable;
                                    double? nullable2;
                                    Vector3d vectord2 = (Vector3d) (2.0 * blockReference.Position.GetVectorTo(flag ? new Point3d(point.Value.X, blockReference.Position.Y, blockReference.Position.Z) : new Point3d(blockReference.Position.X, point.Value.Y, blockReference.Position.Z)));
                                    ObjectId blockId = flag ? AppUtils.GetXMirror(resources, blockReference.DynamicBlockTableRecord) : AppUtils.GetYMirror(resources, blockReference.DynamicBlockTableRecord);
                                    BlockReference reference2 = TableUtils.CreateBlockReference(transaction, modelspace, blockReference.Position + vectord2, blockId, true);
                                    TableUtils.GetTableParameters(blockReference, out nullable, out nullable2);
                                    if (nullable != null)
                                    {
                                        nullable = new Point3d?(flag ? new Point3d(-nullable.Value.X, nullable.Value.Y, nullable.Value.Z) : new Point3d(nullable.Value.X, -nullable.Value.Y, nullable.Value.Z));
                                    }
                                    TableUtils.SetTableParameters(reference2, nullable, nullable2);
                                    DataXYZModel xZY = TableUtils.GetXZY(transaction, blockReference);
                                    if (xZY.Y != null)
                                    {
                                        xZY.Y = new double?(-xZY.Y.Value);
                                    }
                                    TableUtils.SetXYZ(transaction, reference2, xZY);
                                }
                                index++;
                            }
                        }
                    }
                }
            }
        }
    }
}

