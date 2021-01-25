

namespace GeoLib
{
    using ZwSoft.ZwCAD.ApplicationServices;
    using ZwSoft.ZwCAD.DatabaseServices;
    using ZwSoft.ZwCAD.Geometry;
    using ZwSoft.ZwCAD.Runtime;
    using GeoLib.XData;
    using System;

    public class Cmd_Test2
    {
        [CommandMethod("TESTCMD2", CommandFlags.UsePickSet)]
        public void NewTestCmd()
        {
            Document mdiActiveDocument = ZwSoft.ZwCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            XDataUtils.EnsureApp(mdiActiveDocument.Database);
            ObjectId @null = ObjectId.Null;
            using (Transaction transaction = mdiActiveDocument.Database.TransactionManager.StartTransaction())
            {
                Line entity = new Line {
                    StartPoint = new Point3d(0.0, 0.0, 0.0),
                    EndPoint = new Point3d(100.0, 100.0, 0.0)
                };
                LineData data1 = new LineData();
                data1.A = 0x22;
                entity.XData = XDataSerializer.Serialize(data1);
                @null = (transaction.GetObject(SymbolUtilityServices.GetBlockModelSpaceId(mdiActiveDocument.Database), OpenMode.ForWrite) as BlockTableRecord).AppendEntity(entity);
                transaction.AddNewlyCreatedDBObject(entity, true);
                transaction.Commit();
            }
            using (Transaction transaction2 = mdiActiveDocument.Database.TransactionManager.StartTransaction())
            {
                XDataSerializer.Deserialize(transaction2.GetObject(@null, OpenMode.ForRead).XData);
                transaction2.Commit();
            }
        }
    }
}

