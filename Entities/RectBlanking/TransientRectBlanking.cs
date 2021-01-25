

namespace GeoLib.Entities.RectBlanking
{
    using ZwSoft.ZwCAD.DatabaseServices;
    using ZwSoft.ZwCAD.EditorInput;
    using ZwSoft.ZwCAD.Geometry;
    using GeoLib.Transient;
    using System;

    public class TransientRectBlanking : TransientUpdater
    {
        private Point3d start = Point3d.Origin;
        private Polyline3d polyline;

        public TransientRectBlanking(Point3d start)
        {
            this.start = start;
            this.polyline = CreatePolyline(start, start);
        }

        private static Polyline3d CreatePolyline(Point3d start, Point3d end)
        {
            Vector3d vectorTo = start.GetVectorTo(end);
            return new Polyline3d(Poly3dType.SimplePoly, new Point3dCollection { 
                start,
                start + (vectorTo.Y * Vector3d.YAxis),
                end,
                start + (vectorTo.X * Vector3d.XAxis)
            }, true);
        }

        public override Entity GetTransientEntity() => 
            this.polyline;

        public override void OnMonitorPoint(PointMonitorEventArgs e)
        {
            this.polyline = CreatePolyline(this.start, e.Context.ComputedPoint);
        }
    }
}

