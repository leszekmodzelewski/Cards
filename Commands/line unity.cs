using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using ZwSoft.ZwCAD.ApplicationServices;
using ZwSoft.ZwCAD.DatabaseServices;
using ZwSoft.ZwCAD.EditorInput;
using ZwSoft.ZwCAD.Geometry;
using ZwSoft.ZwCAD.Runtime;
using System.Windows.Forms;
using Application = ZwSoft.ZwCAD.ApplicationServices.Application;
using PointCalc;
using System.Globalization;
using GeoLib.Logic;
//using GeoLib.Geometry;
//using GeoLib.Solvers;


namespace GeoLib.Entities.RectBlanking
{
    public class FitCylinderCommand
    {
        static List<MyPoint3D> theoryPoints = new List<MyPoint3D>();
        static Point3d theoryPoints2 = new Point3d();
        [CommandMethod("FC")]
        public void ExportToFile()
        {
            
            //FitCylinder();

        }
      
        //public void FitCylinder()
        //{
        //    //pobierz punkty

        //    Document doc = Application.DocumentManager.MdiActiveDocument;
        //    Database db = doc.Database;

        //    List<Point3d> pointList = new List<Point3d>(); // lista punktów

        //    using (Transaction trans = db.TransactionManager.StartTransaction())
        //    {
        //        BlockTable bt = trans.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;

        //        foreach (ObjectId btrId in bt)
        //        {
        //            BlockTableRecord btr = trans.GetObject(btrId, OpenMode.ForRead) as BlockTableRecord;

        //            foreach (ObjectId entId in btr)
        //            {
        //                Entity ent = trans.GetObject(entId, OpenMode.ForRead) as Entity;

        //                if (ent is DBPoint)
        //                {
        //                    DBPoint point = ent as DBPoint;

        //                    // Add point coordinates to the pointList
        //                    pointList.Add(point.Position);
        //                }
        //            }
        //        }
        //    }
                        
        //    string v = "\nWybierz linie cylindra:";

        //    // Pobranie linii od użytkownika
        //    PromptEntityResult lineSelectionResult = GetEntity(v);
           
        //    Line line = new Line();

        //    using (Transaction trans = db.TransactionManager.StartTransaction())
        //    {
        //        BlockTable bt = trans.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;

        //        foreach (ObjectId btrId in bt)
        //        {
        //            BlockTableRecord btr = trans.GetObject(btrId, OpenMode.ForRead) as BlockTableRecord;

        //            foreach (ObjectId entId in btr)
        //            {
        //                Entity ent = trans.GetObject(entId, OpenMode.ForRead) as Entity;

        //                if (ent is Line)
        //                {
        //                    line = ent as Line;

        //                    // Do something with the line
        //                    // For example, get the start and end point coordinates
        //                    Point3d startPt = line.StartPoint;
        //                    Point3d endPt = line.EndPoint;
        //                }
        //            }
        //        }
        //    }
        //    if (lineSelectionResult.Status != PromptStatus.OK) return;

        //    // Przygotowanie linii
            
        //    // Dopasowanie cylindra
                      
        //    Vector<double> cylinderCenter = FitCylinderToPoints(pointList.ToArray());
            
        //    // Obliczenie wektora kierunku osi cylindra

        //    Vector3d axisDirection = new Vector3d(cylinderCenter[3], cylinderCenter[4], cylinderCenter[5]);


        //    // Obliczenie punktu na linii osi cylindra, najbliższego do środka
        //    Point3d closestPointOnLine = GetClosestPointOnLine(line, cylinderCenter[0], cylinderCenter[1], cylinderCenter[2], axisDirection);

        //    MessageBox.Show(axisDirection.X.ToString()+ axisDirection.Y.ToString()+ axisDirection.Z.ToString());

        //    // Rysowanie linii osi cylindra
        //    using (Transaction tr = HostApplicationServices.WorkingDatabase.TransactionManager.StartTransaction())
        //    {
        //        BlockTableRecord btr = (BlockTableRecord)tr.GetObject(SymbolUtilityServices.GetBlockModelSpaceId(HostApplicationServices.WorkingDatabase), OpenMode.ForWrite);

        //        Vector3d axisDirectionVector = new Vector3d(axisDirection.X, axisDirection.Y, axisDirection.Z);
        //        Line cylinderAxis = new Line(closestPointOnLine, closestPointOnLine + axisDirectionVector);
        //        btr.AppendEntity(cylinderAxis);
        //        tr.AddNewlyCreatedDBObject(cylinderAxis, true);

        //        tr.Commit();
        //    }
        //}
                

        private PromptEntityResult GetEntity(string message)
        {
            PromptEntityOptions entityOptions = new PromptEntityOptions(message);
            entityOptions.AllowNone = false;
            return Application.DocumentManager.MdiActiveDocument.Editor.GetEntity(entityOptions);
        }

        //private Vector<double> FitCylinderToPoints(Point3d[] points)
        //{
           
        //}

        private Point3d GetClosestPointOnLine(Line line, double x, double y, double z, Vector3d direction)
        {
            Vector3d vectorToCenter = new Vector3d(x - line.StartPoint.X, y - line.StartPoint.Y, z - line.StartPoint.Z);

            double projectionLength = vectorToCenter.DotProduct(direction) / (direction.X * direction.X + direction.Y * direction.Y + direction.Z * direction.Z);

            Point3d closestPointOnLine;

            if (projectionLength < 0)
            {
                closestPointOnLine = line.StartPoint;
            }
            else if (projectionLength > 1)
            {
                closestPointOnLine = line.EndPoint;
            }
            else
            {
                closestPointOnLine = line.StartPoint + projectionLength * direction;
            }

            return closestPointOnLine;
        }
        //public class Ransac<T>
        //{
        //    private IModel<T> model;
        //    private IDistanceMeasure<T> distanceMeasure;
        //    private double inlierThreshold;
        //    private double confidence;
        //    private int maxIterations;

        //    public Ransac(IModel<T> model, IDistanceMeasure<T> distanceMeasure, double inlierThreshold, double confidence, int maxIterations)
        //    {
        //        this.model = model;
        //        this.distanceMeasure = distanceMeasure;
        //        this.inlierThreshold = inlierThreshold;
        //        this.confidence = confidence;
        //        this.maxIterations = maxIterations;
        //    }

        //    public T FindBestFit(List<T> data)
        //    {
        //        T bestFit = default(T);
        //        int numInliers = 0;

        //        for (int i = 0; i < maxIterations; i++)
        //        {
        //            var sample = GetRandomSample(data);
        //            var fit = model.Fit(sample);
        //            var inliers = GetInliers(data, fit);

        //            if (inliers.Count > numInliers)
        //            {
        //                numInliers = inliers.Count;
        //                bestFit = fit;
        //            }

        //            var inlierRatio = (double)inliers.Count / data.Count;

        //            if (inlierRatio >= confidence)
        //            {
        //                break;
        //            }
        //        }

        //        return bestFit;
        //    }

        //    //private List<T> GetRandomSample(List<T> data)
        //    //{
        //    //    var sample = new List<T>();

        //    //    while (sample.Count < model.NumSamples)
        //    //    {
        //    //        var randomIndex = new Random().Next(data.Count);
        //    //        var randomPoint = data[randomIndex];

        //    //        if (!sample.Contains(randomPoint))
        //    //        {
        //    //            sample.Add(randomPoint);
        //    //        }
        //    //    }

        //    //    return sample;
        //    //}

        //    private List<T> GetInliers(List<T> data, T fit)
        //    {
        //        var inliers = new List<T>();

        //        foreach (var point in data)
        //        {
        //            var distance = distanceMeasure.GetDistance(point, fit);

        //            if (distance < inlierThreshold)
        //            {
        //                inliers.Add(point);
        //            }
        //        }

        //        return inliers;
        //    }
        //}

        public interface IModel<T>
        {
            int NumSamples { get; }

            T Fit(List<T> data);
        }

        public class CylinderModel : IModel<Point3d>
        {
            public int NumSamples => 3;

            public Point3d Fit(List<Point3d> data)
            {
                // implement cylinder fitting algorithm here
                return new Point3d(0, 0, 0);
            }
        }

        public interface IDistanceMeasure<T>
        {
            double GetDistance(T point, T fit);
        }

        public class CylinderDistanceMeasure : IDistanceMeasure<Point3d>
        {
            public double GetDistance(Point3d point, Point3d fit)
            {
                // implement cylinder distance measure algorithm here
                return 0.0;
            }
        }
    }
    }
    
