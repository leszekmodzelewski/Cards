using System.Windows;

namespace GeoLib.Entities.Origin
{
    public partial class OriginWindow : Window
    {
        public OriginWindow()
        {
            this.InitializeComponent();
        }

        private void OKClick(object sender, RoutedEventArgs e)
        {
            base.DialogResult = new bool?(true);
        }
    }
}