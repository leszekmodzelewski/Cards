

namespace GeoLib.Entities.Origin
{
    using GeoLib.Entities;
    using GeoLib.XData;
    using ZwSoft.ZwCAD.DatabaseServices;
    using ZwSoft.ZwCAD.Geometry;

    public class Cmd_OriginBase
    {
        protected static ObjectId CreateOriginObject(Database database, Point3d position, ObjectId blockId, OriginWindowModel model)
        {
            ObjectId @null = ObjectId.Null;
            using (Transaction transaction = database.TransactionManager.StartTransaction())
            {
                BlockReference entity = new BlockReference(position, blockId);
                @null = (transaction.GetObject(SymbolUtilityServices.GetBlockModelSpaceId(database), OpenMode.ForWrite) as BlockTableRecord).AppendEntity(entity);
                transaction.AddNewlyCreatedDBObject(entity, true);
                foreach (ObjectId id3 in transaction.GetObject(blockId, OpenMode.ForWrite) as BlockTableRecord)
                {
                    AttributeDefinition definition = transaction.GetObject(id3, OpenMode.ForRead) as AttributeDefinition;
                    if (definition != null)
                    {
                        using (AttributeReference reference2 = new AttributeReference())
                        {
                            reference2.SetAttributeFromBlock(definition, entity.BlockTransform);
                            reference2.TextString = definition.TextString;
                            entity.AttributeCollection.AppendAttribute(reference2);
                            transaction.AddNewlyCreatedDBObject(reference2, true);
                        }
                    }
                }
                entity.XData = XDataSerializer.Serialize(EntityOriginDataConverter.ConvertFrom(model));
                transaction.Commit();
            }
            return @null;
        }

        protected static void RegenerateOriginObject(Database database, ObjectId originId)
        {
            using (Transaction transaction = database.TransactionManager.StartTransaction())
            {
                EntityOrigin entityOrigin = EntityFactory.Create(transaction.GetObject(originId, OpenMode.ForWrite)) as EntityOrigin;
                if (entityOrigin != null)
                {
                    OriginUtils.RegenerateOriginContent(database, entityOrigin);
                }
                transaction.Commit();
            }
        }

        protected static void RemoveOriginObject(Database database, ObjectId originId)
        {
            using (Transaction transaction = database.TransactionManager.StartTransaction())
            {
                transaction.GetObject(originId, OpenMode.ForWrite).Erase();
                transaction.Commit();
            }
        }

        protected static void UpdateOriginObject(Database database, ObjectId originId, OriginWindowModel model)
        {
            using (Transaction transaction = database.TransactionManager.StartTransaction())
            {
                transaction.GetObject(originId, OpenMode.ForWrite).XData = XDataSerializer.Serialize(EntityOriginDataConverter.ConvertFrom(model));
                transaction.Commit();
            }
        }
    }
}

