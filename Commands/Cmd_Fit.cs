using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    public class Cmd_Fit
    {
        [CommandMethod("FIT", CommandFlags.UsePickSet)]
        public void Execute()
        {
            using (Winforms.CalculateDataWindow dlg = new CalculateDataWindow())
            {
                Points.TheoryPoints = ReadTheoryPointsFromCad().ToArray();
                SaveTofile();
                var vm = new FitViewModel(Points.ValueOffset, Points.Range);
                vm.MaxErrorFit = Points.MaxErrorFit;
                var form = new GenericWinFormForWpf(new FitCtrl(vm));
                form.ShowDialog();

                PointCalculator pc = new PointCalculator();
                Points.MatchedPoints = pc.Calculate(Points.TheoryPoints, Points.RealPoints, Points.MaxErrorFit);

                CalculationUtils.UpdateCadEntity(Points.MatchedPoints, ZwSoft.ZwCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Database);
            }
        }

        private List<MyPoint3D> ReadTheoryPointsFromCad()
        {
            return CalculationUtils.ReadFromCad(ZwSoft.ZwCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Database);
        }

        public void SaveTofile()
        {
            var path = @"c:\NetPrograms\Zwcad\theoryPoint.txt";

            using (StreamWriter outputFile = new StreamWriter(path))
            {
                for (int i = 0; i < Points.TheoryPoints.Length; ++i)
                {
                    outputFile.WriteLine($"{i};{Points.TheoryPoints[i].X};{Points.TheoryPoints[i].Y};{Points.TheoryPoints[i].Z}");
                }
                    
            }

        }
    }
}
