using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace GeoLib.ViewModels
{
    class ValueOffsetViewModel : INotifyPropertyChanged
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }

        public int OffsetX { get; set; }
        public int OffsetY { get; set; }
        public int OffsetZ { get; set; }


        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
