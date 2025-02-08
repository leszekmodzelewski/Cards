

namespace GeoLib.Entities.Table
{
    using GeoLib;
    using System;
    using ZwSoft.ZwCAD.DatabaseServices;
    using ZwSoft.ZwCAD.Geometry;

    public class BlockReferenceSelector : IDisposable
    {
        private BlockReference selected;
        private BlockReference brTL;
        private BlockReference brTR;
        private BlockReference brBL;
        private BlockReference brBR;

        public BlockReferenceSelector(TableResources tableResource, Func<ObjectId, BlockReference> blockReferenceBuilder, bool Tpoint = true)
        {
            if (Tpoint == true)
            {
                this.brTL = blockReferenceBuilder(tableResource.TopLeft);
                this.brTR = blockReferenceBuilder(tableResource.TopRight);
                this.brBL = blockReferenceBuilder(tableResource.BottomLeft);
                this.brBR = blockReferenceBuilder(tableResource.BottomRight);
                this.selected = this.Default;
            }
            this.selected = this.Default;
        }
       

        public void Dispose()
        {
            if (this.selected != this.brTL)
            {
                this.brTL.Erase();
            }
            if (this.selected != this.brTR)
            {
                this.brTR.Erase();
            }
            if (this.selected != this.brBL)
            {
                this.brBL.Erase();
            }
            if (this.selected != this.brBR)
            {
                this.brBR.Erase();
            }
        }

        public void ResetSelected()
        {
            this.selected = null;
        }

        public void Select(Point3d position)
        {
            bool flag = position.X < 0.0;
            bool flag1 = position.Y < 0.0;
            if (flag1 & flag)
            {
                this.selected = this.brBL;
            }
            bool local1 = flag1;
            if (local1 && !flag)
            {
                this.selected = this.brBR;
            }
            bool local2 = local1;
            if (!local2 & flag)
            {
                this.selected = this.brTL;
            }
            if (!local2 && !flag)
            {
                this.selected = this.brTR;
            }
        }

        public BlockReference Default =>
            this.brTR;

        public BlockReference Selected =>
            this.selected;
    }
}

