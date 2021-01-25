using ZwSoft.ZwCAD.DatabaseServices;
using System;

namespace GeoLib.Entities
{
	public abstract class EntityBaseTyped<TEntity, TData> : EntityBase
	where TEntity : Entity
	where TData : EntityData
	{
		public new TData Data
		{
			get
			{
				return (TData)this.data;
			}
		}

		public new TEntity Entity
		{
			get
			{
				return (TEntity)this.entity;
			}
		}

		public EntityBaseTyped(Entity entity, TData data) : base(entity, data)
		{
		}
	}
}