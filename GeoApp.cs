namespace GeoLib
{
    using System;
    using ZwSoft.ZwCAD.Runtime;

    public class GeoApp
    {
        [CommandMethod("GEOSTARTUP")]
        public void TestCmdStartup()
        {
            ZwSoft.ZwCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage("Application loaded Darek." + Environment.NewLine);
        }
    }
}

