namespace GeoLib.Entities.TableRef
{
    using GeoLib.Entities;
    using GeoLib.XData.Attributes;
    using ZwSoft.ZwCAD.DatabaseServices;

    [XDataName("TABLEREF")]
    public class EntityTableRefData : EntityData
    {
        public override EntityBase CreateFrom(Entity entity) =>
            new EntityTableRef((BlockReference)entity, this);

        public long Handle { get; set; }
    }
}

