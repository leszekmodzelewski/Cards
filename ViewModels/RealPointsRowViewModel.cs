using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GeoLib.Wpf;
using PointCalc;

namespace GeoLib.ViewModels
{
    public class RealPointsRowViewModel : INotifyPropertyChanged
    {
        //private int x;
        //private int y;
        //private int z;
        //private int dx;
        //private int dy;
        //private int dz;
        private int dxFactor;
        private int dyFactor;
        private int dzFactor;
        private int id;
        private const string missing = "Missing";

        public int Id
        {
            get => id;
            set
            {
                id = value;
                OnPropertyChanged();
            }
        }


        public int X => Convert.ToInt32(this.MatchedPoint.TheoryPoint.X);
        public int Y => Convert.ToInt32(this.MatchedPoint.TheoryPoint.Y);
        public int Z => Convert.ToInt32(this.MatchedPoint.TheoryPoint.Z);


        public string Dx
        {
            get
            {
                if (this.MatchedPoint.RealPoint == null)
                {
                    return missing;
                }

                return (this.MatchedPoint.RealPoint.X - this.MatchedPoint.TheoryPoint.X + DxFactor).ToString(CultureInfo.CurrentCulture);
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

                return (this.MatchedPoint.RealPoint.Y - this.MatchedPoint.TheoryPoint.Y + DyFactor).ToString(CultureInfo.CurrentCulture);
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

                return (this.MatchedPoint.RealPoint.Z - this.MatchedPoint.TheoryPoint.Z + DzFactor).ToString(CultureInfo.CurrentCulture);
            }
        }

        public int DxFactor
        {
            get => dxFactor;
            set
            {
                dxFactor = value;
                OnPropertyChanged("Dx");
            }
        }
        public int DyFactor
        {
            get => dyFactor;
            set
            {
                dyFactor = value;
                OnPropertyChanged("Dy");
            }
        }
        public int DzFactor
        {
            get => dzFactor;
            set
            {
                dzFactor = value;
                OnPropertyChanged("Dz");
            }
        }

        
        public MatchedPoint MatchedPoint { get; set; }

        public ICommand DxExecuteCommand => new SimpleCommand(DxExecute);
        private void DxExecute()
        {
            //this.Dx += 1;
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
