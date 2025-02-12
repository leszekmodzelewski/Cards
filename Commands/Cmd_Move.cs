﻿using GeoLib.Controls;
using GeoLib.Entities.Table;
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

            model.Apply += (o, a) =>
            {
                Points.OffsetToRealPointForDisplayPurposeOnly = model.Offset;
                form.Close();
                form.Dispose();
                Cmd_TableRegen.Refresh();
            };

            model.Cancel += (o, a) =>
            {
                form.Close();
                form.Dispose();
            };

            form.ShowDialog();

        }
    }
}