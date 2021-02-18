using System;
using System.Collections.Generic;
using System.Linq;
using PointCalc;

namespace GeoLib.ViewModels
{
    public static class Points
    {
        private static List<ValueOffsetViewModel> valueOffset = new List<ValueOffsetViewModel>();
        private static List<RangeViewModel> rangeData = new List<RangeViewModel>();

        private static Ranges ranges;

        public static List<ValueOffsetViewModel> ValueOffset => valueOffset;
        public static List<RangeViewModel> Range => rangeData;


        public static MyPoint3D[] RealPoints { get; set; }

        public static MyPoint3D[] TheoryPoints { get; set; }
        public static double MaxErrorFit { get; set; }
        public static List<MatchedPoint> MatchedPoints { get; set; }

        public static void SetValueOffsetData(IEnumerable<ValueOffsetViewModel> valueOffsetViewModel)
        {
            valueOffset.Clear();
            valueOffset.AddRange(valueOffsetViewModel);

            UpdatePointsAboutOffset();
        }

        private static void UpdatePointsAboutOffset()
        {
            foreach (var myPoint3D in TheoryPoints)
            {
                myPoint3D.ReserveOffsetX = valueOffset.FirstOrDefault(m=> m.X == Convert.ToInt32(myPoint3D.X))?.OffsetX ?? 0;
                myPoint3D.ReserveOffsetY = valueOffset.FirstOrDefault(m => m.Y == Convert.ToInt32(myPoint3D.Y))?.OffsetY ?? 0;
                myPoint3D.ReserveOffsetZ = valueOffset.FirstOrDefault(m => m.Z == Convert.ToInt32(myPoint3D.Z))?.OffsetZ ?? 0;
            }
        }


        public static void SetRangeData(IEnumerable<RangeViewModel> range)
        {
            rangeData.Clear();
            rangeData.AddRange(range);

            ranges = new Ranges(rangeData);

        }


        public static int GetRangeForX(double val)
        {
            int pointAsInt = Math.Abs(Convert.ToInt32(val));

            var range = ranges.AllRanges.FirstOrDefault(m => m.From <= pointAsInt && pointAsInt < m.To)?.Val ?? ranges.DefaultRange;
            return range[0];
        }

        public static int GetRangeForY(double val)
        {
            int pointAsInt = Math.Abs(Convert.ToInt32(val));

            var range = ranges.AllRanges.FirstOrDefault(m => m.From <= pointAsInt && pointAsInt < m.To)?.Val ?? ranges.DefaultRange;
            return range[1];
        }

        public static int GetRangeForZ(double val)
        {
            int pointAsInt = Math.Abs(Convert.ToInt32(val));

            var range = ranges.AllRanges.FirstOrDefault(m => m.From <= pointAsInt && pointAsInt < m.To)?.Val ?? ranges.DefaultRange;
            return range[2];
        }

        public static Dictionary<long, int[]> BestFitPointOffsetDictionary { get; } = new Dictionary<long, int[]>();
    }
}