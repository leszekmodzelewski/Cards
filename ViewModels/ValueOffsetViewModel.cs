using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace GeoLib.ViewModels
{
    public class ValueOffsetViewModel : INotifyPropertyChanged
    {
        private int x;
        private int y;
        private int z;

        private int ox;
        private int oy;
        private int oz;

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

        public int OffsetX
        {
            get => ox;
            set
            {
                ox = value;
                OnPropertyChanged();
            }
        }
        public int OffsetY
        {
            get => oy;
            set
            {
                oy = value;
                OnPropertyChanged();
            }
        }
        public int OffsetZ
        {
            get => oz;
            set
            {
                oz = value;
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
