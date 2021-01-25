namespace GeoLib
{
    using ZwSoft.ZwCAD.Runtime;
    using System;

    public class GeoApp
    {
        [CommandMethod("GEOSTARTUP")]
        public void TestCmdStartup()
        {
            ZwSoft.ZwCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage("Application loaded Darek." + Environment.NewLine);
        }
    }
}

