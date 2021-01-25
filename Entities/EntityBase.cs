namespace GeoLib.Entities
{
    using ZwSoft.ZwCAD.DatabaseServices;
    using System;

    public abstract class EntityBase
    {
        protected readonly ZwSoft.ZwCAD.DatabaseServices.Entity entity;
        protected readonly EntityData data;

        public EntityBase(ZwSoft.ZwCAD.DatabaseServices.Entity entity, EntityData data)
        {
            this.entity = entity;
            this.data = data;
        }

        public EntityBase Clone() => 
            EntityFactory.Create(this.entity.Clone() as ZwSoft.ZwCAD.DatabaseServices.Entity);

        public abstract void Regen();

        public ZwSoft.ZwCAD.DatabaseServices.Entity Entity =>
            this.entity;

        public EntityData Data =>
            this.data;
    }
}

