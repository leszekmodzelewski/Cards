

namespace GeoLib.Entities.TableRef
{
    using ZwSoft.ZwCAD.DatabaseServices;
    using GeoLib.Entities;
    using GeoLib.Entities.Table;
    using System;

    public class EntityTableRef : EntityBaseTyped<BlockReference, EntityTableRefData>
    {
        public EntityTableRef(BlockReference blockReference, EntityTableRefData data) : base(blockReference, data)
        {
        }

        public override void Regen()
        {
            Database database = base.entity.Database;
            Transaction topTransaction = database.TransactionManager.TopTransaction;
            EntityTable entityTable = TableUtils.GetEntityTable(database, new Handle(base.Data.Handle));
            if (entityTable != null)
            {
                double? nullable;
                double? nullable2;
                double? nullable3;
                TableUtils.RecalculateTableContent(database, entityTable, out nullable, out nullable2, out nullable3);
                foreach (ObjectId id in base.Entity.AttributeCollection)
                {
                    AttributeReference attRef = (AttributeReference) topTransaction.GetObject(id, OpenMode.ForRead);
                    if (attRef.Tag == "X_1")
                    {
                        EntityBaseUtils.UpdateNullableDoubleAttribute(attRef, nullable);
                    }
                    if (attRef.Tag == "Y_1")
                    {
                        EntityBaseUtils.UpdateNullableDoubleAttribute(attRef, nullable2);
                    }
                    if (attRef.Tag == "Z_1")
                    {
                        EntityBaseUtils.UpdateNullableDoubleAttribute(attRef, nullable3);
                    }
                    if (attRef.Tag == "X_2")
                    {
                        EntityBaseUtils.UpdateNullableDoubleAttribute(attRef, entityTable.Data.X2);
                    }
                    if (attRef.Tag == "Y_2")
                    {
                        EntityBaseUtils.UpdateNullableDoubleAttribute(attRef, entityTable.Data.Y2);
                    }
                    if (attRef.Tag == "Z_2")
                    {
                        EntityBaseUtils.UpdateNullableDoubleAttribute(attRef, entityTable.Data.Z2);
                    }
                }
            }
        }
    }
}

