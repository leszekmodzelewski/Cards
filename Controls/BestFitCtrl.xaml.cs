using GeoLib.ViewModels;
using System.Windows.Controls;

namespace GeoLib.Controls
{
    /// <summary>
    /// Interaction logic for BestFitCtrl.xaml
    /// </summary>
    public partial class BestFitCtrl : UserControl
    {
        public BestFitCtrl()
        {
            InitializeComponent();
        }

        public BestFitCtrl(BestFitViewModel bf) : this()
        {
            this.DataContext = bf;
        }
    }
}
