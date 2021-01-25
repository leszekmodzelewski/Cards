namespace GeoLib.Entities.Origin
{
    using GeoLib.Entities.Table;
    using System;

    public static class EntityOriginDataConverter
    {
        public static EntityOriginData ConvertFrom(OriginWindowModel model)
        {
            if (model == null)
            {
                return null;
            }
            EntityOriginData data = new EntityOriginData {
                Name = model.Name,
                SectionTypes = (int) model.SectionType
            };
            if (model.Ship.X != null)
            {
                data.X = model.Ship.X.Value;
            }
            if (model.Ship.Y != null)
            {
                data.Y = model.Ship.Y.Value;
            }
            if (model.Ship.Z != null)
            {
                data.Z = model.Ship.Z.Value;
            }
            return data;
        }

        public static OriginWindowModel ConvertTo(EntityOriginData data)
        {
            if (data == null)
            {
                return null;
            }
            OriginWindowModel model1 = new OriginWindowModel();
            model1.Name = data.Name;
            model1.SectionType = (TableSectionTypes) data.SectionTypes;
            model1.Ship.X = new double?(data.X);
            model1.Ship.Y = new double?(data.Y);
            model1.Ship.Z = new double?(data.Z);
            return model1;
        }
    }
}

