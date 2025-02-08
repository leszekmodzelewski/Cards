namespace GeoLib.Entities.Table
{
    using GeoLib;
    using GeoLib.Entities;
    using GeoLib.Entities.Origin;
    using GeoLib.XData;
    using System.Windows;
    using ZwSoft.ZwCAD.ApplicationServices;
    using ZwSoft.ZwCAD.DatabaseServices;
    using ZwSoft.ZwCAD.EditorInput;
    using ZwSoft.ZwCAD.Geometry;
    using ZwSoft.ZwCAD.Runtime;

    public class Cmd_CreateTable : Cmd_TableBase
    {
        [CommandMethod("TABLECREATE", CommandFlags.UsePickSet)]
        public void CreateEmptyTable()
        {
            Point3d originPosition;
            Transaction transaction;
            Document mdiActiveDocument = ZwSoft.ZwCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Editor editor = mdiActiveDocument.Editor;
            TableResources resources = AppUtils.EnsureTableBlocks(mdiActiveDocument.Database);
            if (AppUtils.HasAllTableDefinitions(resources))
            {
                XDataApp configuration = XDataUtils.EnsureXDataConfiguration(mdiActiveDocument.Database);
                double defaultTableSize = configuration.DefaultTableSize;
                BlockReference entity = new BlockReference(Point3d.Origin, resources.Frame);
                entity.ScaleFactors = new Scale3d(defaultTableSize / 150.0);
                TableJig jig = new TableJig(entity);
                if (editor.Drag(jig).Status == PromptStatus.OK)
                {
                    originPosition = jig.Position;
                    BlockReference blockReference = null;
                    using (transaction = mdiActiveDocument.Database.TransactionManager.StartTransaction())
                    {
                        PromptPointResult point;
                        Point3d pointd;
                        Point3d? nullable3;
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
                                    goto TR_001A;
                                }
                                else
                                {
                                    selector.ResetSelected();
                                }
                            }
                            return;
                        TR_001A:
                            blockReference = selector.Selected;
                            goto TR_0019;
                        }
                        return;
                    TR_0014:
                        nullable3 = null;
                        TableUtils.SetTableParameters(blockReference, nullable3, new double?(defaultTableSize));
                        blockReference.XData = XDataSerializer.Serialize(new EntityTableData());
                        blockReference.Visible = true;
                        transaction.Commit();
                        configuration.DefaultTableSize = defaultTableSize;
                        XDataUtils.SetXDataConfiguration(mdiActiveDocument.Database, configuration);
                        TableWindowModel model = new TableWindowModel();
                        TableWindowViewModel model2 = new TableWindowViewModel(OriginUtils.GetOriginItems(mdiActiveDocument.Database), originPosition, model);
                        TableWindow window1 = new TableWindow();
                        window1.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                        window1.DataContext = model2;
                        bool? nullable = window1.ShowDialog();
                        if ((nullable == null) || !nullable.Value)
                        {
                            RemoveTableObject(mdiActiveDocument.Database, blockReference.ObjectId);
                        }
                        else
                        {
                            UpdateTableObject(mdiActiveDocument.Database, blockReference.ObjectId, model);
                            EntityBaseUtils.RegenerateObject(mdiActiveDocument.Database, blockReference.ObjectId);
                            return;
                        }
                        return;
                    TR_0019:
                        pointd = point.Value;
                        using (new TransientTableDistance(blockReference, pointd))
                        {
                            PromptDistanceOptions options = new PromptDistanceOptions("Specify size")
                            {
                                BasePoint = pointd,
                                UseBasePoint = true,
                                DefaultValue = defaultTableSize
                            };
                            //var scale = editor.GetInteger("Podaj skale");
                           PromptDoubleResult distance = editor.GetDistance(options);
                            if (distance.Status == PromptStatus.OK)
                            {
                                defaultTableSize = distance.Value;
                                goto TR_0014;
                            }
                        }
                    }
                }
            }
        }
    }
}

