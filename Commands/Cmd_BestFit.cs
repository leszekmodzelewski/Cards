using System;
using System.Collections.Generic;
using System.Windows.Forms;
using GeoLib.Controls;
using GeoLib.ViewModels;
using GeoLib.Winforms;
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

            var points = new List<RealPointsRowViewModel>();

            for (int i=0; i<Points.MatchedPoints.Count; ++i)
            { 
                points.Add(new RealPointsRowViewModel
                {
                    MatchedPoint = Points.MatchedPoints[i],
                    Id = i+1

                    //RealX = Points.MatchedPoints[i].RealPoint?.X,
                    //RealY = Points.MatchedPoints[i].RealPoint?.Y,
                    //RealZ = Points.MatchedPoints[i].RealPoint?.Z,

                    //X = Convert.ToInt32(Points.MatchedPoints[i].TheoryPoint.X),
                    //Y = Convert.ToInt32(Points.MatchedPoints[i].TheoryPoint.Y),
                    //Z = Convert.ToInt32(Points.MatchedPoints[i].TheoryPoint.Z),
                }
                );

                if(Points.BestFitPointOffsetDictionary.TryGetValue(Points.MatchedPoints[i].TheoryPoint.Id, out int [] pointFactor))
                {
                    points[points.Count - 1].DxFactor = pointFactor[0];
                    points[points.Count - 1].DyFactor = pointFactor[1];
                    points[points.Count - 1].DzFactor = pointFactor[2];
                }
            }
           

            var bf = new BestFitViewModel(points);
            using (var form = new GenericWinFormForWpf(new BestFitCtrl(bf)))
            {
                form.ShowDialog();
            }
        }
    }
}