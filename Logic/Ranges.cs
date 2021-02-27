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
            DefaultRange = new[] {0, 0, 0};
            ConvertToUsableData(rangesDataViewModel);
        }

        private void ConvertToUsableData(IEnumerable<RangeViewModel> rangesDataViewModel)
        {
            AllRanges.Clear();
            foreach (var rangeViewModel in rangesDataViewModel)
            {
                if (rangeViewModel.Range.Trim().ToUpper() == "ALL")
                {
                    DefaultRange[0] = rangeViewModel.X;
                    DefaultRange[1] = rangeViewModel.Y;
                    DefaultRange[2] = rangeViewModel.Z;

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

                singleRange.Val = new[] {rangeViewModel.X, rangeViewModel.Y, rangeViewModel.Z};
                this.AllRanges.Add(singleRange);
            }
        }


        public int[] DefaultRange { get; private set; }

        public List<SingleRange> AllRanges
        {
            get;
        }

    }
}