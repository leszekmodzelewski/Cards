using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GeoLib.Entities.Table;
using GeoLib.Wpf;
using PointCalc;

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


        public ICommand ApplyCommand => new SimpleCommand(ApplyExecute);

        private void ApplyExecute()
        {
            //Logic.Points.SetRangeData(this.Ranges);
            //Logic.Points.SetValueOffsetData(this.ValueOffset);
            Logic.Points.MaxErrorBestFit = this.MaxFitValue;

            CalculateUsingBestFit();


        }

        private void CalculateUsingBestFit()
        {
            PointCalculator pc = new PointCalculator();
            Logic.Points.MatchedPointsBestFit = pc.CalculateBestFit(Logic.Points.TheoryPoints, Logic.Points.RealPoints, Logic.Points.MaxErrorBestFit);

            CalculationUtils.UpdateCadEntity(Logic.Points.MatchedPointsBestFit, ZwSoft.ZwCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Database);

            this.Points.Clear();

            foreach (var realPointsRowViewModel in GetPointsForBestFitDialog(Logic.Points.MatchedPointsBestFit))
            {
                this.Points.Add(realPointsRowViewModel);
            }
            
        }


        public static List<RealPointsRowViewModel> GetPointsForBestFitDialog(List<MatchedPoint> matchedPoints)
        {
            var points = new List<RealPointsRowViewModel>();

            for (int i = 0; i < matchedPoints.Count; ++i)
            {
                points.Add(new RealPointsRowViewModel
                    {
                        MatchedPoint = matchedPoints[i],
                        Id = i + 1
                    }
                );

                if (Logic.Points.BestFitPointOffsetDictionary.TryGetValue(matchedPoints[i].TheoryPoint.Id, out int[] pointFactor))
                {
                    points[points.Count - 1].DxFactor = pointFactor[0];
                    points[points.Count - 1].DyFactor = pointFactor[1];
                    points[points.Count - 1].DzFactor = pointFactor[2];
                }
            }

            return points;
        }
    }
}
