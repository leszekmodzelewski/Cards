

using ZwSoft.ZwCAD.Geometry;

namespace GeoLib.Transient
{
    using System;
    using ZwSoft.ZwCAD.DatabaseServices;
    using ZwSoft.ZwCAD.EditorInput;
    using ZwSoft.ZwCAD.GraphicsInterface;

    public abstract class TransientUpdater : IDisposable
    {
        private Entity transientEntity;

        public TransientUpdater()
        {
            ZwSoft.ZwCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor.PointMonitor += new PointMonitorEventHandler(this.Editor_PointMonitor);
        }

        private void CreateTransient()
        {
            TransientManager.CurrentTransientManager.AddTransient(this.transientEntity, TransientDrawingMode.DirectShortTerm, 0x80, new IntegerCollection());
        }

        public void Dispose()
        {
            if (this.transientEntity != null)
            {
                this.EraseTransient();
            }
            ZwSoft.ZwCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor.PointMonitor -= new PointMonitorEventHandler(this.Editor_PointMonitor);
        }

        private void Editor_PointMonitor(object sender, PointMonitorEventArgs e)
        {
            if (this.transientEntity == null)
            {
                this.transientEntity = this.GetTransientEntity();
                this.CreateTransient();
            }
            this.OnMonitorPoint(e);
            Entity transientEntity = this.GetTransientEntity();
            if (transientEntity != this.transientEntity)
            {
                this.EraseTransient();
                this.transientEntity = transientEntity;
                this.CreateTransient();
            }
            this.UpdateTransient();
        }

        private void EraseTransient()
        {
            TransientManager.CurrentTransientManager.EraseTransient(this.transientEntity, new IntegerCollection());
        }

        public abstract Entity GetTransientEntity();
        public abstract void OnMonitorPoint(PointMonitorEventArgs e);
        private void UpdateTransient()
        {
            TransientManager.CurrentTransientManager.UpdateTransient(this.transientEntity, new IntegerCollection());
        }
    }
}

