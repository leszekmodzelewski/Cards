using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using GeoLib.ViewModels;
using Microsoft.Win32;

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
    }
}
