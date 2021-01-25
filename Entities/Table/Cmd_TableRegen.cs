namespace GeoLib.Entities.Table
{
    
    using ZwSoft.ZwCAD.Runtime;
    using System;

    public class Cmd_TableRegen : Cmd_TableBase
    {
        [CommandMethod("TABLEREGEN", CommandFlags.UsePickSet)]
        public void TableRegen()
        {
            TableUtils.RegenerateAllTables(ZwSoft.ZwCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Database);
        }
    }

    

}

