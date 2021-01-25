

namespace GeoLib.Entities
{
    using ZwSoft.ZwCAD.DatabaseServices;
    using GeoLib.XData;
    using System;

    public class EntityFactory
    {
        public static EntityBase Create(DBObject dbObject)
        {
            EntityData data;
            Entity entity = dbObject as Entity;
            if (entity == null)
            {
                return null;
            }
            ResultBuffer xDataForApplication = dbObject.GetXDataForApplication("GEOLIB");
            return ((xDataForApplication != null) ? (((data = XDataSerializer.Deserialize(xDataForApplication) as EntityData) == null) ? null : data.CreateFrom(entity)) : null);
        }
    }
}

