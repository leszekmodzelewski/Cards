

namespace GeoLib.Entities.Table
{
    using ZwSoft.ZwCAD.DatabaseServices;
    using ZwSoft.ZwCAD.EditorInput;
    using GeoLib.Transient;
    using System;

    public class TransientTableOrigin : TransientUpdater
    {
        private readonly BlockReference current;

        public TransientTableOrigin(BlockReference current)
        {
            this.current = current;
        }

        public override Entity GetTransientEntity() => 
            this.current;

        public override void OnMonitorPoint(PointMonitorEventArgs e)
        {
            this.current.Position = e.Context.ComputedPoint;
        }
    }
}

