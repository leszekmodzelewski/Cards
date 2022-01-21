using System.Linq;
using GeoLib.Logic;
using PointCalc;

namespace GeoLib.Entities.Table
{
    
    using ZwSoft.ZwCAD.Runtime;
    using System;


    public class Cmd_TableRegen : Cmd_TableBase
    {
        [CommandMethod("TABLEREGEN", CommandFlags.UsePickSet)]
        public void TableRegen()
        {
            Refresh();
        }

        public static void Refresh()
        {
            if (Points.MatchedPoints != null && Points.MatchedPoints.Any())
            {
                TableUtils.RegenerateAllTables(ZwSoft.ZwCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument
                    .Database);

                Points.TheoryPoints = CalculationUtils.ReadTheoryPointsFromCad().ToArray();
                PointCalculator pc = new PointCalculator();
                Points.MatchedPoints = pc.Calculate(Points.TheoryPoints, Points.RealPoints,
                    string.IsNullOrEmpty(Points.MaxErrorFit)
                        ? Double.MaxValue
                        : int.Parse(Points.MaxErrorFit));

                CalculationUtils.UpdateCadEntity(Points.MatchedPoints,
                    ZwSoft.ZwCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Database,
                    Points.OffsetToRealPointForDisplayPurposeOnly);
            }
            else
            {
                TableUtils.RegenerateAllTables(ZwSoft.ZwCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument
                    .Database);

                CalculationUtils.Clean_X_4(ZwSoft.ZwCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Database);
            }
        }
    }
}

