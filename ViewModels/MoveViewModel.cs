using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace GeoLib.ViewModels
{
    public class MoveViewModel : INotifyPropertyChanged
    {
        private int xOffset;
        private int yOffset;
        private int zOffset;

        public MoveViewModel(int[] offset)
        {
            this.XOffset = offset[0];
            this.YOffset = offset[1];
            this.ZOffset = offset[2];
        }

        public int XOffset
        {
            get => xOffset;
            set
            {
                xOffset = value;
                OnPropertyChanged();
            }
        }

        public int YOffset
        {
            get => yOffset;
            set
            {
                yOffset = value;
                OnPropertyChanged();
            }
        }

        public int ZOffset
        {
            get => zOffset;
            set
            {
                zOffset = value;
                OnPropertyChanged();
            }
        }

        public int[] Offset
        {
            get
            {
                return new[] {XOffset, YOffset, ZOffset};
            }

        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
