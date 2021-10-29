using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using System.Windows.Input;
using GeoLib.Entities.RectBlanking;
using GeoLib.Logic;
using GeoLib.Wpf;
using PointCalc;

namespace GeoLib.ViewModels
{
    public class FitViewModel : INotifyPropertyChanged
    {
        private string filePath = @"c:\NetPrograms\Zwcad\PointsDataForCardsWithSemiColon.txt";
        private string maxErrorFit = string.Empty;

        public string FilePath
        {
            get => filePath;
            set
            {
                filePath = value;
                OnPropertyChanged();
            }
        }

        public string MaxErrorFit
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

        public ICommand ReadRecentlyExported => new SimpleCommand(ImportRecentlyExported);

        private void ImportRecentlyExported()
        {
            // disable when CardsData.RecentlyExportedPoints == null
            Points.RealPoints = CardsData.RecentlyExportedPoints?.Select(m => new MyPoint3D(m.Point.X, m.Point.Y, m.Point.Z, m.TextId)).ToArray();
        }

        public ICommand ApplyCommand => new SimpleCommand(ApplyExecute);

        public ICommand OkCommand => new SimpleCommand(OkExecute);

        public ICommand ApplyRangeCommand => new SimpleCommand(RangeExecute);


        public bool IsRecentlyExported => CardsData.RecentlyExportedPoints != null && CardsData.RecentlyExportedPoints.Any();

        private void ApplyExecute()
        {
            Points.SetRangeData(this.Ranges);
            Points.SetValueOffsetDataForTheoryPoints(this.ValueOffset);
            Points.MaxErrorFit = this.MaxErrorFit;
        }

        private void OkExecute()
        {
            ApplyExecute();
            Close?.Invoke(this, new CloseEventArgs(DialogResult.OK));
        }

        private void RangeExecute()
        {
            Points.SetRangeData(this.Ranges);
            RangeUpdate?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler<CloseEventArgs> Close;

        public event EventHandler<EventArgs> RangeUpdate;

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
