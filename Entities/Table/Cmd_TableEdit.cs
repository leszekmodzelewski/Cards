

namespace GeoLib.Entities.Table
{
    using GeoLib.Entities;
    using GeoLib.Entities.Origin;
    using System.Windows;
    using ZwSoft.ZwCAD.ApplicationServices;
    using ZwSoft.ZwCAD.DatabaseServices;
    using ZwSoft.ZwCAD.EditorInput;
    using ZwSoft.ZwCAD.Geometry;
    using ZwSoft.ZwCAD.Runtime;

    public class Cmd_TableEdit : Cmd_TableBase
    {
        [CommandMethod("TABLEEDIT", CommandFlags.UsePickSet)]
        public void RecalculateSectionTableX()
        {
            Document mdiActiveDocument = ZwSoft.ZwCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Editor editor = mdiActiveDocument.Editor;
            Point3d origin = Point3d.Origin;
            TableWindowModel model = null;
            ObjectId @null = ObjectId.Null;
            PromptSelectionResult result = editor.SelectImplied();
            if ((result.Value != null) && (result.Value.Count == 1))
            {
                @null = result.Value[0].ObjectId;
                model = TryGetModel(mdiActiveDocument.Database, @null, out origin);
            }
            while (model == null)
            {
                PromptEntityResult entity = editor.GetEntity("Specify table entity");
                if (entity.Status != PromptStatus.OK)
                {
                    return;
                }
                @null = entity.ObjectId;
                model = TryGetModel(mdiActiveDocument.Database, @null, out origin);
            }
            TableWindowViewModel model2 = new TableWindowViewModel(OriginUtils.GetOriginItems(mdiActiveDocument.Database), origin, model);
            TableWindow window1 = new TableWindow();
            window1.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            window1.DataContext = model2;
            bool? nullable = window1.ShowDialog();
            if ((nullable != null) && nullable.Value)
            {
                UpdateTableObject(mdiActiveDocument.Database, @null, model);
                EntityBaseUtils.RegenerateObject(mdiActiveDocument.Database, @null);
            }
        }

        private static TableWindowModel TryGetModel(Database database, ObjectId id, out Point3d position)
        {
            using (database.TransactionManager.StartTransaction())
            {
                EntityTable entityTable = TableUtils.GetEntityTable(database, id);
                if (entityTable != null)
                {
                    position = entityTable.Entity.Position;
                    return EntityTableDataConverter.ConvertTo(entityTable.Data);
                }
            }
            position = Point3d.Origin;
            return null;
        }
    }
}

