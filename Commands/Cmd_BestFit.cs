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

            var matchedPoints = Points.MatchedPoints;

            var points = BestFitViewModel.GetPointsForBestFitDialog(matchedPoints);

            var bf = new BestFitViewModel(points) {MaxFitValue = Points.MaxErrorBestFit};

            var form = new GenericWinFormForWpf(new BestFitCtrl(bf));
            
            bf.Close += (o, a) =>
            {
                if (a.Result == DialogResult.OK)
                {
                    form.Close();
                    form.Dispose();
                    bf.Calculate();
                }
            };
                
            form.ShowDialog();
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