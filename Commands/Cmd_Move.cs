using System.Windows.Forms;
using GeoLib.Controls;
using GeoLib.Logic;
using GeoLib.ViewModels;
using GeoLib.Winforms;
using ZwSoft.ZwCAD.Runtime;

namespace GeoLib.Commands
{
    public class Cmd_Move
    {
        [CommandMethod("MOVEPOINTS", CommandFlags.UsePickSet)]
        public void Execute()
        {
            var model = new MoveViewModel(Points.OffsetToRealPointForDisplayPurposeOnly);

            var form = new GenericWinFormForWpf(new MoveCtrl(model));

            //bf.Close += (o, a) =>
            //{
            //    if (a.Result == DialogResult.OK)
            //    {
            //        form.Close();
            //        form.Dispose();
            //        bf.Calculate();
            //    }
            //};

            form.ShowDialog();
            Points.OffsetToRealPointForDisplayPurposeOnly = model.Offset;
        }
    }
}