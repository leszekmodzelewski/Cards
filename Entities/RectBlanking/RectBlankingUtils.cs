

namespace GeoLib.Entities.RectBlanking
{
    using ZwSoft.ZwCAD.DatabaseServices;
    using ZwSoft.ZwCAD.Geometry;
    using System;

    public static class RectBlankingUtils
    {
        public static ObjectId CreateBlockReference(Database database, ObjectId blockId, Point3d start, double width, double height)
        {
            ObjectId @null = ObjectId.Null;
            using (Transaction transaction = database.TransactionManager.StartTransaction())
            {
                BlockReference entity = new BlockReference(start, blockId);
                @null = (transaction.GetObject(SymbolUtilityServices.GetBlockModelSpaceId(database), OpenMode.ForWrite) as BlockTableRecord).AppendEntity(entity);
                transaction.AddNewlyCreatedDBObject(entity, true);
                SetRectBlankingParameters(entity, new double?(width), new double?(height));
                transaction.Commit();
            }
            return @null;
        }

        public static void SetRectBlankingParameters(BlockReference blockReference, double? width, double? height)
        {
            foreach (DynamicBlockReferenceProperty property in blockReference.DynamicBlockReferencePropertyCollection)
            {
                if ((property.PropertyName == "Distance1") && (width != null))
                {
                    property.Value = width.Value;
                }
                if ((property.PropertyName == "Distance2") && (height != null))
                {
                    property.Value = height.Value;
                }
            }
        }
    }
}

