using GeoLib.Logic;
using ZwSoft.ZwCAD.Runtime;

namespace GeoLib.Entities.Table
{
    public class Cmd_Clear : Cmd_TableBase
    {
        [CommandMethod("CLEAR", CommandFlags.UsePickSet)]
        public void ClearData()
        {
            Points.Clear();
            Cmd_TableRegen.Refresh();
        }
    }
}