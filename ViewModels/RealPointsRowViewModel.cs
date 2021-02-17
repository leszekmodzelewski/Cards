using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GeoLib.Wpf;

namespace GeoLib.ViewModels
{
    public class RealPointsRowViewModel : INotifyPropertyChanged
    {
        private int x;
        private int y;
        private int z;
        private int dx;
        private int dy;
        private int dz;
        private string id = string.Empty;

        public string Id
        {
            get => id;
            set
            {
                id = value;
                OnPropertyChanged();
            }
        }


        public int X
        {
            get => x;
            set
            {
                x = value;
                OnPropertyChanged();
            }
        }
        public int Y
        {
            get => y;
            set
            {
                y = value;
                OnPropertyChanged();
            }
        }
        public int Z
        {
            get => z;
            set
            {
                z = value;
                OnPropertyChanged();
            }
        }

        public int Dx
        {
            get => dx;
            set
            {
                dx = value;
                OnPropertyChanged();
            }
        }
        public int Dy
        {
            get => dy;
            set
            {
                dy = value;
                OnPropertyChanged();
            }
        }
        public int Dz
        {
            get => dz;
            set
            {
                dz = value;
                OnPropertyChanged();
            }
        }

        public ICommand DxExecuteCommand => new SimpleCommand(DxExecute);

        private void DxExecute()
        {
            this.Dx += 1;
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
