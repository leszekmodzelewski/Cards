

namespace GeoLib.Entities
{
    using ZwSoft.ZwCAD.DatabaseServices;
    using System;
    using System.Globalization;

    public static class EntityBaseUtils
    {
        public static void RegenerateObject(Database database, ObjectId objectId)
        {
            using (Transaction transaction = database.TransactionManager.StartTransaction())
            {
                EntityBase base2 = EntityFactory.Create(transaction.GetObject(objectId, OpenMode.ForWrite));
                if (base2 != null)
                {
                    base2.Regen();
                }
                transaction.Commit();
            }
        }

        public static void UpdateNullableDoubleAttribute(AttributeReference attRef, double? value)
        {
            string text1;
            attRef.UpgradeOpen();
            if (value == null)
            {
                text1 = string.Empty;
            }
            else
            {
                object[] args = new object[] { value.Value };
                text1 = string.Format(CultureInfo.InvariantCulture, "{0:0}", args);
            }
            attRef.TextString = text1;
        }
    }
}

