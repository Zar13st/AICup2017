using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;
using System;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk
{
    public static class MyGridPoinExtension
    {
        #region Public Methods

        public static double Distance(this MyGridCellInfo cell, MySquad squad)
        {
            return Math.Sqrt(Math.Pow(cell.X - squad.X, 2) + Math.Pow(cell.Y - squad.Y, 2));
        }

        public static double Distance(this MyGridCellInfo cell, MyGridCellInfo cell2)
        {
            return Math.Sqrt(Math.Pow(cell.X - cell2.X, 2) + Math.Pow(cell.Y - cell2.Y, 2));
        }

        public static double Distance(this MySquad s, Facility f)
        {
            return Math.Sqrt(Math.Pow(s.X - (f.Left + 32), 2) + Math.Pow(s.Y - (f.Top + 32), 2));
        }

        public static double Distance(this Facility f, double x, double y)
        {
            return Math.Sqrt(Math.Pow(f.Left + 32 - x, 2) + Math.Pow(f.Top + 8 - y, 2));
        }

        public static int GetCellX(this Vehicle vehicle)
        {
            return Convert.ToInt32(vehicle.X) / MyGameGrid.GameGridDelta;
        }

        public static int GetCellX(this Facility facility)
        {
            return Convert.ToInt32(facility.Left) / MyGameGrid.GameGridDelta;
        }

        public static int GetCellX(this double x)
        {
            return Convert.ToInt32(x) / MyGameGrid.GameGridDelta;
        }

        public static int GetCellY(this Vehicle vehicle)
        {
            return Convert.ToInt32(vehicle.Y) / MyGameGrid.GameGridDelta;
        }

        public static int GetCellY(this Facility facility)
        {
            return Convert.ToInt32(facility.Top) / MyGameGrid.GameGridDelta;
        }

        public static int GetCellY(this double y)
        {
            return Convert.ToInt32(y) / MyGameGrid.GameGridDelta;
        }

        public static MyPoint GetCenter(this Facility facility)
        {
            var center = new MyPoint()
            {
                X = facility.Left + 32,
                Y = facility.Top + 32,
            };

            return center;
        }

        public static bool IsGroundSquad(this MySquad squad)
        {
            return squad.VehicleType == VehicleType.Tank ||
                   squad.VehicleType == VehicleType.Arrv ||
                   squad.VehicleType == VehicleType.Ifv;
        }

        public static bool OnMyGround(this Facility facility)
        {
            return facility.Top + facility.Left < 1024;
        }

        #endregion Public Methods
    }
}