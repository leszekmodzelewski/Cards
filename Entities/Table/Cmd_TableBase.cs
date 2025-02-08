

namespace GeoLib.Entities.Table
{
    using GeoLib.Entities;
    using GeoLib.XData;
    using ZwSoft.ZwCAD.DatabaseServices;
    using ZwSoft.ZwCAD.Geometry;

    public class Cmd_TableBase
    {
        protected static ObjectId CreateTableObject(Database database, Point3d position, ObjectId originBlock, TableWindowModel model)
        {
            ObjectId @null = ObjectId.Null;
            using (Transaction transaction = database.TransactionManager.StartTransaction())
            {
                BlockReference entity = new BlockReference(position, originBlock);
                @null = (transaction.GetObject(SymbolUtilityServices.GetBlockModelSpaceId(database), OpenMode.ForWrite) as BlockTableRecord).AppendEntity(entity);
                transaction.AddNewlyCreatedDBObject(entity, true);
                entity.XData = XDataSerializer.Serialize(EntityTableDataConverter.ConvertFrom(model));
                EntityTable table = EntityFactory.Create(entity) as EntityTable;
                if (table != null)
                {
                    table.Regen();
                }
                transaction.Commit();
            }
            return @null;
        }

        protected static void RegenerateTableObject(Database database, ObjectId tableId)
        {
            using (Transaction transaction = database.TransactionManager.StartTransaction())
            {
                EntityTable table = EntityFactory.Create(transaction.GetObject(tableId, OpenMode.ForWrite)) as EntityTable;
                if (table != null)
                {
                    table.Regen();
                }
                transaction.Commit();
            }
        }

        protected static void RemoveTableObject(Database database, ObjectId originId)
        {
            using (Transaction transaction = database.TransactionManager.StartTransaction())
            {
                transaction.GetObject(originId, OpenMode.ForWrite).Erase();
                transaction.Commit();
            }
        }

        protected static void UpdateTableObject(Database database, ObjectId tableId, TableWindowModel model)
        {
            using (Transaction transaction = database.TransactionManager.StartTransaction())
            {
                transaction.GetObject(tableId, OpenMode.ForWrite).XData = XDataSerializer.Serialize(EntityTableDataConverter.ConvertFrom(model));
                transaction.Commit();
            }
        }
    }
}

