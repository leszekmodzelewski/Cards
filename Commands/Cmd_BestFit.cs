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
            var points = new List<RealPointsRowViewModel>();
            points.Add(new RealPointsRowViewModel());
            points.Add(new RealPointsRowViewModel());
            points.Add(new RealPointsRowViewModel());
            points.Add(new RealPointsRowViewModel());
            points.Add(new RealPointsRowViewModel());

            points[0].X = 5;
            points[1].X = 6;
            points[2].X = 8;
            points[3].X = 35;

            var bf = new BestFitViewModel(points);
            using (var form = new GenericWinFormForWpf(new BestFitCtrl(bf)))
            {
                form.ShowDialog();
            }
        }
    }
}