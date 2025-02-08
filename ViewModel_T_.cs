using System.ComponentModel;

namespace GeoLib
{
    public class ViewModel<T> : INotifyPropertyChanged
    {
        private readonly T model;

        public T Model
        {
            get
            {
                return this.model;
            }
        }

        public ViewModel(T model)
        {
            this.model = model;
        }

        public void FirePropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler propertyChangedEventHandler = this.PropertyChanged;
            if (propertyChangedEventHandler == null)
            {
                return;
            }
            propertyChangedEventHandler(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}