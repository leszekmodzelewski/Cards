using System.Linq;

namespace GeoLib.Entities.Origin
{
    using GeoLib.Entities;
    using System.Collections.Generic;
    using ZwSoft.ZwCAD.DatabaseServices;

    public class OriginUtils
    {
        public static string GenerateUniqueOriginName(Database database)
        {
            HashSet<string> set = new HashSet<string>(from name in GetOriginItems(database) select name.Data.Name);
            int num = 1;
            string item = "Name" + num.ToString();
            while (set.Contains(item))
            {
                num++;
                item = "Name" + num.ToString();
            }
            return item;
        }

        public static EntityOrigin GetEntityOrigin(Database database, Handle originHandle)
        {
            ObjectId id;
            Transaction topTransaction = database.TransactionManager.TopTransaction;
            return ((!database.TryGetObjectId(originHandle, out id) || (!id.IsValid || id.IsErased)) ? null : (EntityFactory.Create(topTransaction.GetObject(id, OpenMode.ForRead)) as EntityOrigin));
        }

        public static EntityOrigin GetEntityOrigin(Database database, ObjectId originId) =>
            GetEntityOrigin(database, originId.Handle);

        public static OriginItem[] GetOriginItems(Database database)
        {
            List<OriginItem> list = new List<OriginItem>();
            using (Transaction transaction = database.TransactionManager.StartTransaction())
            {
                foreach (ObjectId id2 in transaction.GetObject(SymbolUtilityServices.GetBlockModelSpaceId(database), OpenMode.ForRead) as BlockTableRecord)
                {
                    EntityOrigin origin = EntityFactory.Create(transaction.GetObject(id2, OpenMode.ForRead)) as EntityOrigin;
                    if (origin != null)
                    {
                        OriginItem item = new OriginItem
                        {
                            ObjectId = origin.Entity.ObjectId,
                            Position = origin.Entity.Position,
                            Data = origin.Data
                        };
                        list.Add(item);
                    }
                }
            }
            return list.ToArray();
        }

        public static void RegenerateOriginContent(Database database, EntityOrigin entityOrigin)
        {
            Transaction topTransaction = database.TransactionManager.TopTransaction;
            foreach (ObjectId id in entityOrigin.Entity.AttributeCollection)
            {
                AttributeReference attRef = (AttributeReference)topTransaction.GetObject(id, OpenMode.ForRead);
                if (attRef.Tag == "BASE")
                {
                    UpdateStringAttribute(attRef, entityOrigin.Data.Name);
                }
            }
        }

        private static void UpdateStringAttribute(AttributeReference attRef, string value)
        {
            attRef.UpgradeOpen();
            attRef.TextString = value;
        }
        /*
        [Serializable, CompilerGenerated]
        private sealed class <>c
        {
            public static readonly OriginUtils.<>c <>9 = new OriginUtils.<>c();
            public static Func<OriginItem, string> <>9__1_0;

            internal string <GenerateUniqueOriginName>b__1_0(OriginItem name) => 
                name.Data.Name;
        }
        */
    }
}

