using GeoLib.Logic;
using GeoLib.Winforms;
using PointCalc;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media.Media3D;
using MessageBox = System.Windows.MessageBox;

namespace GeoLib.Entities.Table
{
    using System;
    using ZwSoft.ZwCAD.DatabaseServices;
    using ZwSoft.ZwCAD.Runtime;
    

    public class Cmd_TblCalculate
    {
        [CommandMethod("TABLECALC", CommandFlags.UsePickSet)]
        public void TblCalculate()
        {
            using (Winforms.CalculateDataWindow dlg = new CalculateDataWindow())
            {
                //var res = dlg.ShowDialog();
                //if (res == DialogResult.OK)
                {
                    FileCoordsDataUtils.ReadPointsFromFile(dlg.FileName, dlg.ScaleFactor);
                    CalculationUtils.Calculate(ZwSoft.ZwCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Database, dlg.FileName);

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
                if (lineValues.Length != 4)
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
            var coords = new List<long>();


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

                        coords.Add(id2.Handle.Value);
                    }
                }
                transaction.Commit();
            }

            string filePath = Path.Combine(Path.GetTempPath(), "Coord.txt");
            File.WriteAllText(filePath, sb.ToString().TrimEnd());


            PointCalculator pc = new PointCalculator();
            //var res = pc.Calculate();

            //Debug.Assert(res.Count == coords.Count);

            // UpdateCadEntity(coords, res, database);


        }

        public static List<MyPoint3D> ReadTheoryPointsFromCad()
        {
            
          
                return CalculationUtils.ReadFromCad(ZwSoft.ZwCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Database);
            
        }

        public static void Clean_X_4(Database database)
        {
            return;
            using (Transaction transaction = database.TransactionManager.StartTransaction())
            {
                foreach (ObjectId id2 in transaction.GetObject(SymbolUtilityServices.GetBlockModelSpaceId(database), OpenMode.ForRead) as BlockTableRecord)
                {
                    EntityBase base2 = EntityFactory.Create(transaction.GetObject(id2, OpenMode.ForRead));
                    if (base2 is EntityTable eTable)
                    {
                        foreach (ObjectId id in eTable.Entity.AttributeCollection)
                        {
                            AttributeReference attRef = (AttributeReference)transaction.GetObject(id, OpenMode.ForRead);
                            if (attRef.Tag == "X_4")
                            {
                                attRef.TextString = string.Empty;
                            }
                            if (attRef.Tag == "Y_4")
                            {
                                attRef.TextString = string.Empty;
                            }
                            if (attRef.Tag == "Z_4")
                            {
                                attRef.TextString = string.Empty;
                            }
                        }
                    }
                }
                transaction.Commit();
            }
        }

        private static List<MyPoint3D> ReadFromCad(Database database)
        {
            var coords = new List<long>();
            List<MyPoint3D> cadPoints = new List<MyPoint3D>();


            using (Transaction transaction = database.TransactionManager.StartTransaction())
            {
                foreach (ObjectId id2 in transaction.GetObject(SymbolUtilityServices.GetBlockModelSpaceId(database), OpenMode.ForRead) as BlockTableRecord)
                {
                    EntityBase base2 = EntityFactory.Create(transaction.GetObject(id2, OpenMode.ForRead));
                    if (base2 is EntityTable eTable)
                    {
                        var xyz = GetXYZ(database, eTable);

                        double x = double.Parse(xyz[0]);
                        double y = double.Parse(xyz[1]);
                        double z = double.Parse(xyz[2]);

                        cadPoints.Add(new MyPoint3D(new Point3D(x, y, z), id2.Handle.Value.ToString(CultureInfo.InvariantCulture)));

                        coords.Add(id2.Handle.Value);
                    }
                }
                transaction.Commit();
            }

            return cadPoints;
            //string filePath = Path.Combine(Path.GetTempPath(), "Coord.txt");
            //File.WriteAllText(filePath, sb.ToString().TrimEnd());


            //PointCalculator pc = new PointCalculator();
            //var res = pc.Calculate();

            //Debug.Assert(res.Count == coords.Count);

            //UpdateCadEntity(coords, res, database);


        }
        private static List<MyPoint3D> ReadFromA()
        {
            
            List<MyPoint3D> cadPoints = new List<MyPoint3D>();
            int a = Points.TheoryPoints.ToArray().Length;

            for (int i = 0; i < a; i++)
            {
                cadPoints.Add(new MyPoint3D(new Point3D(Points.TheoryPoints.ElementAt(i).X, Points.TheoryPoints.ElementAt(i).Y, Points.TheoryPoints.ElementAt(i).Z),i.ToString()));
            }

            //MessageBox.Show(cadPoints.ElementAt(0).ToString(), "Do wyjebania");

            return cadPoints;
        }
            public static void UpdateCadRangeOnlyEntity(Database database, MyPoint3D[] theoryPoints)
        {
            using (Transaction transaction = database.TransactionManager.StartTransaction())
            {
                foreach (ObjectId id2 in transaction.GetObject(SymbolUtilityServices.GetBlockModelSpaceId(database),
                    OpenMode.ForWrite) as BlockTableRecord)
                {
                    var coordinate = theoryPoints.FirstOrDefault(m =>
                        m.Id == id2.Handle.Value.ToString(CultureInfo.InvariantCulture));
                    if (coordinate == null)
                        continue;

                    EntityBase base2 = EntityFactory.Create(transaction.GetObject(id2, OpenMode.ForWrite));
                    if (base2 is EntityTable eTable)
                    {
                        Transaction topTransaction = database.TransactionManager.TopTransaction;
                        foreach (ObjectId id in eTable.Entity.AttributeCollection)
                        {
                            AttributeReference attRef = (AttributeReference)topTransaction.GetObject(id, OpenMode.ForWrite);
                            UpdateRange(coordinate, attRef);
                        }
                    }
                }
                transaction.Commit();
            }
        }




        public static void UpdateCadEntity(List<MatchedPoint> res, Database database, int[] extraOffset)
        {
            int missingPointCounter = 0;
            int updatedPointsCount = 0;
            using (Transaction transaction = database.TransactionManager.StartTransaction())
            {
                foreach (ObjectId id2 in transaction.GetObject(SymbolUtilityServices.GetBlockModelSpaceId(database), OpenMode.ForWrite) as BlockTableRecord)
                {
                    var coordinate = res.FirstOrDefault(m => m.TheoryPoint?.Id == id2.Handle.Value.ToString(CultureInfo.InvariantCulture));
                    if (coordinate == null)
                        continue;

                    EntityBase base2 = EntityFactory.Create(transaction.GetObject(id2, OpenMode.ForWrite));
                    if (base2 is EntityTable eTable)
                    {
                        UpdateAboutRealValues(database, eTable, coordinate, extraOffset);

                        updatedPointsCount++;
                        if (coordinate.RealPoint == null)
                            missingPointCounter++;

                    }


                }
                transaction.Commit();
            }

            if (missingPointCounter == updatedPointsCount)
            {
                MessageBox.Show("No matched points.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }


        private static void UpdateAboutRealValues(Database database, EntityTable eTable, MatchedPoint point, int[] extraOffset)
        {
            Transaction topTransaction = database.TransactionManager.TopTransaction;
            TableUtils.RecalculateTableContent(database, eTable, out double? nullable, out double? nullable2, out double? nullable3);
            foreach (ObjectId id in eTable.Entity.AttributeCollection)
            {
                AttributeReference attRef = (AttributeReference)topTransaction.GetObject(id, OpenMode.ForWrite);
                const string missingText = "";

                if (attRef.Tag == "X_2")
                {
                    if (point.RealPoint != null)
                    {
                        EntityBaseUtils.UpdateNullableDoubleAttribute(attRef, point.RealPoint.X + extraOffset[0]);
                    }
                    else
                    {
                        attRef.TextString = missingText;
                    }
                }
                if (attRef.Tag == "Y_2")
                {
                    if (point.RealPoint != null)
                    {
                        EntityBaseUtils.UpdateNullableDoubleAttribute(attRef, point.RealPoint.Y + extraOffset[1]);
                    }
                    else
                    {
                        attRef.TextString = missingText;
                    }
                }
                if (attRef.Tag == "Z_2")
                {
                    if (point.RealPoint != null)
                    {
                        EntityBaseUtils.UpdateNullableDoubleAttribute(attRef, point.RealPoint.Z + extraOffset[2]);
                    }
                    else
                    {
                        attRef.TextString = missingText;
                    }
                }

                //UpdateRange(point.TheoryPoint, attRef);
                UpdateRealMinusTheory(point, attRef, extraOffset);
            }
        }

        private static void UpdateRange(MyPoint3D theoryPoint, AttributeReference attRef)
        {
            if (attRef.Tag == "X_3")
            {
                EntityBaseUtils.UpdateRangeAttribute(attRef, Points.GetRangeForX(theoryPoint.Xo));
            }

            if (attRef.Tag == "Y_3")
            {
                EntityBaseUtils.UpdateRangeAttribute(attRef, Points.GetRangeForY(theoryPoint.Yo));
            }

            if (attRef.Tag == "Z_3")
            {
                EntityBaseUtils.UpdateRangeAttribute(attRef, Points.GetRangeForZ(theoryPoint.Zo));
            }
        }



        private static void UpdateRealMinusTheory(MatchedPoint matchedPoint, AttributeReference attRef, int[] extraOffset)
        {
            if (matchedPoint.RealPoint == null)
                return;

            if (attRef.Tag == "X_4")
            {
                EntityBaseUtils.UpdateNullableDoubleAttribute(attRef, ValueToSet(matchedPoint.RealPoint.X + +extraOffset[0], matchedPoint.TheoryPoint.X));
            }

            if (attRef.Tag == "Y_4")
            {
                EntityBaseUtils.UpdateNullableDoubleAttribute(attRef, ValueToSet(matchedPoint.RealPoint.Y + extraOffset[1], matchedPoint.TheoryPoint.Y));
            }

            if (attRef.Tag == "Z_4")
            {
                EntityBaseUtils.UpdateNullableDoubleAttribute(attRef, matchedPoint.RealPoint.Z - matchedPoint.TheoryPoint.Z + extraOffset[2]);
            }
        }

        private static double ValueToSet(double real, double theory)
        {
            double res;
            if (real < 0)
            {
                res = theory - real;
            }
            else
            {
                res = real - theory;
            }

            return res;
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