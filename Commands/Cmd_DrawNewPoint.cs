using GeoLib.Entities.Table;
using GeoLib.Entities.TableRef;
using PointCalc;
using System.Collections.Generic;
using ZwSoft.ZwCAD.ApplicationServices;
using ZwSoft.ZwCAD.Colors;
using ZwSoft.ZwCAD.DatabaseServices;
using ZwSoft.ZwCAD.EditorInput;
using ZwSoft.ZwCAD.Geometry;
using ZwSoft.ZwCAD.Runtime;
using Application = ZwSoft.ZwCAD.ApplicationServices.Application;

namespace GeoLib.Commands
{
    public class Cmd_DrawNewPoint
    {
        [CommandMethod("DRAWPOINTSBYPOINTS", CommandFlags.UsePickSet)]
        public void Execute()
        {
            Document mdiActiveDocument = Application.DocumentManager.MdiActiveDocument;
            Editor editor = mdiActiveDocument.Editor;
            Point3d origin = Point3d.Origin;
            TableWindowModel model = null;
            ObjectId objectId = ObjectId.Null;
            // PromptSelectionResult result = editor.SelectImplied();
            // if ((result.Value != null) && (result.Value.Count == 3))
            // {
            //     @null = result.Value[0].ObjectId;
            //     model = TryGetModel(mdiActiveDocument.Database, @null, out origin);
            // }

            string searchFor = "xyz";
            int index = 0;

            var coord = new List<Point3d>();

            while (index < 3)
            {
                PromptEntityResult entity = editor.GetEntity($"Specify point for {searchFor[index]} value");
                if (entity.Status != PromptStatus.OK)
                {
                    return;
                }
                objectId = entity.ObjectId;


                var point = TryGetPoint(mdiActiveDocument.Database, objectId);

                if (point.HasValue)
                {
                    coord.Add(point.Value);
                    index++;
                }
            }

            if (coord.Count == 3)
            {
                var pointToDraw = new MyPoint3D(coord[0].X, coord[1].Y, coord[2].Z, "d");
                CadDrawPoints.Draw(new[] { pointToDraw }, color: Color.FromColor(System.Drawing.Color.Orange), deleteLayerIfExist: false);
            }
        }

        private Point3d? TryGetPoint(Database database, ObjectId objectId)
        {
            using (Transaction acTrans = database.TransactionManager.StartTransaction())
            {
                var point = acTrans.GetObject(objectId, OpenMode.ForRead) as DBPoint;
                if (point != null)
                {
                    return point.Position;
                }
            }

            return null;
        }


    }
}