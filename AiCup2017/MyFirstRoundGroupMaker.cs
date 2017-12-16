using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk
{
    public class MyFirstRoundGroupMaker : MyIGroupMaker
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

        public MyFirstRoundGroupMaker(MyStrategy strategy)
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

        private void EndMergeUnits()
        {
            mergeUnitsCount++;
            if (mergeUnitsCount == 2)
            {
                _str.MainGameTasks.Enqueue(_str.Act.SelectByGroup((int)Group.Temp1));
                _str.MainGameTasks.Enqueue(_str.Act.MoveByVector(new MyPoint(60, 0)));

                _str.MainGameTasks.Enqueue(_str.Act.SelectByGroup((int)Group.Temp2));
                _str.MainGameTasks.Enqueue(_str.Act.MoveByVector(new MyPoint(-60, 0)));

                _str.DelayTaksBuilder.Create(Group.All, RotateGroup45, 400);
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

            _str.MainGameTasks.Enqueue(_str.Act.MoveToPoint(new MyPoint(193, 193)));
            _str.MainGameTasks.Enqueue(_str.Act.AssignToGroupTask(Group.Air));

            var delayedTask1 = new Queue<Task<bool>>();
            delayedTask1.Enqueue(_str.Act.SelectByGroup((int)fistGroup.ToGroup()));
            delayedTask1.Enqueue(_str.Act.MoveToPoint(new MyPoint(179, 119)));

            _str.DelayTaksBuilder.Create(fistGroup.ToGroup(), delayedTask1);

            _str.MainGameTasks.Enqueue(_str.Act.SelectByTypeAndFrameTask(secondGroup, new MyPoint(0, 0), new MyPoint(_str.World.Width, _str.World.Height)));

            _str.MainGameTasks.Enqueue(_str.Act.AssignToGroupTask(secondGroup.ToGroup()));

            _str.MainGameTasks.Enqueue(_str.Act.MoveToPoint(new MyPoint(45, 45)));
            _str.MainGameTasks.Enqueue(_str.Act.AssignToGroupTask(Group.Air));

            var delayedTask2 = new Queue<Task<bool>>();
            delayedTask2.Enqueue(_str.Act.SelectByGroup((int)secondGroup.ToGroup()));
            delayedTask2.Enqueue(_str.Act.MoveToPoint(new MyPoint(59, 119)));
            _str.DelayTaksBuilder.Create(secondGroup.ToGroup(), delayedTask2);
        }

        private void LastMoveToGroup()
        {
            _str.MainGameTasks.Enqueue(_str.Act.SelectByGroup((int)_landGroups[0]));
            _str.MainGameTasks.Enqueue(_str.Act.MoveToPoint(new MyPoint(59, 119)));

            _str.MainGameTasks.Enqueue(_str.Act.SelectByGroup((int)_landGroups[2]));
            _str.MainGameTasks.Enqueue(_str.Act.MoveToPoint(new MyPoint(179, 119)));

            _str.DelayTaksBuilder.Create(Group.All, VehicleToRows);
            var squad = new MySquad()
            {
                Id = (int)Group.All,
            };
            _str.GroupManager.AddBigSquad(squad);
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

        private void MoveMidLandGroup()
        {
            landTopAndDownGroupFinished++;
            if (landTopAndDownGroupFinished == 2)
            {
                _str.MainGameTasks.Enqueue(_str.Act.SelectByTypeAndFrameTask(_landGroups[1].ToVehicleType(), new MyPoint(0, 0), new MyPoint(_str.World.Width, _str.World.Height)));

                _str.MainGameTasks.Enqueue(_str.Act.AssignToGroupTask(_landGroups[1]));
                _str.MainGameTasks.Enqueue(_str.Act.AssignToGroupTask(Group.Land));

                if (!_positions[_landGroups[1]].Ravno(new MyPoint(1, 1)))
                {
                    _str.MainGameTasks.Enqueue(_str.Act.MoveToPoint(new MyPoint(119, 119)));
                }

                if (_positions[_landGroups[1]].X == 1)
                {
                    LastMoveToGroup();
                }
                else
                {
                    _str.DelayTaksBuilder.Create(_landGroups[1], LastMoveToGroup);
                }
            }
        }

        private void MoveTopAndDownLandGroup()
        {
            _landGroups = _positions.Where(v => v.Key != Group.Fighter && v.Key != Group.Helicopter).OrderBy(v => v.Value.X + v.Value.Y).Select(v => v.Key).ToArray();

            if (_positions[_landGroups[0]].Ravno(new MyPoint(1, 0)) && _positions[_landGroups[1]].Ravno(new MyPoint(0, 1)))
            {
                _landGroups.Swap(0, 1);
            }
            if (_positions[_landGroups[1]].Ravno(new MyPoint(2, 1)) && _positions[_landGroups[2]].Ravno(new MyPoint(1, 2)))
            {
                _landGroups.Swap(1, 2);
            }
            if (_positions[_landGroups[1]].Ravno(new MyPoint(0, 2)) && _positions[_landGroups[0]].Ravno(new MyPoint(1, 1)))
            {
                _landGroups.Swap(1, 0);
            }
            if (_positions[_landGroups[1]].Ravno(new MyPoint(2, 0)) && _positions[_landGroups[2]].Ravno(new MyPoint(1, 1)))
            {
                _landGroups.Swap(1, 2);
            }

            _str.MainGameTasks.Enqueue(_str.Act.SelectByTypeAndFrameTask(_landGroups[2].ToVehicleType(), new MyPoint(0, 0), new MyPoint(_str.World.Width, _str.World.Height)));

            _str.MainGameTasks.Enqueue(_str.Act.AssignToGroupTask(_landGroups[2]));
            _str.MainGameTasks.Enqueue(_str.Act.AssignToGroupTask(Group.Land));

            if (_positions[_landGroups[2]].Ravno(new MyPoint(2, 2)) ||
                _positions[_landGroups[2]].Ravno(new MyPoint(2, 0)) ||
                (_positions[_landGroups[2]].Ravno(new MyPoint(2, 1)) && !_positions[_landGroups[1]].Ravno(new MyPoint(2, 0))))
            {
                MoveMidLandGroup();
            }
            else
            {
                _str.MainGameTasks.Enqueue(_str.Act.MoveToPoint(new MyPoint(193, 193)));

                _str.DelayTaksBuilder.Create(_landGroups[2], MoveMidLandGroup);
            }

            _str.MainGameTasks.Enqueue(_str.Act.SelectByTypeAndFrameTask(_landGroups[0].ToVehicleType(), new MyPoint(0, 0), new MyPoint(_str.World.Width, _str.World.Height)));

            _str.MainGameTasks.Enqueue(_str.Act.AssignToGroupTask(_landGroups[0]));
            _str.MainGameTasks.Enqueue(_str.Act.AssignToGroupTask(Group.Land));

            if (_positions[_landGroups[0]].Ravno(new MyPoint(0, 0)) ||
                _positions[_landGroups[0]].Ravno(new MyPoint(0, 2)) ||
                (_positions[_landGroups[0]].Ravno(new MyPoint(0, 1)) && !_positions[_landGroups[1]].Ravno(new MyPoint(0, 2))))
            {
                MoveMidLandGroup();
            }
            else
            {
                _str.MainGameTasks.Enqueue(_str.Act.MoveToPoint(new MyPoint(45, 45)));

                _str.DelayTaksBuilder.Create(_landGroups[0], MoveMidLandGroup);
            }
        }

        private void RotateGroup45()
        {
            _str.MainGameTasks.Enqueue(_str.Act.SelectByGroup((int)Group.All));
            _str.MainGameTasks.Enqueue(_str.Act.FastCenterRotate(45));
            _str.GroupingEnded = true;
            _str.DelayTaksBuilder.Create(Group.All, StayAtOne);
        }

        private void StayAtOne()
        {
            _str.MainGameTasks.Enqueue(_str.Act.SelectByGroup((int)Group.All));
            _str.MainGameTasks.Enqueue(_str.Act.Scale(0.1));

            _str.DelayTaksBuilder.Create(Group.All, Ready, 300);
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