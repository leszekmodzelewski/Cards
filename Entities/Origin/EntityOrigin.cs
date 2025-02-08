namespace GeoLib.Entities.Origin
{
    using GeoLib.Entities;
    using ZwSoft.ZwCAD.DatabaseServices;

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

