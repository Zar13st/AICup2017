using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;
using System;
using System.Collections;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk
{
    public static class MyGroupCenterExtension
    {
        #region Public Methods

        public static MyPoint CenterXY(this IEnumerable vehicles)
        {
            double sumX = 0;
            double sumY = 0;
            int count = 0;

            foreach (Vehicle vehicle in vehicles)
            {
                sumX += vehicle.X;
                sumY += vehicle.Y;
                count++;
            }

            if (count == 0)
            {
                count = 1;
            }

            var center = new MyPoint()
            {
                X = sumX / count,
                Y = sumY / count,
            };

            return center;
        }

        public static double GetDistance(this MyPoint p1, MyPoint p2)
        {
            return Math.Sqrt(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2));
        }

        public static double GetSqrDistance(this MyPoint p1, MyPoint p2)
        {
            return Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2);
        }

        public static bool Ravno(this MyPoint point1, MyPoint point2)
        {
            return point1.X == point2.X && point1.Y == point2.Y;
        }

        public static void Swap(this Group[] groups, int a, int b)
        {
            var temp = groups[a];
            groups[a] = groups[b];
            groups[b] = temp;
        }

        public static Group ToGroup(this VehicleType vehicleType)
        {
            return (Group)((int)vehicleType + 1);
        }

        public static VehicleType ToVehicleType(this Group group)
        {
            if ((int)group > 5)
            {
                return VehicleType.Arrv;
            }
            return (VehicleType)((int)group - 1);
        }

        #endregion Public Methods
    }
}