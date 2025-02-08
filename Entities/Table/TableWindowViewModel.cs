

namespace GeoLib.Entities.Table
{
    using GeoLib;
    using GeoLib.Controls;
    using GeoLib.Entities.Origin;
    using System.Collections.Generic;
    using ZwSoft.ZwCAD.DatabaseServices;
    using ZwSoft.ZwCAD.Geometry;

    public class TableWindowViewModel : ViewModel<TableWindowModel>
    {
        private readonly OriginItem[] originItems;
        private readonly OriginItem nullItem;
        private readonly Point3d originPosition;

        public TableWindowViewModel(OriginItem[] originItems, Point3d originPosition, TableWindowModel model) : base(model)
        {
            this.originPosition = originPosition;
            OriginItem item1 = new OriginItem();
            item1.ObjectId = ObjectId.Null;
            EntityOriginData data1 = new EntityOriginData();
            data1.Name = "[None]";
            data1.X = 0.0;
            data1.Y = 0.0;
            data1.Z = 0.0;
            item1.Data = data1;
            item1.AutoUpdate = false;
            this.nullItem = item1;
            List<OriginItem> list = new List<OriginItem> {
                this.nullItem
            };
            list.AddRange(originItems);
            this.originItems = list.ToArray();
        }

        private OriginItem FindItem(Handle handle)
        {
            foreach (OriginItem item in this.originItems)
            {
                ObjectId objectId = item.ObjectId;
                if (objectId.Handle == handle)
                {
                    return item;
                }
            }
            return this.nullItem;
        }

        public OriginItem FirstOrigin
        {
            get =>
                this.FindItem(base.Model.FirstOrigin);
            set
            {
                base.Model.FirstOrigin = value.ObjectId.Handle;
                base.FirePropertyChanged("FirstOrigin");
                base.FirePropertyChanged("AreCoordinatesEnabled");
            }
        }

        public OriginItem[] Items =>
            this.originItems;

        public DataXYZViewModel FirstShipDrawing =>
            new DataXYZViewModel(base.Model.FirstShipDrawing);

        public DataXYZViewModel FirstShipMeasure =>
            new DataXYZViewModel(base.Model.FirstShipMeasure);

        //// DKO: To remove
        //public bool UseSecond
        //{
        //    get => 
        //        base.Model.UseSecond;
        //    set
        //    {
        //        base.Model.UseSecond = value;
        //        base.FirePropertyChanged("UseSecond");
        //    }
        //}


        //// DKO: To remove
        //public DataXYZViewModel SecondShipDrawing =>
        //    new DataXYZViewModel(base.Model.SecondShipDrawing);


        //// DKO: To remove
        //public double Distance
        //{
        //    get => 
        //        base.Model.Distance;
        //    set
        //    {
        //        base.Model.Distance = value;
        //        base.FirePropertyChanged("Distance");
        //    }
        //}

        public bool AreCoordinatesEnabled =>
            ReferenceEquals(this.nullItem, this.FirstOrigin);

        public Point3d OriginPosition =>
            this.originPosition;
    }
}

