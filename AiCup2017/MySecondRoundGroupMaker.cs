using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk
{
    public class MySecondRoundGroupMaker : MyIGroupMaker
    {
        #region Private Fields

        private readonly MyStrategy _str;
        private int _dl = 80;
        private Group[] _landGroups;
        private Dictionary<Group, MyPoint> _positions = new Dictionary<Group, MyPoint>();

        private int landTopAndDownGroupFinished = 0;

        private int mergeUnitsCount = 0;

        #endregion Private Fields

        #region Public Constructors

        public MySecondRoundGroupMaker(MyStrategy strategy)
        {
            _str = strategy;
        }

        #endregion Public Constructors

        #region Public Events

        public event Action Ready;

        #endregion Public Events

        #region Public Methods

        public void Make()
        {
            foreach (var typeInt in Enum.GetValues(typeof(VehicleType)))
            {
                var center = _str.MyVehicles.Where(v => (int)v.Type == (int)typeInt).CenterXY();

                int x = Convert.ToInt32(center.X);
                int y = Convert.ToInt32(center.Y);

                _positions[((VehicleType)typeInt).ToGroup()] = new MyPoint(x / _dl, y / _dl);
            }
            _str.MainGameTasks.Enqueue(_str.Act.SelectByFrameTask(new MyPoint(0, 0), new MyPoint(_str.World.Width, _str.World.Height)));

            _str.MainGameTasks.Enqueue(_str.Act.AssignToGroupTask(Group.All));

            MoveTopAndDownLandGroup();

            GroupAir();
        }

        #endregion Public Methods

        #region Private Methods

        private void AirMission()
        {
            _str.MainGameTasks.Enqueue(_str.Act.SelectByGroup((int)Group.Air));
            _str.MainGameTasks.Enqueue(_str.Act.MoveByVector(new MyPoint(100, 100)));
            _str.DelayTaksBuilder.Create(Group.Air, Split, 200);
        }

        private void EndMergeUnits()
        {
            mergeUnitsCount++;
            if (mergeUnitsCount == 2)
            {
                _str.MainGameTasks.Enqueue(_str.Act.SelectByGroup((int)Group.Temp1));
                _str.MainGameTasks.Enqueue(_str.Act.MoveByVector(new MyPoint(60, 0)));

                _str.MainGameTasks.Enqueue(_str.Act.SelectByGroup((int)Group.Temp2));
                _str.MainGameTasks.Enqueue(_str.Act.MoveByVector(new MyPoint(-60, 0)));

                _str.DelayTaksBuilder.Create(Group.All, RotateGroup45, 200);
            }
        }

        private void GroupAir()
        {
            VehicleType fistGroup = VehicleType.Helicopter;
            VehicleType secondGroup = VehicleType.Fighter;
            if (_positions[Group.Fighter].X + _positions[Group.Fighter].Y >=
                _positions[Group.Helicopter].X + _positions[Group.Helicopter].Y)
            {
                fistGroup = VehicleType.Fighter;
                secondGroup = VehicleType.Helicopter;
            }

            _str.MainGameTasks.Enqueue(_str.Act.SelectByTypeAndFrameTask(fistGroup, new MyPoint(0, 0), new MyPoint(_str.World.Width, _str.World.Height)));

            _str.MainGameTasks.Enqueue(_str.Act.AssignToGroupTask(fistGroup.ToGroup()));

            _str.MainGameTasks.Enqueue(_str.Act.MoveToPoint(new MyPoint(267, 267)));
            _str.MainGameTasks.Enqueue(_str.Act.AssignToGroupTask(Group.Air));

            _str.MainGameTasks.Enqueue(_str.Act.SelectByTypeAndFrameTask(secondGroup, new MyPoint(0, 0), new MyPoint(_str.World.Width, _str.World.Height)));

            _str.MainGameTasks.Enqueue(_str.Act.AssignToGroupTask(secondGroup.ToGroup()));

            _str.MainGameTasks.Enqueue(_str.Act.MoveToPoint(new MyPoint(119, 119 + 9)));
            _str.MainGameTasks.Enqueue(_str.Act.AssignToGroupTask(Group.Air));

            _str.DelayTaksBuilder.Create(Group.Air, MixAir);

            var squadF = new MySquad()
            {
                Id = (int)Group.Fighter,
                VehicleType = VehicleType.Fighter
            };

            var squadC = new MySquad()
            {
                Id = (int)Group.Copter1,
                VehicleType = VehicleType.Helicopter
            };

            _str.GroupManager.AddBigSquad(squadF);
            _str.GroupManager.AddBigSquad(squadC);
        }

        private void LastMoveToGroup()
        {
            _str.MainGameTasks.Enqueue(_str.Act.SelectByGroup((int)_landGroups[0]));
            _str.MainGameTasks.Enqueue(_str.Act.MoveToPoint(new MyPoint(59, 119)));

            _str.MainGameTasks.Enqueue(_str.Act.SelectByGroup((int)_landGroups[2]));
            _str.MainGameTasks.Enqueue(_str.Act.MoveToPoint(new MyPoint(179, 119)));

            _str.DelayTaksBuilder.Create(Group.All, VehicleToRows);
        }

        private void MergeUnits()
        {
            _str.MainGameTasks.Enqueue(_str.Act.SelectByFrameTask(new MyPoint(0, 0), new MyPoint(89, _str.World.Width)));
            _str.MainGameTasks.Enqueue(_str.Act.AssignToGroupTask(Group.Temp1));
            _str.MainGameTasks.Enqueue(_str.Act.MoveByVector(new MyPoint(0, -6)));

            _str.DelayTaksBuilder.Create(Group.Temp1, EndMergeUnits);

            _str.MainGameTasks.Enqueue(_str.Act.SelectByFrameTask(new MyPoint(149, 0), new MyPoint(_str.World.Height, _str.World.Width)));
            _str.MainGameTasks.Enqueue(_str.Act.AssignToGroupTask(Group.Temp2));
            _str.MainGameTasks.Enqueue(_str.Act.MoveByVector(new MyPoint(0, 6)));

            _str.DelayTaksBuilder.Create(Group.Temp2, EndMergeUnits);
        }

        private void MixAir()
        {
            VehicleType fistGroup = VehicleType.Helicopter;
            VehicleType secondGroup = VehicleType.Fighter;
            if (_positions[Group.Fighter].X + _positions[Group.Fighter].Y >=
                _positions[Group.Helicopter].X + _positions[Group.Helicopter].Y)
            {
                fistGroup = VehicleType.Fighter;
                secondGroup = VehicleType.Helicopter;
            }

            _str.MainGameTasks.Enqueue(_str.Act.SelectByGroup((int)secondGroup.ToGroup()));
            _str.MainGameTasks.Enqueue(_str.Act.Scale(3, new MyPoint(92, 92 + 9)));

            _str.MainGameTasks.Enqueue(_str.Act.SelectByGroup((int)fistGroup.ToGroup()));
            _str.MainGameTasks.Enqueue(_str.Act.Scale(3, new MyPoint(240, 240)));

            _str.DelayTaksBuilder.Create(Group.Air, MixAir2);
        }

        private void MixAir2()
        {
            VehicleType fistGroup = VehicleType.Helicopter;
            VehicleType secondGroup = VehicleType.Fighter;
            if (_positions[Group.Fighter].X + _positions[Group.Fighter].Y >=
                _positions[Group.Helicopter].X + _positions[Group.Helicopter].Y)
            {
                fistGroup = VehicleType.Fighter;
                secondGroup = VehicleType.Helicopter;
            }

            _str.MainGameTasks.Enqueue(_str.Act.SelectByGroup((int)secondGroup.ToGroup()));
            _str.MainGameTasks.Enqueue(_str.Act.MoveByVector(new MyPoint(71, 71)));

            _str.MainGameTasks.Enqueue(_str.Act.SelectByGroup((int)fistGroup.ToGroup()));
            _str.MainGameTasks.Enqueue(_str.Act.MoveByVector(new MyPoint(-71, -71)));

            _str.DelayTaksBuilder.Create(Group.Air, MixAir3);
        }

        private void MixAir3()
        {
            _str.MainGameTasks.Enqueue(_str.Act.SelectByGroup((int)Group.Air));
            _str.MainGameTasks.Enqueue(_str.Act.Scale(0.1));

            _str.DelayTaksBuilder.Create(Group.Air, Ready);
        }

        private void MoveMidLandGroup()
        {
            landTopAndDownGroupFinished++;
            if (landTopAndDownGroupFinished == 2)
            {
                for (var i = 2; i >= 0; i--)
                {
                    var tempI = i;
                    if (_landGroups[tempI].ToVehicleType() == VehicleType.Tank)
                    {
                        _str.MainGameTasks.Enqueue(_str.Act.SelectByGroup((int)_landGroups[tempI]));
                    }
                    else
                    {
                        var centerLandGroups = _str.MyVehicles.Where(v => v.Groups.Contains((int)_landGroups[tempI])).CenterXY();

                        _str.MainGameTasks.Enqueue(_str.Act.SelectByTypeAndFrameTask(_landGroups[tempI].ToVehicleType(), new MyPoint(0, 0), new MyPoint(centerLandGroups.X + 40, centerLandGroups.Y)));

                        if (_landGroups[tempI].ToVehicleType() == VehicleType.Ifv)
                        {
                            _str.MainGameTasks.Enqueue(_str.Act.AssignToGroupTask(Group.LandTop));
                        }
                        else
                        {
                            _str.MainGameTasks.Enqueue(_str.Act.AssignToGroupTask(Group.LandTopR));
                        }

                        if (tempI == 0)
                        {
                            _str.MainGameTasks.Enqueue(_str.Act.MoveByVector(new MyPoint(100, 0)));
                        }
                        else
                        {
                            _str.MainGameTasks.Enqueue(_str.Act.MoveByVector(new MyPoint(150, 0)));
                        }

                        _str.MainGameTasks.Enqueue(_str.Act.SelectByTypeAndFrameTask(_landGroups[tempI].ToVehicleType(), new MyPoint(0, centerLandGroups.Y), new MyPoint(centerLandGroups.X + 40, 1024)));

                        if (_landGroups[tempI].ToVehicleType() == VehicleType.Ifv)
                        {
                            _str.MainGameTasks.Enqueue(_str.Act.AssignToGroupTask(Group.LandLeft));
                        }
                        else
                        {
                            _str.MainGameTasks.Enqueue(_str.Act.AssignToGroupTask(Group.LandLeftR));
                        }

                        if (tempI == 0)
                        {
                            _str.MainGameTasks.Enqueue(_str.Act.MoveByVector(new MyPoint(0, 100)));
                        }
                        else
                        {
                            _str.MainGameTasks.Enqueue(_str.Act.MoveByVector(new MyPoint(0, 150)));
                        }
                    }
                }

                var squadL1 = new MySquad()
                {
                    Id = (int)Group.LandTop,
                    VehicleType = VehicleType.Ifv
                };

                var squadL2 = new MySquad()
                {
                    Id = (int)Group.LandLeft,
                    VehicleType = VehicleType.Ifv
                };

                var squadL1R = new MySquad()
                {
                    Id = (int)Group.LandTopR,
                    VehicleType = VehicleType.Arrv
                };

                var squadL2R = new MySquad()
                {
                    Id = (int)Group.LandLeftR,
                    VehicleType = VehicleType.Arrv
                };

                var squadT = new MySquad()
                {
                    Id = (int)Group.Tank,
                    VehicleType = VehicleType.Tank
                };

                _str.GroupManager.AddBigSquad(squadL1);
                _str.GroupManager.AddBigSquad(squadL2);
                _str.GroupManager.AddBigSquad(squadL1R);
                _str.GroupManager.AddBigSquad(squadL2R);
                _str.GroupManager.AddBigSquad(squadT);

                _str.DelayTaksBuilder.Create(_landGroups[1], Ready);
            }
        }

        private void MoveTopAndDownLandGroup()
        {
            _landGroups = _positions.Where(v => v.Key != Group.Fighter && v.Key != Group.Helicopter).OrderBy(v => v.Value.X + v.Value.Y).Select(v => v.Key).ToArray();

            _str.MainGameTasks.Enqueue(_str.Act.SelectByTypeAndFrameTask(_landGroups[2].ToVehicleType(), new MyPoint(0, 0), new MyPoint(_str.World.Width, _str.World.Height)));

            _str.MainGameTasks.Enqueue(_str.Act.AssignToGroupTask(_landGroups[2]));

            _str.MainGameTasks.Enqueue(_str.Act.MoveToPoint(new MyPoint(267, 267)));

            _str.DelayTaksBuilder.Create(_landGroups[2], MoveMidLandGroup);

            _str.MainGameTasks.Enqueue(_str.Act.SelectByTypeAndFrameTask(_landGroups[0].ToVehicleType(), new MyPoint(0, 0), new MyPoint(_str.World.Width, _str.World.Height)));

            _str.MainGameTasks.Enqueue(_str.Act.AssignToGroupTask(_landGroups[0]));

            _str.MainGameTasks.Enqueue(_str.Act.MoveToPoint(new MyPoint(45, 45)));

            _str.DelayTaksBuilder.Create(_landGroups[0], MoveMidLandGroup);

            _str.MainGameTasks.Enqueue(_str.Act.SelectByTypeAndFrameTask(_landGroups[1].ToVehicleType(), new MyPoint(0, 0), new MyPoint(_str.World.Width, _str.World.Height)));

            _str.MainGameTasks.Enqueue(_str.Act.AssignToGroupTask(_landGroups[1]));
        }

        private void RotateGroup45()
        {
            _str.MainGameTasks.Enqueue(_str.Act.SelectByGroup((int)Group.Air));
            _str.MainGameTasks.Enqueue(_str.Act.Scale(0.1));

            _str.DelayTaksBuilder.Create(Group.Air, AirMission, 100);
        }

        private void Split()
        {
            //Ready?.Invoke();

            var centerLandGroups = _str.MyVehicles.Where(v => v.Groups.Contains((int)Group.Land)).CenterXY();

            _str.MainGameTasks.Enqueue(_str.Act.SelectByFrameTask(new MyPoint(0, 0), new MyPoint(centerLandGroups.X + 50, centerLandGroups.Y)));
            _str.MainGameTasks.Enqueue(_str.Act.AssignToGroupTask(Group.LandTop));
            _str.MainGameTasks.Enqueue(_str.Act.MoveByVector(new MyPoint(50, 0)));

            _str.MainGameTasks.Enqueue(_str.Act.SelectByFrameTask(new MyPoint(0, centerLandGroups.Y), new MyPoint(centerLandGroups.X + 50, 1024)));
            _str.MainGameTasks.Enqueue(_str.Act.AssignToGroupTask(Group.LandLeft));
            _str.MainGameTasks.Enqueue(_str.Act.MoveByVector(new MyPoint(0, 50)));

            var squadL1 = new MySquad()
            {
                Id = (int)Group.LandTop
            };

            var squadL2 = new MySquad()
            {
                Id = (int)Group.LandLeft
            };

            _str.GroupManager.AddBigSquad(squadL1);
            _str.GroupManager.AddBigSquad(squadL2);

            //_str.DelayTaksBuilder.Create(Group.Land, Ready);
        }

        private void VehicleToRows()
        {
            for (int i = 0; i < 9; i++)
            {
                _str.MainGameTasks.Enqueue(_str.Act.SelectByFrameTask(new MyPoint(0, 143 - i * 6), new MyPoint(_str.World.Width, 149 - i * 6)));
                _str.MainGameTasks.Enqueue(_str.Act.MoveByVector(new MyPoint(0, (9 - i) * 12)));
            }

            _str.DelayTaksBuilder.Create(Group.All, MergeUnits);
        }

        #endregion Private Methods
    }
}