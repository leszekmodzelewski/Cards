

namespace GeoLib.Entities.RectBlanking
{
    using ZwSoft.ZwCAD.ApplicationServices;
    using ZwSoft.ZwCAD.DatabaseServices;
    using ZwSoft.ZwCAD.EditorInput;
    using ZwSoft.ZwCAD.Geometry;
    using ZwSoft.ZwCAD.Runtime;
    using GeoLib;
    using System;

    public class Cmd_RectBlankingCreate
    {
        [CommandMethod("RECTBLANKINGCREATE", CommandFlags.UsePickSet)]
        public void CreateRectBlanking()
        {
            Document mdiActiveDocument = ZwSoft.ZwCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Editor editor = mdiActiveDocument.Editor;
            ObjectId blockId = AppUtils.EnsureRectBlanking(mdiActiveDocument.Database);
            if (!blockId.IsNull)
            {
                PromptPointResult point = editor.GetPoint("Specify first point.");
                if (point.Status == PromptStatus.OK)
                {
                    PromptPointResult result2 = null;
                    using (new TransientRectBlanking(point.Value))
                    {
                        result2 = editor.GetPoint("Specify second point.");
                        if (result2.Status != PromptStatus.OK)
                        {
                            return;
                        }
                    }
                    double x = Math.Min(point.Value.X, result2.Value.X);
                    double num2 = Math.Max(point.Value.X, result2.Value.X);
                    double y = Math.Min(point.Value.Y, result2.Value.Y);
                    Point3d start = new Point3d(x, y, 0.0);
                    RectBlankingUtils.CreateBlockReference(mdiActiveDocument.Database, blockId, start, num2 - x, Math.Max(point.Value.Y, result2.Value.Y) - y);
                }
            }
        }
    }
}

