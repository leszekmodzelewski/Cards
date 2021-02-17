using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GeoLib.Wpf;

namespace GeoLib.ViewModels
{
    public class FitViewModel : INotifyPropertyChanged
    {
        private string filePath;
        public string FilePath
        {
            get => filePath;
            set
            {
                filePath = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<ValueOffsetViewModel> ValueOffset { get; } = new ObservableCollection<ValueOffsetViewModel>();

        public ObservableCollection<RangeViewModel> Ranges { get; } = new ObservableCollection<RangeViewModel>();

        public ICommand AddValueOffsetCommand => new SimpleCommand(AddValueOffsetExecute);

        private void AddValueOffsetExecute()
        {
            this.ValueOffset.Add(new ValueOffsetViewModel());
        }

        public ICommand RemoveValueOffsetCommand => new SimpleCommand(RemoveValueOffsetExecute);

        private void RemoveValueOffsetExecute()
        {
            if (this.ValueOffset.Count > 0)
            {
                this.ValueOffset.RemoveAt(this.ValueOffset.Count - 1);
            }
        }

        public ICommand AddRangeCommand => new SimpleCommand(AddRangeExecute);

        private void AddRangeExecute()
        {
            this.Ranges.Add(new RangeViewModel());
        }

        public ICommand RemoveRangeCommand => new SimpleCommand(RemoveRangeExecute);

        private void RemoveRangeExecute()
        {
            if (this.Ranges.Count > 0)
            {
                this.Ranges.RemoveAt(this.Ranges.Count - 1);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
