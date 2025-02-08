namespace GeoLib.Entities.Origin
{
    using GeoLib;
    using GeoLib.Controls;
    using GeoLib.Entities.Table;

    public class OriginWindowViewModel : ViewModel<OriginWindowModel>
    {
        private SectionTypeItem[] items;

        public OriginWindowViewModel(OriginWindowModel model) : base(model)
        {
            SectionTypeItem item1 = new SectionTypeItem();
            item1.SectionType = TableSectionTypes.FRAME;
            item1.Text = "Frame";
            SectionTypeItem[] itemArray1 = new SectionTypeItem[3];
            itemArray1[0] = item1;
            SectionTypeItem item2 = new SectionTypeItem();
            item2.SectionType = TableSectionTypes.TOP;
            item2.Text = "Top";
            itemArray1[1] = item2;
            SectionTypeItem item3 = new SectionTypeItem();
            item3.SectionType = TableSectionTypes.SIDE;
            item3.Text = "Side";
            itemArray1[2] = item3;
            this.items = itemArray1;
        }

        private SectionTypeItem GetItemFor(TableSectionTypes sectionType)
        {
            foreach (SectionTypeItem item in this.items)
            {
                if (item.SectionType == sectionType)
                {
                    return item;
                }
            }
            return this.items[0];
        }

        public string Name
        {
            get =>
                base.Model.Name;
            set
            {
                base.Model.Name = value;
                base.FirePropertyChanged("Name");
            }
        }

        public DataXYZViewModel2 Ship =>
            new DataXYZViewModel2(base.Model.Ship);

        public SectionTypeItem[] Items =>
            this.items;

        public SectionTypeItem SelectedItem
        {
            get =>
                this.GetItemFor(base.Model.SectionType);
            set
            {
                base.Model.SectionType = value.SectionType;
                base.FirePropertyChanged("SelectedItem");
            }
        }
    }
}

