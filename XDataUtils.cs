

namespace GeoLib
{
    using GeoLib.XData;
    using ZwSoft.ZwCAD.DatabaseServices;

    public static class XDataUtils
    {
        public const string APP_NAME = "GEOLIB";

        public static ObjectId EnsureApp(Database database)
        {
            ObjectId @null = ObjectId.Null;
            using (Transaction transaction = database.TransactionManager.StartTransaction())
            {
                RegAppTable table = transaction.GetObject<RegAppTable>(database.RegAppTableId, OpenMode.ForRead);
                if (!table.Has("GEOLIB"))
                {
                    table.UpgradeOpen();
                    RegAppTableRecord record = new RegAppTableRecord
                    {
                        Name = "GEOLIB"
                    };
                    @null = table.Add(record);
                    transaction.AddNewlyCreatedDBObject(record, true);
                    transaction.Commit();
                }
                else
                {
                    return table["GEOLIB"];
                }
            }
            XDataApp configuration = new XDataApp();
            SetXDataConfiguration(database, configuration);
            return @null;
        }

        public static XDataApp EnsureXDataConfiguration(Database database)
        {
            ObjectId objectId = EnsureApp(database);
            using (Transaction transaction = database.TransactionManager.StartTransaction())
            {
                return (XDataSerializer.Deserialize(transaction.GetObject<RegAppTableRecord>(objectId, OpenMode.ForRead).XData) as XDataApp);
            }
        }

        public static void SetXDataConfiguration(Database database, XDataApp configuration)
        {
            ObjectId objectId = EnsureApp(database);
            using (Transaction transaction = database.TransactionManager.StartTransaction())
            {
                transaction.GetObject<RegAppTableRecord>(objectId, OpenMode.ForWrite).XData = XDataSerializer.Serialize(configuration);
                transaction.Commit();
            }
        }
    }
}

