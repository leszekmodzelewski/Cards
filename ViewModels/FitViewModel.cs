using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using System.Windows.Input;
using GeoLib.Logic;
using GeoLib.Wpf;
using PointCalc;

namespace GeoLib.ViewModels
{
    public class FitViewModel : INotifyPropertyChanged
    {
        private string filePath = @"c:\NetPrograms\Zwcad\PointsDataForCardsWithSemiColon.txt";
        private int maxErrorFit;

        public string FilePath
        {
            get => filePath;
            set
            {
                filePath = value;
                OnPropertyChanged();
            }
        }

        public int MaxErrorFit
        {
            get => maxErrorFit;
            set
            {
                maxErrorFit = value;
                OnPropertyChanged();
            }
        }

        public FitViewModel(IEnumerable<ValueOffsetViewModel> valueOffset, IEnumerable<RangeViewModel> ranges)
        {
            foreach (var rangeViewModel in ranges)
            {
                Ranges.Add(rangeViewModel);
            }

            foreach (var valueOffsetViewModel in valueOffset)
            {
                ValueOffset.Add(valueOffsetViewModel);
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


        public ICommand ReadFromFileCommand => new SimpleCommand(ReadFromFileExecute);

        private void ReadFromFileExecute()
        {
            Points.RealPoints = PointData.GetPoints(filePath);
            MessageBox.Show($@"Read {Points.RealPoints.Length} points from file.");
        }

        public ICommand ApplyCommand => new SimpleCommand(ApplyExecute);

        private void ApplyExecute()
        {
            Points.SetRangeData(this.Ranges);
            Points.SetValueOffsetData(this.ValueOffset);
            Points.MaxErrorFit = this.MaxErrorFit;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
