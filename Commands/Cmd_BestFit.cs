using System;
using System.Collections.Generic;
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

            //PointCalculator pc = new PointCalculator();
            //Points.MatchedPointsBestFit = pc.CalculateBestFit(Points.TheoryPoints, Points.RealPoints, Points.MaxErrorBestFit);

            //CalculationUtils.UpdateCadEntity(Points.MatchedPointsBestFit, ZwSoft.ZwCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Database);
        }

       
    }
}