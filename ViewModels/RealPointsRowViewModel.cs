﻿using GeoLib.Controls;
using GeoLib.Winforms;
using GeoLib.Wpf;
using PointCalc;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using System.Windows.Input;


namespace GeoLib.ViewModels
{
    public class RealPointsRowViewModel : INotifyPropertyChanged
    {
        private int dxFactor;
        private int dyFactor;
        private int dzFactor;
        private int id;
        private const string missing = " ";

        public int Id
        {
            get => id;
            set
            {
                id = value;
                OnPropertyChanged();
                
            }
        }

        public string RealPointId { get; set; }

        
        public int X => Convert.ToInt32(this.MatchedPoint.TheoryPoint.Xo);
        public int Y => Convert.ToInt32(this.MatchedPoint.TheoryPoint.Yo);
        public int Z => Convert.ToInt32(this.MatchedPoint.TheoryPoint.Zo);


        public string Dx
        {
            get
            {
                if (this.MatchedPoint.RealPoint == null)
                {
                    return missing;
                }

                return $"{(this.MatchedPoint.RealPoint.X - this.MatchedPoint.TheoryPoint.X + DxFactor):0.0}";
            }
        }
        public string Dy
        {
            get
            {
                if (this.MatchedPoint.RealPoint == null)
                {
                    return missing;
                }

                return $"{(this.MatchedPoint.RealPoint.Y - this.MatchedPoint.TheoryPoint.Y + DyFactor):0.0}";
            }
        }
        public string Dz
        {
            get
            {
                if (this.MatchedPoint.RealPoint == null)
                {
                    return missing;
                }

                return $"{(this.MatchedPoint.RealPoint.Z - this.MatchedPoint.TheoryPoint.Z + DzFactor):0.0}";
            }
        }

        public int DxFactor
        {
            get => dxFactor;
            set
            {
                dxFactor = value;
                OnPropertyChanged("Dx");
                OnPropertyChanged("DxFactorText");
            }
        }
        public int DyFactor
        {
            get => dyFactor;
            set
            {
                dyFactor = value;
                OnPropertyChanged("Dy");
                OnPropertyChanged("DyFactorText");
            }
        }
        public int DzFactor
        {
            get => dzFactor;
            set
            {
                dzFactor = value;
                OnPropertyChanged("Dz");
                OnPropertyChanged("DzFactorText");
                
            }
        }


        private int dxModifiedFactor, dyModifiedFactor, dzModifiedFactor;


        public int DxModifiedFactor
        {
            get => dxModifiedFactor;
            set
            {
                dxModifiedFactor = value;
                OnPropertyChanged();
                
            }
        }
        public int DyModifiedFactor
        {
            get => dyModifiedFactor;
            set
            {
                dyModifiedFactor = value;
                OnPropertyChanged();
            }
        }
        public int DzModifiedFactor
        {
            get => dzModifiedFactor;
            set
            {
                dzModifiedFactor = value;
                OnPropertyChanged();
            }
        }











        //==========================================
        public string DxFactorText => dxFactor == 0 ? "+" : dxFactor.ToString();

        public string DyFactorText => dyFactor == 0 ? "+" : dyFactor.ToString();

        public string DzFactorText => dzFactor == 0 ? "+" : dzFactor.ToString();


        public MatchedPoint MatchedPoint { get; set; }

        public ICommand DxExecuteCommand => new SimpleCommand(DxExecute);
        private void DxExecute()
        {
            var vm = new BoxWithTextViewModel { IntegerValue = this.DxFactor };
            ShowDialog(vm);
            if (vm.DialogResult == DialogResult.OK)
            {
                this.DxFactor = vm.IntegerValue;
            }
        }

        public ICommand DyExecuteCommand => new SimpleCommand(DyExecute);
        private void DyExecute()
        {
            var vm = new BoxWithTextViewModel { IntegerValue = this.DyFactor };
            ShowDialog(vm);
            if (vm.DialogResult == DialogResult.OK)
            {
                this.DyFactor = vm.IntegerValue;
            }
        }

        public ICommand DzExecuteCommand => new SimpleCommand(DzExecute);
        private void DzExecute()
        {
            var vm = new BoxWithTextViewModel { IntegerValue = this.DzFactor };
            ShowDialog(vm);
            if (vm.DialogResult == DialogResult.OK)
            {
                this.DzFactor = vm.IntegerValue;
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        internal static void ShowDialog(BoxWithTextViewModel vm)
        {
            var form = new GenericWinFormForWpf(new BoxWithText(vm));
            form.ShowDialog();
        }
    }


}
