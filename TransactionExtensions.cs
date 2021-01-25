using ZwSoft.ZwCAD.DatabaseServices;

namespace GeoLib
{
    using System;
    using System.Runtime.CompilerServices;

    public static class TransactionExtensions
    {
        public static T GetObject<T>(this Transaction transaction, ObjectId objectId, OpenMode openMode) where T: DBObject => 
            (transaction.GetObject(objectId, openMode) as T);
    }
}

