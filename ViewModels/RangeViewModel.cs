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
        private int x;
        private int y;
        private int z;
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

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
