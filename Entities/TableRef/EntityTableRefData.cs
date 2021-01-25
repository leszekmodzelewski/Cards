namespace GeoLib.Entities.TableRef
{
    using ZwSoft.ZwCAD.DatabaseServices;
    using GeoLib.Entities;
    using GeoLib.XData.Attributes;
    using System;
    using System.Runtime.CompilerServices;

    [XDataName("TABLEREF")]
    public class EntityTableRefData : EntityData
    {
        public override EntityBase CreateFrom(Entity entity) => 
            new EntityTableRef((BlockReference) entity, this);

        public long Handle { get; set; }
    }
}

