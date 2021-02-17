using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace GeoLib.ViewModels
{
    public class BestFitViewModel : INotifyPropertyChanged
    {
        public BestFitViewModel(IEnumerable<RealPointsRowViewModel> points)
        {
            Points = new ObservableCollection<RealPointsRowViewModel>(points);
        }

        private int maxFitValue;

        public int MaxFitValue
        {
            get => maxFitValue;
            set { maxFitValue = value; OnPropertyChanged(); }
        }

        public ObservableCollection<RealPointsRowViewModel> Points
        {
            get;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
