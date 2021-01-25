

namespace GeoLib.Entities.Origin.Edit
{
    using ZwSoft.ZwCAD.ApplicationServices;
    using ZwSoft.ZwCAD.DatabaseServices;
    using ZwSoft.ZwCAD.EditorInput;
    using ZwSoft.ZwCAD.Runtime;
    using GeoLib;
    using GeoLib.Entities.Origin;
    using GeoLib.Entities.Table;
    using System;
    using System.Windows;

    public class Cmd_OriginEdit : Cmd_OriginBase
    {
        [CommandMethod("ORIGINEDIT", CommandFlags.UsePickSet)]
        public void EditOrigin()
        {
            Document mdiActiveDocument = ZwSoft.ZwCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Editor editor = mdiActiveDocument.Editor;
            if (!AppUtils.EnsureOrigin(mdiActiveDocument.Database).IsNull)
            {
                XDataUtils.EnsureApp(mdiActiveDocument.Database);
                OriginWindowModel model = null;
                ObjectId @null = ObjectId.Null;
                PromptSelectionResult result = editor.SelectImplied();
                if ((result.Value != null) && (result.Value.Count == 1))
                {
                    @null = result.Value[0].ObjectId;
                    model = TryGetModel(mdiActiveDocument.Database, @null);
                }
                while (model == null)
                {
                    PromptEntityResult entity = editor.GetEntity("Specify origin entity");
                    if (entity.Status != PromptStatus.OK)
                    {
                        return;
                    }
                    @null = entity.ObjectId;
                    model = TryGetModel(mdiActiveDocument.Database, @null);
                }
                OriginWindowViewModel model2 = new OriginWindowViewModel(model);
                OriginWindow window1 = new OriginWindow();
                window1.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                window1.DataContext = model2;
                bool? nullable = window1.ShowDialog();
                if ((nullable != null) && nullable.Value)
                {
                    UpdateOriginObject(mdiActiveDocument.Database, @null, model);
                    RegenerateOriginObject(mdiActiveDocument.Database, @null);
                    TableUtils.RegenerateAllTables(mdiActiveDocument.Database);
                }
            }
        }

        private static OriginWindowModel TryGetModel(Database database, ObjectId id)
        {
            using (database.TransactionManager.StartTransaction())
            {
                EntityOrigin entityOrigin = OriginUtils.GetEntityOrigin(database, id);
                if (entityOrigin != null)
                {
                    return EntityOriginDataConverter.ConvertTo(entityOrigin.Data);
                }
            }
            return null;
        }
    }
}

