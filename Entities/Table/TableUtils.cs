using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Documents;
using PointCalc;


namespace GeoLib.Entities.Table
{
    using ZwSoft.ZwCAD.DatabaseServices;
    using ZwSoft.ZwCAD.Geometry;
    using GeoLib.Controls;
    using GeoLib.Entities;
    using GeoLib.Entities.Origin;
    using System;
    using System.Globalization;
    using System.Runtime.InteropServices;

    public static class TableUtils
    {
        public const string POSITION_X = "Position1 X";
        public const string POSITION_Y = "Position1 Y";
        public const string DISTANCE = "Distance1";
        public const double MIN_DISTANCE = 10.0;

        public static BlockReference CreateBlockReference(Transaction transaction, BlockTableRecord modelspace, Point3d position, ObjectId blockId, bool visible)
        {
            BlockReference entity = new BlockReference(position, blockId) {
                Visible = visible
            };
            modelspace.AppendEntity(entity);
            transaction.AddNewlyCreatedDBObject(entity, true);
            foreach (ObjectId id in transaction.GetObject(blockId, OpenMode.ForWrite) as BlockTableRecord)
            {
                AttributeDefinition definition = transaction.GetObject(id, OpenMode.ForRead) as AttributeDefinition;
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
            return entity;
        }

        public static EntityTable GetEntityTable(Database database, Handle originHandle)
        {
            ObjectId id;
            Transaction topTransaction = database.TransactionManager.TopTransaction;
            if (database.TryGetObjectId(originHandle, out id))
            {
                if((!id.IsValid || id.IsErased))
                {
                    return null;
                }
                var en = EntityFactory.Create(topTransaction.GetObject(id, OpenMode.ForRead));
                return en as EntityTable;
            }

            return null;
            //return ((!database.TryGetObjectId(originHandle, out id) || (!id.IsValid || id.IsErased)) ? null : (EntityFactory.Create(topTransaction.GetObject(id, OpenMode.ForRead)) as EntityTable));
        }

        public static EntityTable GetEntityTable(Database database, ObjectId objectId) => 
            GetEntityTable(database, objectId.Handle);

        private static double GetT(double a, double b, double distance)
        {
            double num = b - a;
            if (num < 1.0)
            {
                num = 1.0;
            }
            return ((distance >= 1E-06) ? ((distance <= (num - 1E-06)) ? (distance / num) : 1.0) : 0.0);
        }

        public static void GetTableParameters(BlockReference blockReference, out Point3d? position, out double? distance)
        {
            double? nullable = null;
            double? nullable2 = null;
            position = null;
            distance = 0;
            foreach (DynamicBlockReferenceProperty property in blockReference.DynamicBlockReferencePropertyCollection)
            {
                if (property.PropertyName == "Position1 X")
                {
                    nullable = new double?((double) property.Value);
                }
                if (property.PropertyName == "Position1 Y")
                {
                    nullable2 = new double?((double) property.Value);
                }
                if (property.PropertyName == "Distance1")
                {
                    distance = new double?((double) property.Value);
                }
            }
            if ((nullable != null) && (nullable2 != null))
            {
                position = new Point3d(nullable.Value, nullable2.Value, 0.0);
            }
        }

        public static DataXYZModel GetXZY(Transaction transaction, BlockReference blockReference)
        {
            double? x = null;
            double? y = null;
            double? z = null;
            foreach (ObjectId id in blockReference.AttributeCollection)
            {
                double num;
                AttributeReference reference = (AttributeReference) transaction.GetObject(id, OpenMode.ForRead);
                if ((reference.Tag == "X_1") && double.TryParse(reference.TextString, NumberStyles.Any, CultureInfo.InvariantCulture, out num))
                {
                    x = new double?(num);
                }
                if ((reference.Tag == "Y_1") && double.TryParse(reference.TextString, NumberStyles.Any, CultureInfo.InvariantCulture, out num))
                {
                    y = new double?(num);
                }
                if ((reference.Tag == "Z_1") && double.TryParse(reference.TextString, NumberStyles.Any, CultureInfo.InvariantCulture, out num))
                {
                    z = new double?(num);
                }
            }
            return new DataXYZModel(x, y, z);
        }

        private static double Lerp(double a, double b, double t) => 
            ((a * (1.0 - t)) + (b * t));

        public static void RecalculateTableContent(Database database, EntityTable entityTable, out double? x, out double? y, out double? z)
        {
            double? nullable = entityTable.Data.X1;
            double? nullable2 = entityTable.Data.Y1;
            double? nullable3 = entityTable.Data.Z1;
            Transaction topTransaction = database.TransactionManager.TopTransaction;
            Handle originHandle = new Handle(entityTable.Data.Handle);
            EntityOrigin entityOrigin = OriginUtils.GetEntityOrigin(database, originHandle);
            if (entityOrigin != null)
            {
                TableSectionTypes sectionTypes = (TableSectionTypes) entityOrigin.Data.SectionTypes;
                Point3d pointd = Transform(entityOrigin.Entity.Position, sectionTypes, entityTable.Entity.Position) + new Vector3d(entityOrigin.Data.X, entityOrigin.Data.Y, entityOrigin.Data.Z);
                nullable = new double?(pointd.X);
                nullable2 = new double?(pointd.Y);
                nullable3 = new double?(pointd.Z);
            }
            x = nullable;
            y = nullable2;
            z = nullable3;
            if (entityTable.Data.UseSecond != 0)
            {
                double? nullable4 = entityTable.Data.A1;
                double? nullable5 = entityTable.Data.B1;
                double? nullable6 = entityTable.Data.C1;
                if ((nullable != null) && ((nullable2 != null) && ((nullable3 != null) && ((nullable4 != null) && ((nullable5 != null) && (nullable6 != null))))))
                {
                    double t = GetT(nullable.Value, nullable4.Value, entityTable.Data.Distance);
                    x = new double?(Lerp(nullable.Value, nullable4.Value, t));
                    y = new double?(Lerp(nullable2.Value, nullable5.Value, t));
                    z = new double?(Lerp(nullable3.Value, nullable6.Value, t));
                }
            }
        }

        public static void RegenerateAllTables(Database database)
        {
            
            using (Transaction transaction = database.TransactionManager.StartTransaction())
            {
                foreach (ObjectId id2 in transaction.GetObject(SymbolUtilityServices.GetBlockModelSpaceId(database), OpenMode.ForWrite) as BlockTableRecord)
                {
                    EntityBase base2 = EntityFactory.Create(transaction.GetObject(id2, OpenMode.ForRead));
                    if (base2 != null)
                    {
                        base2.Regen();
                    }
                }
                transaction.Commit();
            }
        }

        //private static string GetXYZ(Database database, EntityTable eTable)
        //{
        //    string res = string.Empty;

        //    Transaction topTransaction = database.TransactionManager.TopTransaction;
        //    TableUtils.RecalculateTableContent(database, eTable, out double? nullable, out double? nullable2, out double? nullable3);
        //    foreach (ObjectId id in eTable.Entity.AttributeCollection)
        //    {
        //        AttributeReference attRef = (AttributeReference)topTransaction.GetObject(id, OpenMode.ForWrite);
        //        //if (attRef.Tag == "X_1")
        //        //{
        //        //    res += attRef.TextString;
        //        //}
        //        //if (attRef.Tag == "Y_1")
        //        //{
        //        //    res += attRef.TextString;
        //        //}
        //        //if (attRef.Tag == "Z_1")
        //        //{
        //        //    res += attRef.TextString;
        //        //}


        //        if (attRef.Tag == "X_2")
        //        {
        //            attRef.TextString = "x2";
        //        }
        //        if (attRef.Tag == "Y_2")
        //        {
        //            attRef.TextString = "y2";
        //        }
        //        if (attRef.Tag == "Z_2")
        //        {
        //            attRef.TextString = "z2";
        //        }
        //        if (attRef.Tag == "X_3")
        //        {
        //            attRef.TextString = "x3";
        //        }
        //        if (attRef.Tag == "Y_3")
        //        {
        //            attRef.TextString = "y3";
        //        }
        //        if (attRef.Tag == "Z_3")
        //        {
        //            attRef.TextString = "z3";
        //        }
        //        if (attRef.Tag == "X_4")
        //        {
        //            attRef.TextString = "x4";
        //        }
        //        if (attRef.Tag == "Y_4")
        //        {
        //            attRef.TextString = "y4";
        //        }
        //        if (attRef.Tag == "Z_4")
        //        {
        //            attRef.TextString = "z4";
        //        }
        //    }

        //    return res;
        //}

        public static void SetTableParameters(BlockReference blockReference, Point3d? position, double? distance)
        {
            foreach (DynamicBlockReferenceProperty property in blockReference.DynamicBlockReferencePropertyCollection)
            {
                if ((property.PropertyName == "Position1 X") && (position != null))
                {
                    property.Value = position.Value.X;
                }
                if ((property.PropertyName == "Position1 Y") && (position != null))
                {
                    property.Value = position.Value.Y;
                }
                if ((property.PropertyName == "Distance1") && (distance != null))
                {
                    property.Value = Math.Max(10.0, distance.Value);
                }
            }
        }

        public static void SetXYZ(Transaction transaction, BlockReference blockReference, DataXYZModel data)
        {
            foreach (ObjectId id in blockReference.AttributeCollection)
            {
                AttributeReference reference = (AttributeReference) transaction.GetObject(id, OpenMode.ForRead);
                if ((reference.Tag == "X_1") && (data.X != null))
                {
                    reference.UpgradeOpen();
                    object[] args = new object[] { data.X.Value };
                    reference.TextString = string.Format(CultureInfo.InvariantCulture, "{0:0}", args);
                }
                if ((reference.Tag == "Y_1") && (data.Y != null))
                {
                    reference.UpgradeOpen();
                    object[] args = new object[] { data.Y.Value };
                    reference.TextString = string.Format(CultureInfo.InvariantCulture, "{0:0}", args);
                }
                if ((reference.Tag == "Z_1") && (data.Z != null))
                {
                    reference.UpgradeOpen();
                    object[] args = new object[] { data.Z.Value };
                    reference.TextString = string.Format(CultureInfo.InvariantCulture, "{0:0}", args);
                }
            }
        }

        public static Point3d Transform(Point3d originPosition, TableSectionTypes sectionType, Point3d tablePosition)
        {
            if (sectionType == TableSectionTypes.TOP)
            {
                return new Point3d(tablePosition.X - originPosition.X, (tablePosition.Y - originPosition.Y), tablePosition.Z - originPosition.Z);
            }
            if (sectionType != TableSectionTypes.FRAME)
            {
                return ((sectionType != TableSectionTypes.SIDE) ? Point3d.Origin : new Point3d(tablePosition.X - originPosition.X, tablePosition.Z - originPosition.Z, tablePosition.Y - originPosition.Y));
            }
            return new Point3d((tablePosition.Z - originPosition.Z), originPosition.X - tablePosition.X , tablePosition.Y - originPosition.Y);
        }

        private static void UpdateNullableDoubleAttribute(AttributeReference attRef, double? value)
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

