namespace GeoLib.Entities.Origin
{
    using ZwSoft.ZwCAD.DatabaseServices;
    using GeoLib.Entities;
    using GeoLib.XData.Attributes;
    using System;
    using System.Runtime.CompilerServices;

    [XDataName("ORIGIN")]
    public class EntityOriginData : EntityData
    {
        public override EntityBase CreateFrom(Entity entity) => 
            new EntityOrigin((BlockReference) entity, this);

        public int SectionTypes { get; set; }

        public double X { get; set; }

        public double Y { get; set; }

        public double Z { get; set; }

        public string Name { get; set; }
    }
}

