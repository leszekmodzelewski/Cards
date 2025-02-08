using GeoLib.Wpf;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using System.Windows.Input;

namespace GeoLib.ViewModels
{
    public class BoxWithTextViewModel : INotifyPropertyChanged
    {
        private int integerValue;

        public int IntegerValue
        {
            get => this.integerValue;
            set
            {
                this.integerValue = value;
                OnPropertyChanged();
            }
        }

        public DialogResult DialogResult
        {
            get;
            set;
        } = DialogResult.Cancel;



        public ICommand OkCommand => new SimpleCommand(Ok);
        private void Ok()
        {
            this.DialogResult = DialogResult.OK;
        }

        public ICommand CancelCommand => new SimpleCommand(Cancel);
        private void Cancel()
        {
            this.DialogResult = DialogResult.Cancel;
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
