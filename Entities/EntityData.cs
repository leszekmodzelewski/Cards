

namespace GeoLib.Entities
{
    using ZwSoft.ZwCAD.DatabaseServices;

    public abstract class EntityData
    {
        protected EntityData()
        {
        }

        public abstract EntityBase CreateFrom(Entity entity);
    }
}

