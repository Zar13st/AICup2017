using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk
{
    public class MyGameGrid
    {
        #region Public Fields

        public const int GameGridDelta = 64;

        #endregion Public Fields

        #region Private Fields

        private readonly int _cellCount;
        private readonly MyStrategy _strategy;

        #endregion Private Fields

        #region Public Constructors

        public MyGameGrid(MyStrategy strategy)
        {
            _strategy = strategy;

            _cellCount = Convert.ToInt32(strategy.World.Width) / GameGridDelta;

            Grid = new MyGridCellInfo[_cellCount, _cellCount];

            for (var i = 0; i < _cellCount; i++)
            {
                for (var j = 0; j < _cellCount; j++)
                {
                    Grid[i, j] = new MyGridCellInfo(i, j);
                }
            }
        }

        #endregion Public Constructors

        #region Public Properties

        public MyGridCellInfo[,] Grid { get; }

        #endregion Public Properties

        #region Public Methods

        public MyPoint GetPosition(int id)
        {
            var squad = _strategy.GroupManager.Squads.SingleOrDefault(s => s.Id == id);

            if (squad == null) return new MyPoint(512, 512);

            return new MyPoint((squad.X + 0.5) * GameGridDelta, (squad.Y + 0.5) * GameGridDelta);
        }

        public void Update(IEnumerable<Vehicle> vehicles, Facility[] facilities)
        {
            Clear();

            foreach (var squad in _strategy.GroupManager.Squads)
            {
                TakePosition(squad);
            }

            foreach (var facility in facilities)
            {
                SetupFacilitiesInfo(facility);
            }

            foreach (var v in vehicles)
            {
                UpdateVehicles(v);
            }
        }

        #endregion Public Methods

        #region Private Methods

        private void Clear()
        {
            for (var i = 0; i < _cellCount; i++)
            {
                for (var j = 0; j < _cellCount; j++)
                {
                    Grid[i, j].Clear();
                }
            }
        }

        private void SetupFacilitiesInfo(Facility facility)
        {
            Grid[facility.GetCellX(), facility.GetCellY()].Facility = facility;
        }

        private void TakePosition(MySquad squad)
        {
            var center = _strategy.MyVehicles.Where(v => v.Groups.Contains(squad.Id)).CenterXY();

            squad.X = center.X.GetCellX();
            squad.Y = center.Y.GetCellY();
        }

        private void UpdateVehicles(Vehicle vehicles)
        {
            var cellX = vehicles.GetCellX();
            var cellY = vehicles.GetCellY();

            var cellInfo = Grid[cellX, cellY];
            cellInfo.IsEnemyCell = true;

            switch (vehicles.Type)
            {
                case VehicleType.Arrv:
                    cellInfo.EnemyRemonts.Add(vehicles.Id);
                    break;

                case VehicleType.Fighter:
                    cellInfo.EnemySamolets.Add(vehicles.Id);
                    break;

                case VehicleType.Helicopter:
                    cellInfo.EnemyCopters.Add(vehicles.Id);
                    break;

                case VehicleType.Ifv:
                    cellInfo.EnemyZeneitkas.Add(vehicles.Id);
                    break;

                case VehicleType.Tank:
                    cellInfo.EnemyTanks.Add(vehicles.Id);
                    break;
            }
        }

        #endregion Private Methods
    }
}