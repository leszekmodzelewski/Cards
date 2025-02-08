

namespace GeoLib
{
    using System;
    using ZwSoft.ZwCAD.ApplicationServices;
    using ZwSoft.ZwCAD.DatabaseServices;
    using ZwSoft.ZwCAD.EditorInput;
    using ZwSoft.ZwCAD.Runtime;

    public class Cmd_Test
    {
        [CommandMethod("TESTCMD", CommandFlags.UsePickSet)]
        public void NewTestCmd()
        {
            Document mdiActiveDocument = ZwSoft.ZwCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Editor editor = mdiActiveDocument.Editor;
            PromptSelectionResult selection = editor.SelectImplied();
            if (selection.Value == null)
            {
                selection = editor.GetSelection();
                if (selection.Value == null)
                {
                    return;
                }
            }
            if (selection.Value.Count > 0)
            {
                using (Transaction transaction = mdiActiveDocument.Database.TransactionManager.StartTransaction())
                {
                    foreach (DynamicBlockReferenceProperty property in ((BlockReference)transaction.GetObject(selection.Value[0].ObjectId, OpenMode.ForWrite)).DynamicBlockReferencePropertyCollection)
                    {
                        ZwSoft.ZwCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage(property.PropertyName.ToString() + Environment.NewLine);
                        ZwSoft.ZwCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage(property.Value.ToString() + Environment.NewLine);
                        if (property.PropertyName == "Position1 X")
                        {
                            property.Value = ((double)property.Value) + 100.0;
                        }
                    }
                    transaction.Commit();
                }
            }
        }
    }
}

