

namespace GeoLib.Entities.Table
{
    using GeoLib.Transient;
    using ZwSoft.ZwCAD.DatabaseServices;
    using ZwSoft.ZwCAD.EditorInput;
    using ZwSoft.ZwCAD.Geometry;

    public class TransientTableDistance : TransientUpdater
    {
        private readonly BlockReference current;
        private readonly Point3d basePoint;

        public TransientTableDistance(BlockReference current, Point3d basePoint)
        {
            this.current = current;
            this.basePoint = basePoint;
        }

        public override Entity GetTransientEntity() =>
            this.current;

        public override void OnMonitorPoint(PointMonitorEventArgs e)
        {
            Point3d? position = null;
            TableUtils.SetTableParameters(this.current, position, new double?(e.Context.ComputedPoint.DistanceTo(this.basePoint)));
        }
    }
}

