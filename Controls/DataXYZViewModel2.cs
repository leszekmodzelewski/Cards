namespace GeoLib.Controls
{
    using GeoLib;

    public class DataXYZViewModel2 : ViewModel<DataXYZModel>
    {
        public DataXYZViewModel2(DataXYZModel model) : base(model)
        {
        }

        public string X
        {
            get =>
                ParsingUtils.FormatNullableDouble(() => base.Model.X);
            set
            {
                ParsingUtils.ParseNullableDouble2(value, v => base.Model.X = v);
                base.FirePropertyChanged("X");
            }
        }

        public string Y
        {
            get =>
                ParsingUtils.FormatNullableDouble(() => base.Model.Y);
            set
            {
                ParsingUtils.ParseNullableDouble2(value, v => base.Model.Y = v);
                base.FirePropertyChanged("Y");
            }
        }

        public string Z
        {
            get =>
                ParsingUtils.FormatNullableDouble(() => base.Model.Z);
            set
            {
                ParsingUtils.ParseNullableDouble2(value, v => base.Model.Z = v);
                base.FirePropertyChanged("Z");
            }
        }
    }
}

