namespace GeoLib.Entities.Origin
{
    using ZwSoft.ZwCAD.DatabaseServices;
    using GeoLib.Entities;
    using System;

    public class EntityOrigin : EntityBaseTyped<BlockReference, EntityOriginData>
    {
        public EntityOrigin(BlockReference blockReference, EntityOriginData data) : base(blockReference, data)
        {
        }

        public override void Regen()
        {
        }
    }
}

