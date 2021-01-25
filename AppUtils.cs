

namespace GeoLib
{
    using ZwSoft.ZwCAD.ApplicationServices;
    using ZwSoft.ZwCAD.DatabaseServices;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    public static class AppUtils
    {
        private const string TABLE_TL = "TableTL";
        private const string TABLE_TR = "TableTR";
        private const string TABLE_BL = "TableBL";
        private const string TABLE_BR = "TableBR";
        private const string TABLE_FRAME = "TableFrame";
        private static readonly string[] TABLES_ALL = new string[] { "TableTR", "TableTL", "TableBR", "TableBL", "TableFrame" };
        private const string ORIGIN = "Origin";
        private const string RECT_BLANKING = "RectBlanking";

        private static void CopyResources(Database database, string[] blockNames)
        {
            using (Database database2 = new Database(false, true))
            {
                string path = Path.Combine(GetPluginFolder(), "DrawingLib.dwg");
                if (!File.Exists(path))
                {
                    throw new FileNotFoundException("Missing library drawing file");
                }
                database2.ReadDwgFile(path, FileShare.Read, true, string.Empty);
                Dictionary<string, ObjectId> blocks = GetBlocks(database2, blockNames);
                if (!HasAllDefinitions(blocks))
                {
                    throw new FileLoadException("Unable to find requested resources.");
                }
                IdMapping mapping = new IdMapping();
                database2.WblockCloneObjects(new ObjectIdCollection(blocks.Values.ToArray<ObjectId>()), database.BlockTableId, mapping, DuplicateRecordCloning.Replace, false);
            }
        }

        public static Dictionary<string, ObjectId> EnsureBlocks(Database database, string[] blockNames)
        {
            Dictionary<string, ObjectId> blocks = GetBlocks(database, blockNames);
            if (HasAllDefinitions(blocks))
            {
                return blocks;
            }
            CopyResources(database, blockNames);
            return GetBlocks(database, blockNames);
        }

        public static ObjectId EnsureOrigin(Database database)
        {
            string[] blockNames = new string[] { "Origin" };
            return EnsureBlocks(database, blockNames)["Origin"];
        }

        public static ObjectId EnsureRectBlanking(Database database)
        {
            string[] blockNames = new string[] { "RectBlanking" };
            return EnsureBlocks(database, blockNames)["RectBlanking"];
        }

        public static TableResources EnsureTableBlocks(Database database)
        {
            Dictionary<string, ObjectId> dictionary = EnsureBlocks(database, TABLES_ALL);
            return new TableResources { 
                TopLeft = dictionary["TableTL"],
                TopRight = dictionary["TableTR"],
                BottomLeft = dictionary["TableBL"],
                BottomRight = dictionary["TableBR"],
                Frame = dictionary["TableFrame"]
            };
        }

        public static void EnsureTransientRegen(Document document)
        {
            ObjectId @null = ObjectId.Null;
            using (Transaction transaction = document.Database.TransactionManager.StartTransaction())
            {
                Line entity = new Line();
                (transaction.GetObject(SymbolUtilityServices.GetBlockModelSpaceId(document.Database), OpenMode.ForWrite) as BlockTableRecord).AppendEntity(entity);
                transaction.AddNewlyCreatedDBObject(entity, true);
                @null = entity.Id;
                transaction.Commit();
            }
            using (Transaction transaction2 = document.Database.TransactionManager.StartTransaction())
            {
                transaction2.GetObject(@null, OpenMode.ForWrite).Erase();
                transaction2.Commit();
            }
        }

        private static Dictionary<string, ObjectId> GetBlocks(Database database, string[] blockNames)
        {
            Dictionary<string, ObjectId> dictionary = new Dictionary<string, ObjectId>(blockNames.Length);
            foreach (string str in blockNames)
            {
                dictionary.Add(str, ObjectId.Null);
            }
            using (Transaction transaction = database.TransactionManager.StartTransaction())
            {
                foreach (ObjectId id in transaction.GetObject(database.BlockTableId, OpenMode.ForRead) as BlockTable)
                {
                    BlockTableRecord record = transaction.GetObject(id, OpenMode.ForRead) as BlockTableRecord;
                    if (dictionary.ContainsKey(record.Name))
                    {
                        dictionary[record.Name] = id;
                    }
                }
            }
            return dictionary;
        }

        private static string GetPluginFolder() => 
            Path.GetDirectoryName(Assembly.GetAssembly(typeof(AppUtils)).Location);

        public static ObjectId GetXMirror(TableResources resources, ObjectId id) => 
            (!(id == resources.BottomLeft) ? (!(id == resources.BottomRight) ? (!(id == resources.TopLeft) ? (!(id == resources.TopRight) ? ObjectId.Null : resources.TopLeft) : resources.TopRight) : resources.BottomLeft) : resources.BottomRight);

        public static ObjectId GetYMirror(TableResources resources, ObjectId id) => 
            (!(id == resources.BottomLeft) ? (!(id == resources.TopLeft) ? (!(id == resources.BottomRight) ? (!(id == resources.TopRight) ? ObjectId.Null : resources.BottomRight) : resources.TopRight) : resources.BottomLeft) : resources.TopLeft);

        public static bool HasAllDefinitions(Dictionary<string, ObjectId> map)
        {
            bool flag = true;
            foreach (KeyValuePair<string, ObjectId> pair in map)
            {
                flag &= pair.Value != ObjectId.Null;
            }
            return flag;
        }

        public static bool HasAllTableDefinitions(TableResources resources) => 
            (((((true | (resources.BottomLeft != ObjectId.Null)) | (resources.BottomRight != ObjectId.Null)) | (resources.TopLeft != ObjectId.Null)) | (resources.TopRight != ObjectId.Null)) | (resources.Frame != ObjectId.Null));
    }
}

