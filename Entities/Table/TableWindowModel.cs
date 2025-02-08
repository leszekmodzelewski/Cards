namespace GeoLib.Entities.Table
{
    using GeoLib.Controls;
    using ZwSoft.ZwCAD.DatabaseServices;

    public class TableWindowModel
    {
        public TableWindowModel()
        {
            double? x = null;
            x = null;
            x = null;
            firstShipDrawing = new DataXYZModel(x, x, x);
            x = null;
            x = null;
            x = null;
            this.firstShipMeasure = new DataXYZModel(x, x, x);
            x = null;
            x = null;
            x = null;
            this.secondShipDrawing = new DataXYZModel(x, x, x);
        }

        public Handle FirstOrigin { get; set; }

        private readonly DataXYZModel firstShipDrawing;
        private readonly DataXYZModel firstShipMeasure;
        private readonly DataXYZModel secondShipDrawing;
        public DataXYZModel FirstShipDrawing
        {
            get => firstShipDrawing;
        }

        public DataXYZModel FirstShipMeasure { get => firstShipMeasure; }

        public bool UseSecond { get; set; }

        public DataXYZModel SecondShipDrawing { get => secondShipDrawing; }

        public double Distance { get; set; }
    }
}

