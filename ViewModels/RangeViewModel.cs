using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace GeoLib.ViewModels
{
    public class RangeViewModel : INotifyPropertyChanged
    {
        private string x1;
        private string y1;
        private string z1;
        private int x2;
        private int y2;
        private int z2;
        private string range = string.Empty;

        public string Range
        {
            get => range;
            set
            {
                range = value;
                OnPropertyChanged();
            }
        }

        public string X1
        {
            get => x1;
            set
            {
                x1 = value;
                OnPropertyChanged();
            }
        }
        public string Y1
        {
            get => y1;
            set
            {
                y1 = value;
                OnPropertyChanged();
            }
        }
        public string Z1
        {
            get => z1;
            set
            {
                z1 = value;
                OnPropertyChanged();
            }
        }

        public int X2
        {
            get => x2;
            set
            {
                x2 = value;
                OnPropertyChanged();
            }
        }
        public int Y2
        {
            get => y2;
            set
            {
                y2 = value;
                OnPropertyChanged();
            }
        }
        public int Z2
        {
            get => z2;
            set
            {
                z2 = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
