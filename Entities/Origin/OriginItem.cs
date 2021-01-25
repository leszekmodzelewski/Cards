

namespace GeoLib.Entities.Origin
{
    using ZwSoft.ZwCAD.Geometry;
    using System;
    using System.Runtime.CompilerServices;

    public class OriginItem
    {
        public ZwSoft.ZwCAD.DatabaseServices.ObjectId ObjectId { get; set; }

        public Point3d Position { get; set; }

        public EntityOriginData Data { get; set; }

        public bool AutoUpdate { get; set; }
    }
}

