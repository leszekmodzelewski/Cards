using GeoLib.ViewModels;
using System.Windows.Controls;

namespace GeoLib.Controls
{
    /// <summary>
    /// Interaction logic for RealPointsRow.xaml
    /// </summary>
    public partial class RealPointsRow : UserControl
    {
        public RealPointsRow()
        {
            InitializeComponent();
        }

        public RealPointsRow(RealPointsRowViewModel vm) : this()
        {
            this.DataContext = vm;
        }
    }
}
