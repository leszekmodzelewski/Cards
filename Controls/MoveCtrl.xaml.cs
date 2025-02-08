using GeoLib.ViewModels;
using System.Windows.Controls;

namespace GeoLib.Controls
{
    /// <summary>
    /// Interaction logic for MoveCtrl.xaml
    /// </summary>
    public partial class MoveCtrl : UserControl
    {
        public MoveCtrl(MoveViewModel moveViewModel)
        {
            InitializeComponent();
            this.DataContext = moveViewModel;
        }
    }
}
