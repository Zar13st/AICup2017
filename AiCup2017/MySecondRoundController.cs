using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk
{
    public class MySecondRoundController : MyIStrategyController
    {
        #region Private Fields

        private Dictionary<double, MyGridCellInfo> _airTargets = new Dictionary<double, MyGridCellInfo>();
        private int _process = 0;
        private MyStrategy _str;

        #endregion Private Fields

        #region Public Constructors

        public MySecondRoundController(MyStrategy strategy)
        {
            _str = strategy;
        }

        #endregion Public Constructors

        #region Public Methods

        public void FindSingleEnemy(MySquad squad)
        {
            var myRandomUnit = _str.MyVehicles.FirstOrDefault(v => v.Groups.Contains(squad.Id));
            if (myRandomUnit == null || !_str.EnemyVehicles.Any()) return;
            var nearestEnemyPoint = new MyPoint();
            double distance = 10000;
            IEnumerable<Vehicle> targets;

            var fac = _str.World.Facilities.Where(f => f.OwnerPlayerId != _str.Me.Id && f.Type == FacilityType.VehicleFactory);
            if (!fac.Any())
            {
                fac = _str.World.Facilities.Where(f => f.OwnerPlayerId != _str.Me.Id && f.Type == FacilityType.ControlCenter);
            }

            if (!fac.Any())
            {
                targets = _str.EnemyVehicles.Where(v => v.Type != VehicleType.Fighter && v.Type != VehicleType.Helicopter);
                if (!targets.Any()) new MyPoint(512, 512);
                foreach (var vehicle in targets)
                {
                    double currentDistance;
                    try
                    {
                        currentDistance = vehicle.GetDistanceTo(myRandomUnit);
                    }
                    catch (Exception e)
                    {
                        continue;
                    }

                    if (currentDistance < distance)
                    {
                        distance = currentDistance;
                        nearestEnemyPoint.X = vehicle.X;
                        nearestEnemyPoint.Y = vehicle.Y;
                    }
                }
            }
            else
            {
                foreach (var f in fac)
                {
                    double currentDistance = f.Distance(myRandomUnit.X, myRandomUnit.Y);

                    if (currentDistance < distance)
                    {
                        distance = currentDistance;
                        nearestEnemyPoint.X = f.Top + 42;
                        nearestEnemyPoint.Y = f.Left + 32;
                    }
                }
            }

            _str.MainGameTasks.Enqueue(_str.Act.SelectByGroup(squad.Id));
            _str.MainGameTasks.Enqueue(_str.Act.MoveToPoint(nearestEnemyPoint));
        }

        public MyPoint FindSingleEnemy(int group)
        {
            var myRandomUnit = _str.MyVehicles.FirstOrDefault(v => v.Groups.Contains(group));
            if (myRandomUnit == null) new MyPoint(512, 512);
            var nearestEnemyPoint = new MyPoint();
            double distance = 10000;
            IEnumerable<Vehicle> targets;
            switch (group)
            {
                case (int)Group.Tank:
                    targets = _str.EnemyVehicles.Where(v => v.Type != VehicleType.Fighter && v.Type != VehicleType.Helicopter);
                    break;

                case (int)Group.LandLeft:
                    targets = _str.EnemyVehicles.Where(v => v.Type != VehicleType.Tank && v.Type != VehicleType.Fighter);
                    break;

                case (int)Group.LandTop:
                    targets = _str.EnemyVehicles.Where(v => v.Type != VehicleType.Tank && v.Type != VehicleType.Fighter);
                    break;

                default:
                    targets = _str.EnemyVehicles;
                    break;
            }

            if (targets == null || !targets.Any()) new MyPoint(512, 512);

            foreach (var vehicle in targets)
            {
                double currentDistance;
                try
                {
                    currentDistance = vehicle.GetDistanceTo(myRandomUnit);
                }
                catch (Exception e)
                {
                    continue;
                }

                if (currentDistance < distance)
                {
                    distance = currentDistance;
                    nearestEnemyPoint.X = vehicle.X;
                    nearestEnemyPoint.Y = vehicle.Y;
                }
            }

            return new MyPoint(nearestEnemyPoint.X, nearestEnemyPoint.Y);
        }

        public void Process()
        {
            _process++;
            if (_process == 1)
            {
            }
            else if (_process == 2)
            {
                _str.GroupingEnded = true;
                AirScale();

                TopScale();
                LeftScale();
                TopRScale();
                LeftRScale();
                TanksScale();
            }
        }

        #endregion Public Methods

        #region Private Methods

        private void AirGo()
        {
            IEnumerable<Vehicle> tanks;

            tanks = _str.MyVehicles.Where(v => v.Type == VehicleType.Tank && v.Groups.Contains((int)Group.Tank));
            if (!tanks.Any())
            {
                tanks = _str.MyVehicles.Where(v => v.Type == VehicleType.Tank);
                if (!tanks.Any())
                {
                    tanks = _str.MyVehicles.Where(v => v.Type == VehicleType.Ifv);
                }
            }

            var tagret = tanks.CenterXY();
            _str.MainGameTasks.Enqueue(_str.Act.SelectByGroup((int)Group.Air));
            _str.MainGameTasks.Enqueue(_str.Act.MoveSlowToPoint(new MyPoint(tagret.X, tagret.Y), 0.9d));

            _str.DelayTaksBuilder.Create(Group.Air, AirScale, 400);
        }

        private void AirScale()
        {
            _str.MainGameTasks.Enqueue(_str.Act.SelectByGroup((int)Group.Air));
            _str.MainGameTasks.Enqueue(_str.Act.Scale(0.1));

            _str.DelayTaksBuilder.Create(Group.Air, AirGo, 300);
        }

        private void LandLeftGo()
        {
            var target = _str.IndicatorFacillites.GetNearestFasility(Group.LandLeft);

            _str.MainGameTasks.Enqueue(_str.Act.SelectByGroup((int)Group.LandLeft));
            _str.MainGameTasks.Enqueue(_str.Act.MoveSlowToPoint(new MyPoint(target.X, target.Y + 10), 0.4));

            _str.DelayTaksBuilder.Create(Group.LandLeft, LeftScale, 1200);
        }

        private void LandLeftRGo()
        {
            var target = _str.IndicatorFacillites.GetNearestFasility(Group.LandLeftR);

            _str.MainGameTasks.Enqueue(_str.Act.SelectByGroup((int)Group.LandLeftR));
            _str.MainGameTasks.Enqueue(_str.Act.MoveSlowToPoint(new MyPoint(target.X, target.Y + 10), 0.4));

            _str.DelayTaksBuilder.Create(Group.LandLeftR, LeftRShift, 1600);
        }

        private void LandTanksGo()
        {
            var target = _str.IndicatorFacillites.GetNearestFasility(Group.Tank);

            _str.MainGameTasks.Enqueue(_str.Act.SelectByGroup((int)Group.Tank));
            _str.MainGameTasks.Enqueue(_str.Act.MoveSlowToPoint(new MyPoint(target.X, target.Y + 10), 0.3));

            _str.DelayTaksBuilder.Create(Group.Tank, TanksScale, 1200);
        }

        private void LandTopGo()
        {
            var target = _str.IndicatorFacillites.GetNearestFasility(Group.LandTop);

            _str.MainGameTasks.Enqueue(_str.Act.SelectByGroup((int)Group.LandTop));
            _str.MainGameTasks.Enqueue(_str.Act.MoveSlowToPoint(new MyPoint(target.X, target.Y + 10), 0.4));

            _str.DelayTaksBuilder.Create(Group.LandTop, TopScale, 1200);
        }

        private void LandTopRGo()
        {
            var target = _str.IndicatorFacillites.GetNearestFasility(Group.LandTopR);

            _str.MainGameTasks.Enqueue(_str.Act.SelectByGroup((int)Group.LandTopR));
            _str.MainGameTasks.Enqueue(_str.Act.MoveSlowToPoint(new MyPoint(target.X, target.Y + 10), 0.4));

            _str.DelayTaksBuilder.Create(Group.LandTopR, TopRShift, 1600);
        }

        //private void AirGo()
        //{
        //    //if (!_str.EnemyVehicles.Any() || !_str.MyVehicles.Any()) { return; }

        //    var myRandomUnit = _str.MyVehicles.FirstOrDefault(v => v.Groups.Contains((int)Group.Air));
        //    if (myRandomUnit == null) return;

        //    var nearestEnemyPoint = new MyPoint();
        //    double distance = 10000;
        //    IEnumerable<Vehicle> targets;
        //    if (_str.World.TickIndex < 2000)
        //    {
        //        targets = _str.EnemyVehicles.Where(v => v.Type == VehicleType.Helicopter && v.X < 800 && v.Y < 800);
        //        if (!targets.Any())
        //        {
        //            targets = _str.EnemyVehicles.Where(v => v.Type != VehicleType.Ifv && v.X < 800 && v.Y < 800);
        //        }
        //    }
        //    else
        //    {
        //        targets = _str.EnemyVehicles.Where(v => v.Type == VehicleType.Helicopter);
        //        if (!targets.Any())
        //        {
        //            targets = _str.EnemyVehicles.Where(v => v.Type != VehicleType.Ifv);
        //        }
        //    }

        //    if (!targets.Any())
        //    {
        //        if (even)
        //        {
        //            _str.MainGameTasks.Enqueue(_str.Act.SelectByGroup((int)Group.Air));
        //            _str.MainGameTasks.Enqueue(_str.Act.MoveSlowToPoint(new MyPoint(0, 512), 0.9d));

        //            _str.DelayTaksBuilder.Create(Group.Air, AirRotate);
        //            even = false;
        //        }
        //        else
        //        {
        //            _str.MainGameTasks.Enqueue(_str.Act.SelectByGroup((int)Group.Air));
        //            _str.MainGameTasks.Enqueue(_str.Act.MoveSlowToPoint(new MyPoint(512, 0), 0.9d));

        //            _str.DelayTaksBuilder.Create(Group.Air, AirRotate);
        //            even = true;
        //        }

        //        return;
        //    }

        //    foreach (var vehicle in targets )
        //    {
        //        double currentDistance;
        //        try
        //        {
        //            currentDistance = vehicle.GetDistanceTo(myRandomUnit);
        //        }
        //        catch (Exception e)
        //        {
        //            continue;
        //        }
        //        if (currentDistance < distance)
        //        {
        //            distance = currentDistance;
        //            nearestEnemyPoint.X = vehicle.X;
        //            nearestEnemyPoint.Y = vehicle.Y;
        //        }
        //    }

        //    _str.MainGameTasks.Enqueue(_str.Act.SelectByGroup((int)Group.Air));
        //    _str.MainGameTasks.Enqueue(_str.Act.MoveSlowToPoint(new MyPoint(nearestEnemyPoint.X, nearestEnemyPoint.Y), 0.9d));

        //    _str.DelayTaksBuilder.Create(Group.Air, AirRotate, 450);
        //}
        //private void AirRotate()
        //{
        //    _str.MainGameTasks.Enqueue(_str.Act.SelectByGroup((int)Group.Air));
        //    _str.MainGameTasks.Enqueue(_str.Act.FastCenterRotate(90));

        //    _str.DelayTaksBuilder.Create(Group.Air, AirScale);
        //}

        private void LeftRFirstGo()
        {
            _str.MainGameTasks.Enqueue(_str.Act.SelectByGroup((int)Group.LandLeftR));
            _str.MainGameTasks.Enqueue(_str.Act.MoveToPoint(new MyPoint(0, 350)));

            _str.DelayTaksBuilder.Create(Group.LandLeftR, LeftRShift, 200);
        }

        private void LeftRScale()
        {
            _str.MainGameTasks.Enqueue(_str.Act.SelectByGroup((int)Group.LandLeftR));
            _str.MainGameTasks.Enqueue(_str.Act.Scale(0.1));

            _str.DelayTaksBuilder.Create(Group.LandLeftR, LeftRFirstGo, 200);
        }

        private void LeftRShift()
        {
            _str.MainGameTasks.Enqueue(_str.Act.SelectByGroup((int)Group.LandLeftR));
            _str.MainGameTasks.Enqueue(_str.Act.MoveByVector(new MyPoint(100, 0)));

            _str.DelayTaksBuilder.Create(Group.LandLeftR, LandLeftRGo, 200);
        }

        private void LeftScale()
        {
            _str.MainGameTasks.Enqueue(_str.Act.SelectByGroup((int)Group.LandLeft));
            _str.MainGameTasks.Enqueue(_str.Act.Scale(0.1));

            var myV = _str.MyVehicles.Where(v => v.Groups.Contains((int)Group.LandTop));
            if (myV == null || !myV.Any())
            {
                return;
            }
            var centerV = myV.CenterXY();
            if (_str.IndicatorFacillites.OnFacility(centerV))
            {
                _str.DelayTaksBuilder.Create(Group.LandTop, LandLeftGo, 200);
            }
            else
            {
                _str.DelayTaksBuilder.Create(Group.LandLeft, LeftShift, 200);
            }
        }

        private void LeftShift()
        {
            _str.MainGameTasks.Enqueue(_str.Act.SelectByGroup((int)Group.LandLeft));
            _str.MainGameTasks.Enqueue(_str.Act.MoveByVector(new MyPoint(100, 0)));

            _str.DelayTaksBuilder.Create(Group.LandLeft, LandLeftGo, 200);
        }

        private void TanksScale()
        {
            _str.MainGameTasks.Enqueue(_str.Act.SelectByGroup((int)Group.Tank));
            _str.MainGameTasks.Enqueue(_str.Act.Scale(0.1));

            var myV = _str.MyVehicles.Where(v => v.Groups.Contains((int)Group.LandTop));
            if (myV == null || !myV.Any())
            {
                return;
            }
            var centerV = myV.CenterXY();
            if (_str.IndicatorFacillites.OnFacility(centerV))
            {
                _str.DelayTaksBuilder.Create(Group.LandTop, LandTanksGo, 200);
            }
            else
            {
                _str.DelayTaksBuilder.Create(Group.Tank, TanksShift, 200);
            }
        }

        private void TanksShift()
        {
            _str.MainGameTasks.Enqueue(_str.Act.SelectByGroup((int)Group.Tank));
            _str.MainGameTasks.Enqueue(_str.Act.MoveByVector(new MyPoint(100, 0)));

            _str.DelayTaksBuilder.Create(Group.Tank, LandTanksGo, 200);
        }

        private void TopRFirstGO()
        {
            _str.MainGameTasks.Enqueue(_str.Act.SelectByGroup((int)Group.LandTopR));
            _str.MainGameTasks.Enqueue(_str.Act.MoveToPoint(new MyPoint(350, 0)));

            _str.DelayTaksBuilder.Create(Group.LandTop, LandTopRGo);
        }

        private void TopRScale()
        {
            _str.MainGameTasks.Enqueue(_str.Act.SelectByGroup((int)Group.LandTopR));
            _str.MainGameTasks.Enqueue(_str.Act.Scale(0.1));

            _str.DelayTaksBuilder.Create(Group.LandTop, TopRFirstGO, 200);
        }

        private void TopRShift()
        {
            _str.MainGameTasks.Enqueue(_str.Act.SelectByGroup((int)Group.LandTopR));
            _str.MainGameTasks.Enqueue(_str.Act.MoveByVector(new MyPoint(100, 0)));

            _str.DelayTaksBuilder.Create(Group.LandTopR, LandTopRGo, 200);
        }

        private void TopScale()
        {
            _str.MainGameTasks.Enqueue(_str.Act.SelectByGroup((int)Group.LandTop));
            _str.MainGameTasks.Enqueue(_str.Act.Scale(0.1));

            var myV = _str.MyVehicles.Where(v => v.Groups.Contains((int)Group.LandTop));
            if (myV == null || !myV.Any())
            {
                return;
            }
            var centerV = myV.CenterXY();
            if (_str.IndicatorFacillites.OnFacility(centerV))
            {
                _str.DelayTaksBuilder.Create(Group.LandTop, LandTopGo, 200);
            }
            else
            {
                _str.DelayTaksBuilder.Create(Group.LandTop, TopShift, 200);
            }
        }

        private void TopShift()
        {
            _str.MainGameTasks.Enqueue(_str.Act.SelectByGroup((int)Group.LandTop));
            _str.MainGameTasks.Enqueue(_str.Act.MoveByVector(new MyPoint(100, 0)));

            _str.DelayTaksBuilder.Create(Group.LandTop, LandTopGo, 200);
        }

        #endregion Private Methods
    }
}