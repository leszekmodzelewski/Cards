

namespace GeoLib.Entities.Origin.Create
{
    using GeoLib;
    using GeoLib.Entities.Origin;
    using System.Windows;
    using ZwSoft.ZwCAD.ApplicationServices;
    using ZwSoft.ZwCAD.DatabaseServices;
    using ZwSoft.ZwCAD.EditorInput;
    using ZwSoft.ZwCAD.Geometry;
    using ZwSoft.ZwCAD.Runtime;

    public class Cmd_CreateOrigin : Cmd_OriginBase
    {
        [CommandMethod("ORIGINCREATE", CommandFlags.UsePickSet)]
        public void CreateOrigin()
        {
            Document mdiActiveDocument = ZwSoft.ZwCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Editor editor = mdiActiveDocument.Editor;
            ObjectId blockTableRecord = AppUtils.EnsureOrigin(mdiActiveDocument.Database);
            if (!blockTableRecord.IsNull)
            {
                XDataUtils.EnsureApp(mdiActiveDocument.Database);
                OriginJig jig = new OriginJig(new BlockReference(Point3d.Origin, blockTableRecord));
                if (editor.Drag(jig).Status == PromptStatus.OK)
                {
                    OriginWindowModel model = new OriginWindowModel
                    {
                        Name = OriginUtils.GenerateUniqueOriginName(mdiActiveDocument.Database),
                        Ship = new Controls.DataXYZModel(null, null, null)
                    };
                    OriginWindowViewModel model2 = new OriginWindowViewModel(model);
                    ObjectId originId = CreateOriginObject(mdiActiveDocument.Database, jig.Position, blockTableRecord, model);
                    RegenerateOriginObject(mdiActiveDocument.Database, originId);
                    OriginWindow window1 = new OriginWindow();
                    window1.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                    window1.DataContext = model2;
                    bool? nullable = window1.ShowDialog();
                    if ((nullable == null) || !nullable.Value)
                    {
                        RemoveOriginObject(mdiActiveDocument.Database, originId);
                    }
                    else
                    {
                        UpdateOriginObject(mdiActiveDocument.Database, originId, model);
                        RegenerateOriginObject(mdiActiveDocument.Database, originId);
                    }
                }
            }
        }
    }
}

