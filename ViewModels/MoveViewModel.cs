using GeoLib.Wpf;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

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
                return new[] { XOffset, YOffset, ZOffset };
            }

        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event EventHandler<EventArgs> Apply;

        public event EventHandler<EventArgs> Cancel;


        public ICommand ExecuteApplyCommand => new SimpleCommand(ApplyClicked);
        private void ApplyClicked()
        {
            Apply?.Invoke(this, EventArgs.Empty);
        }

        public ICommand ExecuteCancelCommand => new SimpleCommand(CancelClicked);
        private void CancelClicked()
        {
            Cancel?.Invoke(this, EventArgs.Empty);
        }
    }
}
