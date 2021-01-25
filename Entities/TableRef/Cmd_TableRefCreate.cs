

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

    public class Cmd_TableRefCreate
    {
        [CommandMethod("TABLEREFCREATE", CommandFlags.UsePickSet)]
        public void CreateEmptyTable()
        {
            Document mdiActiveDocument = ZwSoft.ZwCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Database database = mdiActiveDocument.Database;
            Editor editor = mdiActiveDocument.Editor;
            TableResources resources = AppUtils.EnsureTableBlocks(mdiActiveDocument.Database);
            if (AppUtils.HasAllTableDefinitions(resources))
            {
                ObjectId @null = ObjectId.Null;
                EntityTableData data = null;
                PromptSelectionResult result = editor.SelectImplied();
                if ((result.Value != null) && (result.Value.Count == 1))
                {
                    @null = result.Value[0].ObjectId;
                    data = TryGetEntityTableData(database, @null);
                }
                while (true)
                {
                    if (data != null)
                    {
                        XDataApp configuration = XDataUtils.EnsureXDataConfiguration(mdiActiveDocument.Database);
                        double defaultTableSize = configuration.DefaultTableSize;
                        BlockReference reference1 = new BlockReference(Point3d.Origin, resources.Frame);
                        reference1.ScaleFactors = new Scale3d(defaultTableSize / 150.0);
                        TableJig jig = new TableJig(reference1);
                        if (editor.Drag(jig).Status != PromptStatus.OK)
                        {
                            return;
                        }
                        Point3d originPosition = jig.Position;
                        BlockReference blockReference = null;
                        using (Transaction transaction = mdiActiveDocument.Database.TransactionManager.StartTransaction())
                        {
                            PromptPointResult point;
                            Point3d pointd;
                            Point3d? nullable2;
                            ObjectId blockModelSpaceId = SymbolUtilityServices.GetBlockModelSpaceId(mdiActiveDocument.Database);
                            BlockTableRecord modelspace = transaction.GetObject(blockModelSpaceId, OpenMode.ForWrite) as BlockTableRecord;
                            using (BlockReferenceSelector selector = new BlockReferenceSelector(resources, blockId => TableUtils.CreateBlockReference(transaction, modelspace, originPosition, blockId, false)))
                            {
                                using (new TransientTablePosition(selector, defaultTableSize))
                                {
                                    point = editor.GetPoint("Specify point");
                                    if (point.Status == PromptStatus.OK)
                                    {
                                        Point3d pointd2 = point.Value.TransformBy(selector.Selected.BlockTransform.Inverse());
                                        double? distance = null;
                                        TableUtils.SetTableParameters(selector.Selected, new Point3d?(pointd2), distance);
                                        goto TR_0017;
                                    }
                                    else
                                    {
                                        selector.ResetSelected();
                                    }
                                }
                                break;
                            TR_0017:
                                blockReference = selector.Selected;
                                goto TR_0016;
                            }
                            break;
                        TR_0011:
                            nullable2 = null;
                            TableUtils.SetTableParameters(blockReference, nullable2, new double?(defaultTableSize));
                            EntityTableRefData data2 = new EntityTableRefData {
                                Handle = @null.Handle.Value
                            };
                            blockReference.XData = XDataSerializer.Serialize(data2);
                            blockReference.Visible = true;
                            transaction.Commit();
                            EntityBaseUtils.RegenerateObject(mdiActiveDocument.Database, blockReference.ObjectId);
                            configuration.DefaultTableSize = defaultTableSize;
                            XDataUtils.SetXDataConfiguration(mdiActiveDocument.Database, configuration);
                            break;
                        TR_0016:
                            pointd = point.Value;
                            using (new TransientTableDistance(blockReference, pointd))
                            {
                                PromptDistanceOptions options = new PromptDistanceOptions("Specify size") {
                                    BasePoint = pointd,
                                    UseBasePoint = true,
                                    DefaultValue = defaultTableSize
                                };
                                PromptDoubleResult distance = editor.GetDistance(options);
                                if (distance.Status == PromptStatus.OK)
                                {
                                    defaultTableSize = distance.Value;
                                    goto TR_0011;
                                }
                            }
                            break;
                        }
                        break;
                    }
                    PromptEntityResult entity = editor.GetEntity("Specify table entity to copy properties from");
                    if (entity.Status != PromptStatus.OK)
                    {
                        return;
                    }
                    data = TryGetEntityTableData(database, entity.ObjectId);
                }
            }
        }

        private static EntityTableData TryGetEntityTableData(Database database, ObjectId objectId)
        {
            using (database.TransactionManager.StartTransaction())
            {
                EntityTable entityTable = TableUtils.GetEntityTable(database, objectId);
                if (entityTable != null)
                {
                    return entityTable.Data;
                }
            }
            return null;
        }
    }
}

