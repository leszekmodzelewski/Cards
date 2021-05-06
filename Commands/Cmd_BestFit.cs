using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;
using GeoLib.Controls;
using GeoLib.Entities.Table;
using GeoLib.Logic;
using GeoLib.ViewModels;
using GeoLib.Winforms;
using PointCalc;
using ZwSoft.ZwCAD.Runtime;

namespace GeoLib.Commands
{
    public class Cmd_BestFit
    {
        [CommandMethod("BESTFIT", CommandFlags.UsePickSet)]
        public void Execute()
        {
            MessageBox.Show("aa");
            if (Points.MatchedPoints == null)
            {
                MessageBox.Show("Points are not matched yet.", "Fit error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return;
            }

            if (!Verify(Points.MatchedPoints))
            {
                MessageBox.Show("There should be at least 2 points matched before proceed.", "Fit error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return;
            }

            var matchedPoints = Points.MatchedPointsBestFit == null || Points.MatchedPointsBestFit.Count == 0
                ? Points.MatchedPoints
                : Points.MatchedPointsBestFit;

            var points = BestFitViewModel.GetPointsForBestFitDialog(matchedPoints);

            var bf = new BestFitViewModel(points);
            bf.MaxFitValue = Points.MaxErrorBestFit;
            using (var form = new GenericWinFormForWpf(new BestFitCtrl(bf)))
            {
                form.ShowDialog();
            }

            bf.Calculate();
        }

        private bool Verify(List<MatchedPoint> matchedPoints)
        {
            int matchedPointsCount = 0;
            foreach (MatchedPoint matchedPoint in matchedPoints)
            {
                if (matchedPoint.TheoryPoint != null && matchedPoint.RealPoint != null)
                {
                    matchedPointsCount++;
                    if (matchedPointsCount >= 2)
                        return true;
                }
            }

            return false;
        }
    }
}