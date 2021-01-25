namespace GeoLib.Entities.Table
{
    using ZwSoft.ZwCAD.DatabaseServices;
    using ZwSoft.ZwCAD.EditorInput;
    using ZwSoft.ZwCAD.Geometry;
    using GeoLib.Transient;
    using System;

    public class TransientTablePosition : TransientUpdater
    {
        private readonly BlockReferenceSelector selector;
        private double defaultSize;

        public TransientTablePosition(BlockReferenceSelector selector, double defaultSize)
        {
            this.selector = selector;
            this.defaultSize = defaultSize;
        }

        public override Entity GetTransientEntity() => 
            this.selector.Selected;

        public override void OnMonitorPoint(PointMonitorEventArgs e)
        {
            Point3d position = e.Context.ComputedPoint.TransformBy(this.selector.Selected.BlockTransform.Inverse());
            this.selector.Select(position);
            TableUtils.SetTableParameters(this.selector.Selected, new Point3d?(position), new double?(this.defaultSize));
        }
    }
}

