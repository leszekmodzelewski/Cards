using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media.Media3D;
using GeoLib.Winforms;
using PointCalc;

using MessageBox = System.Windows.MessageBox;

namespace GeoLib.Entities.Table
{
    using ZwSoft.ZwCAD.DatabaseServices;
    using ZwSoft.ZwCAD.Runtime;
    using System;

    public class Cmd_TblCalculate
    {
        [CommandMethod("TABLECALC", CommandFlags.UsePickSet)]
        public void TblCalculate()
        {
            using (Winforms.CalculateDataWindow dlg = new CalculateDataWindow())
            {
                var res = dlg.ShowDialog();
                if (res == DialogResult.OK)
                {
                    FileCoordsDataUtils.ReadPointsFromFile(dlg.FileName, dlg.ScaleFactor);
                    //CalculationUtils.Calculate(Application.DocumentManager.MdiActiveDocument.Database, dlg.FileName);

                }
            }
        }
    }

    internal static class FileCoordsDataUtils
    {
        private static List<Point3D> pointsFromFile = new List<Point3D>();

        internal static void ReadPointsFromFile(string filePath, int scaleFactor)
        {
            LoadPointsFromFilePath(filePath, scaleFactor);
        }


        internal static List<Point3D> Points => pointsFromFile;

        private static void LoadPointsFromFilePath(string filePath, int scaleFactor)
        {
            if (!File.Exists(filePath))
            {
                MessageBox.Show("File not found!");
                return;
            }

            pointsFromFile.Clear();

            var textFromFile = File.ReadAllText(filePath);

            var lines = textFromFile.Split(System.Environment.NewLine.ToCharArray());
            foreach (var lineWithPoints in lines)
            {
                var lineValues = lineWithPoints.Split(',');
                if(lineValues.Length != 4)
                    continue;

                var x = double.Parse(lineValues[1].Trim(), CultureInfo.InvariantCulture) * scaleFactor;
                var y = double.Parse(lineValues[2].Trim(), CultureInfo.InvariantCulture) * scaleFactor;
                var z = double.Parse(lineValues[3].Trim(), CultureInfo.InvariantCulture) * scaleFactor;

                pointsFromFile.Add(new Point3D(x, y, z));
            }

            using (var dlg = new ImportedFilesWindow())
            {
                dlg.FillListBox(pointsFromFile);
                dlg.ShowDialog();
            }
        }
    }

    internal static class CalculationUtils
    {
        public static void Calculate(Database database, string dlgFileName)
        {
            // TODO: DKO: Temp
            try
            {
                ExportToFile(database);
            }
            catch (System.Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }


        public static void ExportToFile(Database database)
        {
            List<ObjectId> coords = new List<ObjectId>();


            var sb = new StringBuilder();
            using (Transaction transaction = database.TransactionManager.StartTransaction())
            {
                foreach (ObjectId id2 in transaction.GetObject(SymbolUtilityServices.GetBlockModelSpaceId(database), OpenMode.ForWrite) as BlockTableRecord)
                {
                    EntityBase base2 = EntityFactory.Create(transaction.GetObject(id2, OpenMode.ForWrite));
                    if (base2 is EntityTable eTable)
                    {
                        var xyz = GetXYZ(database, eTable);
                        sb.Append(id2.Handle.Value).Append(";").Append(xyz[0]).Append(";").Append(xyz[1]).Append(";").Append(xyz[2]).Append(Environment.NewLine);

                        coords.Add(id2);
                    }
                }
                transaction.Commit();
            }

            File.WriteAllText(@"D:\Temp\Coord.txt", sb.ToString().TrimEnd());


            PointCalculator pc = new PointCalculator();
            var res = pc.Calculate();

            Debug.Assert(res.Count == coords.Count);

            UpdateCadEntity(coords, res, database);


        }

        private static void UpdateCadEntity(List<ObjectId> coords, List<MatchedPoint> res, Database database)
        {
            using (Transaction transaction = database.TransactionManager.StartTransaction())
            {
                foreach (ObjectId id2 in transaction.GetObject(SymbolUtilityServices.GetBlockModelSpaceId(database), OpenMode.ForWrite) as BlockTableRecord)
                {
                    if (!coords.Contains(id2))
                        continue;

                    EntityBase base2 = EntityFactory.Create(transaction.GetObject(id2, OpenMode.ForWrite));
                    if (base2 is EntityTable eTable)
                    {
                        int index = coords.IndexOf(id2);

                        UpdateAboutRealValues(database, eTable, res[index]);

                    }
                }
                transaction.Commit();
            }
        }

        private static void UpdateAboutRealValues(Database database, EntityTable eTable, MatchedPoint realPoint)
        {
            Transaction topTransaction = database.TransactionManager.TopTransaction;
            TableUtils.RecalculateTableContent(database, eTable, out double? nullable, out double? nullable2, out double? nullable3);
            foreach (ObjectId id in eTable.Entity.AttributeCollection)
            {
                AttributeReference attRef = (AttributeReference)topTransaction.GetObject(id, OpenMode.ForWrite);
                if (attRef.Tag == "X_2")
                {
                    if (realPoint.RealPoint != null)
                    {
                        EntityBaseUtils.UpdateNullableDoubleAttribute(attRef, realPoint.RealPoint.X);
                    }
                    else
                    {
                        attRef.TextString = "Missing";
                    }
                }
                if (attRef.Tag == "Y_2")
                {
                    if (realPoint.RealPoint != null)
                    {
                        EntityBaseUtils.UpdateNullableDoubleAttribute(attRef, realPoint.RealPoint.Y);
                    }
                    else
                    {
                        attRef.TextString = "Missing";
                    }
                }
                if (attRef.Tag == "Z_2")
                {
                    if (realPoint.RealPoint != null)
                    {
                        EntityBaseUtils.UpdateNullableDoubleAttribute(attRef, realPoint.RealPoint.Z);
                    }
                    else
                    {
                        attRef.TextString = "Missing";
                    }
                }
            }
        }

        private static string[] GetXYZ(Database database, EntityTable eTable)
        {
            string[] xyz = new string[3];



            Transaction topTransaction = database.TransactionManager.TopTransaction;
            TableUtils.RecalculateTableContent(database, eTable, out double? nullable, out double? nullable2, out double? nullable3);
            foreach (ObjectId id in eTable.Entity.AttributeCollection)
            {
                AttributeReference attRef = (AttributeReference)topTransaction.GetObject(id, OpenMode.ForRead);
                if (attRef.Tag == "X_1")
                {
                    xyz[0] = attRef.TextString;
                }
                if (attRef.Tag == "Y_1")
                {
                    xyz[1] = attRef.TextString;
                }
                if (attRef.Tag == "Z_1")
                {
                    xyz[2] = attRef.TextString;
                }

                //if (attRef.Tag == "X_3")
                //{
                //    attRef.TextString = "x3";
                //}
            }




            return xyz;
        }
    }
}