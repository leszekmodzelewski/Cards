

namespace GeoLib.Entities.Table
{
    using ZwSoft.ZwCAD.DatabaseServices;
    using ZwSoft.ZwCAD.EditorInput;
    using ZwSoft.ZwCAD.Geometry;

    public class TableJig : EntityJig
    {
        private Point3d samplePosition;

        public TableJig(BlockReference entity) : base(entity)
        {
        }

        protected override SamplerStatus Sampler(JigPrompts prompts)
        {
            PromptPointResult result = prompts.AcquirePoint("Specify point");
            if (result.Status != PromptStatus.OK)
            {
                return SamplerStatus.Cancel;
            }
            if (result.Value.DistanceTo(this.samplePosition) <= double.Epsilon)
            {
                return SamplerStatus.NoChange;
            }
            this.samplePosition = result.Value;
            return SamplerStatus.OK;
        }

        protected override bool Update()
        {
            (base.Entity as BlockReference).Position = this.samplePosition;
            return true;
        }

        public Point3d Position =>
            this.samplePosition;

        public BlockReference BlockRef =>
            (base.Entity as BlockReference);
    }
}

