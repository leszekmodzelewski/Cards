namespace GeoLib.Controls
{
    public class DataXYZModel
    {
        private double? x;
        private double? y;
        private double? z;

        public DataXYZModel(double? x, double? y, double? z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        public static DataXYZModel Merge(DataXYZModel a, DataXYZModel b)
        {
            double? nullable5;
            double? nullable6;
            double? z;
            if (a == null)
            {
                return b;
            }
            if (b == null)
            {
                return a;
            }
            double? x = a.X;
            double? y = b.X;
            if ((x.GetValueOrDefault() == y.GetValueOrDefault()) ? ((x != null) == (y != null)) : false)
            {
                nullable5 = a.X;
            }
            else
            {
                y = null;
                nullable5 = y;
            }
            y = a.Y;
            x = b.Y;
            if ((y.GetValueOrDefault() == x.GetValueOrDefault()) ? ((y != null) == (x != null)) : false)
            {
                nullable6 = a.Y;
            }
            else
            {
                x = null;
                nullable6 = x;
            }
            double? nullable = nullable6;
            x = a.Z;
            y = b.Z;
            if ((x.GetValueOrDefault() == y.GetValueOrDefault()) ? ((x != null) == (y != null)) : false)
            {
                z = a.Z;
            }
            else
            {
                y = null;
                z = y;
            }
            return new DataXYZModel(nullable5, nullable, z);
        }

        public double? X
        {
            get =>
                this.x;
            set { this.x = value; }
        }

        public double? Y
        {
            get =>
                this.y;
            set { this.y = value; }
        }

        public double? Z
        {
            get =>
                this.z;
            set { this.z = value; }
        }
    }
}

