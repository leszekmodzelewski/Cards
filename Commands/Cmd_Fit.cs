using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GeoLib.Controls;
using GeoLib.Entities.Table;
using GeoLib.ViewModels;
using GeoLib.Winforms;
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
                var vm = new FitViewModel();
                var form = new GenericWinFormForWpf(new FitCtrl(vm));
                form.ShowDialog();
                
                
                //var res = dlg.ShowDialog();
                //if (res == DialogResult.OK)
                //{
                //    FileCoordsDataUtils.ReadPointsFromFile(dlg.FileName, dlg.ScaleFactor);
                //    CalculationUtils.Calculate(ZwSoft.ZwCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Database, dlg.FileName);

                //}
            }
        }
    }
}
