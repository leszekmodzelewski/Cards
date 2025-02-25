﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using GeoLib.Entities.Table;
using GeoLib.Entities.TableRef;
using GeoLib.Wpf;
using Newtonsoft.Json.Serialization;
using PointCalc;
using ZwSoft.ZwCAD.Colors;



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
        public int Scale
        {
            get => Scale;
            set { Scale = value; OnPropertyChanged(); }
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

        public ICommand OkCommand => new SimpleCommand(OkExecute);

        private void OkExecute()
        {
            
            ApplyExecute();
            Close?.Invoke(this, new CloseEventArgs(DialogResult.OK));
        }

        public event EventHandler<CloseEventArgs> Close;

        private void ApplyExecute()
        {
            applyExecuted = true;
            ResetToDefaults();
            
            Calculate();
        }
       
        private void ResetToDefaults()
        {
            //var points = GetPointsForBestFitDialog(Logic.Points.MatchedPoints);
            //Logic.Points.MatchedPoints.Clear();
            
            foreach (var item in this.Points)
            {
                var res = Logic.Points.MatchedPoints.FirstOrDefault(m => item.RealPointId == (m.RealPoint?.Id ?? m.TheoryPoint.Id));
                if (res == null)
                {
                    throw new ApplicationException("Unable to find point for reset");
                }
                item.MatchedPoint = res;
            }
        }

        public bool applyExecuted = false;

        public void Calculate()
        {
            if (applyExecuted)
            {
                Logic.Points.SetValueOffsetDataForRealPoints(Points);
               
                Logic.Points.MaxErrorBestFit = this.MaxFitValue;

                
                CalculateUsingBestFit();
            }
        }

        private void CalculateUsingBestFit()
        {
            var r = new List<MyPoint3D>();
            var t = new List<MyPoint3D>();

            foreach (RealPointsRowViewModel pointsRowViewModel in Points)
            {
                if (pointsRowViewModel.MatchedPoint.RealPoint != null &&
                    pointsRowViewModel.MatchedPoint.TheoryPoint != null)
                {
                    r.Add(pointsRowViewModel.MatchedPoint.RealPoint);
                    t.Add(pointsRowViewModel.MatchedPoint.TheoryPoint);
                }
            }
            
                PointCalculator pc = new PointCalculator();
            Logic.Points.MatchedPointsBestFit = pc.CalculateBestFit(t.ToArray(), r.ToArray(), Logic.Points.MaxErrorBestFit, out List<MyPoint3D> realPointsAfterInversions);
            
            CadDrawPoints.Draw(realPointsAfterInversions.ToArray(), "bestfit", Color.FromColor(System.Drawing.Color.PaleVioletRed));

            if (Logic.Points.resultImport == DialogResult.No)
            {
                CalculationUtils.UpdateCadEntity(Logic.Points.MatchedPointsBestFit, ZwSoft.ZwCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Database, Logic.Points.OffsetToRealPointForDisplayPurposeOnly);
            }
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
                    Id = i + 1,
                    RealPointId = matchedPoints[i].RealPoint?.Id ?? matchedPoints[i].TheoryPoint.Id
                }
                );

                if (matchedPoints[i].RealPoint != null)
                {
                    if (Logic.Points.BestFitPointOffsetDictionary.TryGetValue(matchedPoints[i].RealPoint.Id,
                        out int[] pointFactor))
                    {
                        points[points.Count - 1].DxModifiedFactor = pointFactor[0];
                        points[points.Count - 1].DyModifiedFactor = pointFactor[1];
                        points[points.Count - 1].DzModifiedFactor = pointFactor[2];
                    }
                }
            }

            return points;
        }
    }
}
