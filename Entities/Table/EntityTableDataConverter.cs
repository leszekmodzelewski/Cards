namespace GeoLib.Entities.Table
{
    using System;
    using ZwSoft.ZwCAD.DatabaseServices;

    public static class EntityTableDataConverter
    {
        public static EntityTableData ConvertFrom(TableWindowModel model)
        {
            if (model == null)
            {
                return null;
            }
            EntityTableData data1 = new EntityTableData();
            data1.Handle = model.FirstOrigin.Value;
            data1.X1 = model.FirstShipDrawing.X;
            data1.Y1 = model.FirstShipDrawing.Y;
            data1.Z1 = model.FirstShipDrawing.Z;
            data1.X2 = model.FirstShipMeasure.X;
            data1.Y2 = model.FirstShipMeasure.Y;
            data1.Z2 = model.FirstShipMeasure.Z;
            data1.UseSecond = model.UseSecond ? 1 : 0;
            EntityTableData local1 = data1;
            local1.A1 = model.SecondShipDrawing.X;
            local1.B1 = model.SecondShipDrawing.Y;
            local1.C1 = model.SecondShipDrawing.Z;
            local1.Distance = model.Distance;
            return local1;
        }

        public static TableWindowModel ConvertTo(EntityTableData data)
        {
            if (data == null)
            {
                return null;
            }
            TableWindowModel model1 = new TableWindowModel();
            model1.FirstOrigin = new Handle(data.Handle);
            model1.FirstShipDrawing.X = data.X1;
            model1.FirstShipDrawing.Y = data.Y1;
            model1.FirstShipDrawing.Z = data.Z1;
            model1.FirstShipMeasure.X = data.X2;
            model1.FirstShipMeasure.Y = data.Y2;
            model1.FirstShipMeasure.Z = data.Z2;
            model1.UseSecond = data.UseSecond != 0;
            model1.SecondShipDrawing.X = data.A1;
            model1.SecondShipDrawing.Y = data.B1;
            model1.SecondShipDrawing.Z = data.C1;
            model1.Distance = data.Distance;
            return model1;
        }
    }
}

