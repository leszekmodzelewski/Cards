namespace GeoLib.Entities.Table
{
    using ZwSoft.ZwCAD.DatabaseServices;
    using GeoLib.Entities;
    using GeoLib.XData.Attributes;
    using System;
    using System.Runtime.CompilerServices;

    [XDataName("TABLE")]
    public class EntityTableData : EntityData
    {
        public override EntityBase CreateFrom(Entity entity) => 
            new EntityTable((BlockReference) entity, this);

        public long Handle { get; set; }

        public double? X1 { get; set; }

        public double? Y1 { get; set; }

        public double? Z1 { get; set; }

        public double? X2 { get; set; }

        public double? Y2 { get; set; }

        public double? Z2 { get; set; }

        public int UseSecond { get; set; }

        public double? A1 { get; set; }

        public double? B1 { get; set; }

        public double? C1 { get; set; }

        public double Distance { get; set; }
    }
}

