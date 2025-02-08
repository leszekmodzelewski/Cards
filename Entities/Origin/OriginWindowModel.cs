namespace GeoLib.Entities.Origin
{
    using GeoLib.Controls;
    using GeoLib.Entities.Table;

    public class OriginWindowModel
    {
        public string Name { get; set; }

        public DataXYZModel Ship { get; set; }

        public TableSectionTypes SectionType { get; set; }
    }
}

