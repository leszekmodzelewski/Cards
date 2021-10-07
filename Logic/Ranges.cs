using System;
using System.Collections.Generic;
using System.Linq;
using GeoLib.ViewModels;

namespace GeoLib.Logic
{
    class Ranges
    {
        public Ranges(IEnumerable<RangeViewModel> rangesDataViewModel)
        {
            AllRanges = new List<SingleRange>();
            DefaultRange = new RangeValue();
            ConvertToUsableData(rangesDataViewModel);
        }

        private void ConvertToUsableData(IEnumerable<RangeViewModel> rangesDataViewModel)
        {
            AllRanges.Clear();
            foreach (var rangeViewModel in rangesDataViewModel)
            {
                if (rangeViewModel.Range.Trim().ToUpper() == "ALL")
                {
                    DefaultRange.Xmax = rangeViewModel.X2;
                    DefaultRange.Ymax = rangeViewModel.Y2;
                    DefaultRange.Zmax = rangeViewModel.Z2;

                    DefaultRange.Xmin = GetValMin(rangeViewModel.X1, rangeViewModel.X2);
                    DefaultRange.Ymin = GetValMin(rangeViewModel.Y1, rangeViewModel.Y2);
                    DefaultRange.Zmin = GetValMin(rangeViewModel.Z1, rangeViewModel.Z2);

                    continue;
                }

                var rangeAsStringArray = rangeViewModel.Range.Trim().Split('-');
                if (rangeAsStringArray.Count() != 2)
                {
                    continue;
                }

                var singleRange = new SingleRange();
                if (int.TryParse(rangeAsStringArray[0], out int resultFrom))
                {
                    singleRange.From = resultFrom;
                }
                else
                {
                    continue;
                }

                if (int.TryParse(rangeAsStringArray[1], out int resultTo))
                {
                    singleRange.To = resultTo;
                }
                else
                {
                    continue;
                }

                RangeValue rv = new RangeValue();
                rv.Xmax = rangeViewModel.X2;
                rv.Ymax = rangeViewModel.Y2;
                rv.Zmax = rangeViewModel.Z2;

                rv.Xmin = GetValMin(rangeViewModel.X1, rangeViewModel.X2);
                rv.Ymin = GetValMin(rangeViewModel.Y1, rangeViewModel.Y2);
                rv.Zmin = GetValMin(rangeViewModel.Z1, rangeViewModel.Z2);
                
                this.AllRanges.Add(singleRange);
            }
        }

        int GetValMin(string strVal, int maxVal)
        {
            return string.IsNullOrEmpty(strVal) ? -maxVal : int.Parse(strVal);
        }

        public RangeValue DefaultRange { get; private set; }

        public List<SingleRange> AllRanges
        {
            get;
        }
    }

    public class RangeValue
    {
        public int Xmin { get; set; }
        public int Xmax { get; set; }

        public int Ymin { get; set; }
        public int Ymax { get; set; }


        public int Zmin { get; set; }
        public int Zmax { get; set; }


    }

}