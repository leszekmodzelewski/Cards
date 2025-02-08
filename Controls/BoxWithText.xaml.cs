using GeoLib.ViewModels;
using System.Windows.Controls;

namespace GeoLib.Controls
{
    /// <summary>
    /// Interaction logic for BoxWithText.xaml
    /// </summary>
    public partial class BoxWithText : UserControl
    {
        public BoxWithText()
        {
            InitializeComponent();
        }

        public BoxWithText(BoxWithTextViewModel bf) : this()
        {
            this.DataContext = bf;
        }
    }
}
