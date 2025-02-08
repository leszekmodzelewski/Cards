using GeoLib.ViewModels;
using Microsoft.Win32;
using System.Windows;
using System.Windows.Controls;

namespace GeoLib.Controls
{
    /// <summary>
    /// Interaction logic for FitCtrl.xaml
    /// </summary>
    public partial class FitCtrl : UserControl
    {
        public FitCtrl()
        {
            InitializeComponent();
        }

        private FitViewModel vm;

        public FitCtrl(FitViewModel vm) : this()
        {
            this.vm = vm;
            this.DataContext = vm;
        }


        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                vm.FilePath = openFileDialog.FileName;
                vm.ReadFromFileCommand.Execute(null);
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
