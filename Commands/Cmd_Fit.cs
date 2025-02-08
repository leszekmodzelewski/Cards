using GeoLib.Controls;
using GeoLib.Entities.Table;
using GeoLib.Logic;
using GeoLib.ViewModels;
using GeoLib.Winforms;
using PointCalc;
using System;
using System.IO;
using System.Windows.Forms;
using ZwSoft.ZwCAD.Runtime;

namespace GeoLib.Commands
{
    public class Cmd_Fit
    {
        
        [CommandMethod("FIT", CommandFlags.UsePickSet)]
        public void Execute()
        {
            Points.resultImport = MessageBox.Show("Iport Coordinates from cad points? ", "Iport", MessageBoxButtons.YesNo);

            if (Points.resultImport == DialogResult.Yes)
            {
                if (Points.RealPoints != null && Points.RealPoints.Length > 0)
                {
                    PointCalculator pc = new PointCalculator();
                    Points.MatchedPoints = pc.Calculate(Points.TheoryPoints, Points.RealPoints,
                        string.IsNullOrEmpty(Points.MaxErrorFit)
                            ? Double.MaxValue
                            : int.Parse(Points.MaxErrorFit));

                    //CalculationUtils.UpdateCadEntity(Points.MatchedPoints, ZwSoft.ZwCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Database, Points.OffsetToRealPointForDisplayPurposeOnly);
                }
                else
                {
                    //CalculationUtils.UpdateCadRangeOnlyEntity(ZwSoft.ZwCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Database, Points.TheoryPoints);
                }
            }
            else
            {
                using (Winforms.CalculateDataWindow dlg = new CalculateDataWindow())
                {
                    Points.resultImport = DialogResult.No;
                    Points.TheoryPoints = CalculationUtils.ReadTheoryPointsFromCad().ToArray();


                    var vm = new FitViewModel(Points.ValueOffsetArray, Points.Range);
                    vm.MaxErrorFit = Points.MaxErrorFit;
                    var form = new GenericWinFormForWpf(new FitCtrl(vm));

                    vm.RangeUpdate += (o, a) =>
                    {
                        CalculationUtils.UpdateCadRangeOnlyEntity(ZwSoft.ZwCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Database, Points.TheoryPoints);
                    }
                    ;


                    vm.Close += (o, a) =>
                    {
                        form.Close();
                        form.Dispose();
                        if (a.Result == DialogResult.OK)
                        {
                            if (Points.RealPoints != null && Points.RealPoints.Length > 0)
                            {
                                PointCalculator pc = new PointCalculator();
                                Points.MatchedPoints = pc.Calculate(Points.TheoryPoints, Points.RealPoints,
                                    string.IsNullOrEmpty(Points.MaxErrorFit)
                                        ? Double.MaxValue
                                        : int.Parse(Points.MaxErrorFit));

                                CalculationUtils.UpdateCadEntity(Points.MatchedPoints, ZwSoft.ZwCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Database, Points.OffsetToRealPointForDisplayPurposeOnly);
                            }
                            else
                            {
                            //CalculationUtils.UpdateCadRangeOnlyEntity(ZwSoft.ZwCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Database, Points.TheoryPoints);
                            }


                        }
                    };

                    form.ShowDialog();
                }
            }
        }



        public void SaveToFile()
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
