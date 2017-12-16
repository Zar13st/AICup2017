using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;
using System.Collections.Generic;
using System.Linq;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk
{
    public class MyIndicatorFacilites
    {
        #region Private Fields

        private int _newSquadId = (int)Group.NewGroup;
        private MyStrategy _str;

        #endregion Private Fields

        #region Public Constructors

        public MyIndicatorFacilites(MyStrategy strategy)
        {
            _str = strategy;
        }

        #endregion Public Constructors

        #region Public Methods

        public MyPoint GetNearestFasility(Group group)
        {
            var squad = _str.GroupManager.Squads.Single(s => s.Id == (int)group);

            if (squad == null) return new MyPoint(500, 300);

            double distToFacility = 10000d;
            IEnumerable<Facility> facilities;
            switch (group)
            {
                case Group.LandTopR:
                    facilities = _str.World.Facilities.Where(v => v.OwnerPlayerId != _str.Me.Id && v.Left - v.Top >= 350);
                    break;

                case Group.LandTop:
                    facilities = _str.World.Facilities.Where(v => v.OwnerPlayerId != _str.Me.Id && v.Left - v.Top < 350 && v.Left - v.Top >= 200);
                    break;

                case Group.Tank:
                    facilities = _str.World.Facilities.Where(v => v.OwnerPlayerId != _str.Me.Id && v.Left - v.Top < 200 && v.Left - v.Top > -200);
                    break;

                case Group.LandLeft:
                    facilities = _str.World.Facilities.Where(v => v.OwnerPlayerId != _str.Me.Id && v.Left - v.Top <= -200 && v.Left - v.Top > -350);
                    break;

                case Group.LandLeftR:
                    facilities = _str.World.Facilities.Where(v => v.OwnerPlayerId != _str.Me.Id && v.Left - v.Top <= -350);
                    break;

                default:
                    facilities = _str.World.Facilities.Where(v => v.OwnerPlayerId != _str.Me.Id);
                    break;
            }

            if (facilities == null || !facilities.Any())
            {
                if (group == Group.LandLeftR || group == Group.LandTopR)
                {
                    if (group == Group.LandLeftR)
                    {
                        facilities = _str.World.Facilities.Where(v => v.OwnerPlayerId != _str.Me.Id && v.VehicleType != VehicleType.Tank && v.Left < v.Top);
                    }
                    else
                    {
                        facilities = _str.World.Facilities.Where(v => v.OwnerPlayerId != _str.Me.Id && v.VehicleType != VehicleType.Tank && v.Left >= v.Top);
                    }
                }
                else
                {
                    facilities = _str.World.Facilities.Where(v => v.OwnerPlayerId != _str.Me.Id);
                }
            }

            if (facilities == null || !facilities.Any())
            {
                if (group != Group.LandLeftR && group != Group.LandTopR)
                {
                    return _str.StrategyController.FindSingleEnemy((int)group);
                }
            }

            MyPoint point = new MyPoint(0, 0);
            foreach (var facility in facilities)
            {
                var distToCurrentFacility = squad.Distance(facility);
                if (distToCurrentFacility < distToFacility)
                {
                    distToFacility = distToCurrentFacility;

                    point.X = facility.Left + 32;
                    point.Y = facility.Top + 32;
                }
            }

            return point;
        }

        public bool OnFacility(MyPoint gPoint)
        {
            double distToFacility = 10000d;
            MyPoint point = new MyPoint();
            foreach (var facility in _str.World.Facilities.Where(f => f.CapturePoints > 90))
            {
                var distToCurrentFacility = facility.Distance(gPoint.X, gPoint.Y);
                if (distToCurrentFacility < distToFacility)
                {
                    distToFacility = distToCurrentFacility;

                    point.X = facility.Left + 32;
                    point.Y = facility.Top + 32;
                }
            }

            if (distToFacility < 70)
            {
                return false;
            }
            return true;
        }

        public void Update(Facility[] facilities)
        {
            foreach (var facility in facilities)
            {
                var id = facility.Id;

                if (facility.Type == FacilityType.VehicleFactory && facility.OwnerPlayerId == _str.Me.Id)
                {
                    if (facility.VehicleType == null)
                    {
                        _str.MainGameTasks.Enqueue(_str.Act.SetupVehicleProduction(VehicleType.Tank, id));
                    }

                    if (_str.World.TickIndex % 256 != 0) continue;

                    var newVehicleCount = _str.MyVehicles.Count(v =>
                        v.X >= facility.Left &&
                        v.X <= facility.Left + 64 &&
                        v.Y >= facility.Top &&
                        v.Y <= facility.Top + 64 &&
                        v.Groups.Length == 0);

                    if (newVehicleCount > 30)
                    {
                        var squad = new MySquad()
                        {
                            VehicleType = (VehicleType)facility.VehicleType,
                            Id = _newSquadId,
                        };

                        _str.GroupManager.Squads.Add(squad);

                        _newSquadId++;

                        var lastGroupId = _str.CurrentGroup;
                        _str.MainGameTasks.Enqueue(_str.Act.SelectByTypeAndFrameTask((VehicleType)facility.VehicleType, new MyPoint(facility.Left, facility.Top), new MyPoint(facility.Left + 64, facility.Top + 64)));
                        _str.MainGameTasks.Enqueue(_str.Act.AssignToGroupTask(squad.Id));

                        var distToCenter = facility.Distance(512, 512);
                        var normVector = new MyPoint((512 - facility.Left + 32) / distToCenter, (512 - facility.Top + 8) / distToCenter);

                        _str.MainGameTasks.Enqueue(_str.Act.MoveByVector(new MyPoint(facility.Left + normVector.X * 100, facility.Top + normVector.Y * 100)));
                        _str.MainGameTasks.Enqueue(_str.Act.SelectByGroup(lastGroupId));
                    }
                }
            }
        }

        #endregion Public Methods
    }
}