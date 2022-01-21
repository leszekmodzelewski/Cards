using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GeoLib.ViewModels;
using PointCalc;

namespace GeoLib.Logic
{
    public static class Points
    {


        static Points()
        {
            Initialize();
        }

        private static void Initialize()
        {
            MaxErrorBestFit = 10;
            rangeData = new List<RangeViewModel>();
            rangeData.Add(new RangeViewModel() { Range = "All", X2 = 5, Y2 = 5, Z2 = 5 });
        }

        internal static void Clear()
        {
            offsetToRealPointForDisplayPurposeOnly = null;
            valueOffsetArray.Clear();
            rangeData = null;
            ranges = null;
            RealPoints = null;
            TheoryPoints = null;
            MaxErrorFit = string.Empty;
            BestFitPointOffsetDictionary.Clear();
            MatchedPoints = null;
            MatchedPointsBestFit = null;
            MaxErrorBestFit = 0;

            Initialize();
        }

        private static int[] offsetToRealPointForDisplayPurposeOnly;

        public static int[] OffsetToRealPointForDisplayPurposeOnly
        {
            get => offsetToRealPointForDisplayPurposeOnly ?? new[] {0, 0, 0};
            set => offsetToRealPointForDisplayPurposeOnly = value;
        }

        private static List<ValueOffsetViewModel> valueOffsetArray = new List<ValueOffsetViewModel>();
        private static List<RangeViewModel> rangeData;
        private static Ranges ranges;

        public static List<ValueOffsetViewModel> ValueOffsetArray => valueOffsetArray;
        public static List<RangeViewModel> Range => rangeData;
        public static MyPoint3D[] RealPoints { get; set; }
        public static MyPoint3D[] TheoryPoints { get; set; }
        public static string MaxErrorFit { get; set; }
        
        public static Dictionary<string, int[]> BestFitPointOffsetDictionary { get; } = new Dictionary<string, int[]>();

        public static List<MatchedPoint> MatchedPoints { get; set; }
        public static List<MatchedPoint> MatchedPointsBestFit { get; set; }
        public static int MaxErrorBestFit { get; set; }

        public static void SetValueOffsetDataForTheoryPoints(IEnumerable<ValueOffsetViewModel> valueOffsetViewModel)
        {
            valueOffsetArray.Clear();
            valueOffsetArray.AddRange(valueOffsetViewModel);

            UpdateTheoryPointsAboutOffset();
        }


        public static void SetValueOffsetDataForRealPoints(ObservableCollection<RealPointsRowViewModel> valueOffsetViewModel)
        {
            BestFitPointOffsetDictionary.Clear();

            var valuesToModifyRealPoints = new Dictionary<string, int[]>();
            foreach (RealPointsRowViewModel realPointsRowViewModel in valueOffsetViewModel)
            {
                Logic.Points.BestFitPointOffsetDictionary[realPointsRowViewModel.RealPointId] = new int[]
                {
                    realPointsRowViewModel.DxFactor + realPointsRowViewModel.DxModifiedFactor,
                    realPointsRowViewModel.DyFactor + realPointsRowViewModel.DyModifiedFactor,
                    realPointsRowViewModel.DzFactor + realPointsRowViewModel.DzModifiedFactor
                };

                valuesToModifyRealPoints[realPointsRowViewModel.RealPointId] = new[]
                {
                    realPointsRowViewModel.DxFactor,
                    realPointsRowViewModel.DyFactor,
                    realPointsRowViewModel.DzFactor
                };
            }

            UpdateRealPointsAboutOffset(valuesToModifyRealPoints);
        }

        private static void UpdateTheoryPointsAboutOffset()
        {
            foreach (var myPoint3D in TheoryPoints)
            {
                myPoint3D.ReserveOffsetX = valueOffsetArray.FirstOrDefault(m=> m.X == Convert.ToInt32(myPoint3D.X))?.OffsetX ?? 0;
                myPoint3D.ReserveOffsetY = valueOffsetArray.FirstOrDefault(m => m.Y == Convert.ToInt32(myPoint3D.Y))?.OffsetY ?? 0;
                myPoint3D.ReserveOffsetZ = valueOffsetArray.FirstOrDefault(m => m.Z == Convert.ToInt32(myPoint3D.Z))?.OffsetZ ?? 0;
            }
        }

        private static void UpdateRealPointsAboutOffset(Dictionary<string, int[]> valuesToModifyRealPoints)
        {
            foreach (var myPoint3D in RealPoints)
            {
                var offset = valuesToModifyRealPoints.FirstOrDefault(m => m.Key == myPoint3D.Id).Value ?? new [] {0, 0, 0};
                myPoint3D.X += offset[0];
                myPoint3D.Y += offset[1];
                myPoint3D.Z += offset[2];
            }
        }

        public static void SetRangeData(IEnumerable<RangeViewModel> range)
        {
            rangeData.Clear();
            rangeData.AddRange(range);

            ranges = new Ranges(rangeData);

        }

        public static int[] GetRangeForX(double val)
        {
            int pointAsInt = Math.Abs(Convert.ToInt32(val));

            var range = ranges.AllRanges.FirstOrDefault(m => m.From <= pointAsInt && pointAsInt < m.To)?.Val ?? ranges.DefaultRange;
            return new []{range.Xmin, range.Xmax};
        }

        public static int[] GetRangeForY(double val)
        {
            int pointAsInt = Math.Abs(Convert.ToInt32(val));

            var range = ranges.AllRanges.FirstOrDefault(m => m.From <= pointAsInt && pointAsInt < m.To)?.Val ?? ranges.DefaultRange;
            return new[] { range.Ymin, range.Ymax };
        }

        public static int[] GetRangeForZ(double val)
        {
            int pointAsInt = Math.Abs(Convert.ToInt32(val));

            var range = ranges.AllRanges.FirstOrDefault(m => m.From <= pointAsInt && pointAsInt < m.To)?.Val ?? ranges.DefaultRange;
            return new[] { range.Zmin, range.Zmax };
        }
    }
}