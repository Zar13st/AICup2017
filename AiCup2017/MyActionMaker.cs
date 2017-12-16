using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk
{
    public class MyActionMaker
    {
        #region Private Fields

        private readonly MyStrategy _str;

        #endregion Private Fields

        #region Public Constructors

        public MyActionMaker(MyStrategy strategy)
        {
            _str = strategy;
        }

        #endregion Public Constructors

        #region Public Methods

        public Task<bool> AssignToGroupTask(Group group)
        {
            var task = new Task<bool>(() =>
            {
                _str.Action.Action = ActionType.Assign;
                _str.Action.Group = (int)group;
                _str.CurrentGroup = (int)group;
                return true;
            });

            return task;
        }

        public Task<bool> AssignToGroupTask(int group)
        {
            var task = new Task<bool>(() =>
            {
                _str.Action.Action = ActionType.Assign;
                _str.Action.Group = group;
                _str.CurrentGroup = group;
                return true;
            });

            return task;
        }

        public Task<bool> CenterRotate(double angel)
        {
            var task = new Task<bool>(() =>
            {
                var center = _str.MyVehicles.Where(v => v.IsSelected).CenterXY();
                angel = angel * Math.PI / 180d;
                _str.Action.Action = ActionType.Rotate;
                _str.Action.X = center.X;
                _str.Action.Y = center.Y;
                _str.Action.Angle = angel;
                _str.Action.MaxSpeed = _str.Game.TankSpeed;

                return true;
            });
            return task;
        }

        public Task<bool> FastCenterRotate(double angel)
        {
            var task = new Task<bool>(() =>
            {
                var center = _str.MyVehicles.Where(v => v.IsSelected).CenterXY();
                angel = angel * Math.PI / 180d;
                _str.Action.Action = ActionType.Rotate;
                _str.Action.X = center.X;
                _str.Action.Y = center.Y;
                _str.Action.Angle = angel;

                return true;
            });
            return task;
        }

        public Task<bool> MoveByVector(MyPoint vector)
        {
            var task = new Task<bool>(() =>
            {
                _str.Action.Action = ActionType.Move;
                _str.Action.X = vector.X;
                _str.Action.Y = vector.Y;

                return true;
            });

            return task;
        }

        public Task<bool> MoveSlowToPoint(MyPoint point, double speed = 0.18)
        {
            var task = new Task<bool>(() =>
            {
                var centerGroup = _str.MyVehicles.Where(v => v.IsSelected).CenterXY();

                var targetX = point.X - centerGroup.X;
                var targetY = point.Y - centerGroup.Y;

                _str.Action.Action = ActionType.Move;
                _str.Action.X = targetX;
                _str.Action.Y = targetY;
                _str.Action.MaxSpeed = speed;

                return true;
            });

            return task;
        }

        public Task<bool> MoveToPoint(MyPoint point)
        {
            var task = new Task<bool>(() =>
            {
                var centerGroup = _str.MyVehicles.Where(v => v.IsSelected).CenterXY();

                var targetX = point.X - centerGroup.X;
                var targetY = point.Y - centerGroup.Y;

                _str.Action.Action = ActionType.Move;
                _str.Action.X = targetX;
                _str.Action.Y = targetY;

                return true;
            });

            return task;
        }

        public Task<bool> Nuclear(MyPoint p, long vId)
        {
            var task = new Task<bool>(() =>
            {
                _str.Action.Action = ActionType.TacticalNuclearStrike;
                _str.Action.X = p.X;
                _str.Action.Y = p.Y;
                _str.Action.VehicleId = vId;

                return true;
            });

            return task;
        }

        public Task<bool> Scale(double factor)
        {
            var task = new Task<bool>(() =>
            {
                var centerGroup = _str.MyVehicles.Where(v => v.IsSelected).CenterXY();

                _str.Action.Action = ActionType.Scale;
                _str.Action.Factor = factor;
                _str.Action.X = centerGroup.X;
                _str.Action.Y = centerGroup.Y;

                return true;
            });

            return task;
        }

        public Task<bool> Scale(double factor, MyPoint point)
        {
            var task = new Task<bool>(() =>
            {
                _str.Action.Action = ActionType.Scale;
                _str.Action.Factor = factor;
                _str.Action.X = point.X;
                _str.Action.Y = point.Y;

                return true;
            });

            return task;
        }

        public Task<bool> SelectByFrameTask(MyPoint leftTop, MyPoint rigthBot)
        {
            var task = new Task<bool>(() =>
            {
                _str.Action.Action = ActionType.ClearAndSelect;
                _str.Action.Left = leftTop.X;
                _str.Action.Top = leftTop.Y;
                _str.Action.Right = rigthBot.X;
                _str.Action.Bottom = rigthBot.Y;

                return true;
            });

            return task;
        }

        public Task<bool> SelectByGroup(int group)
        {
            var task = new Task<bool>(() =>
            {
                var g = group;
                if (g == _str.CurrentGroup)
                {
                    _str.Action.Action = ActionType.None;
                    return false;
                }
                else
                {
                    _str.Action.Action = ActionType.ClearAndSelect;
                    _str.Action.Group = group;
                    _str.CurrentGroup = group;
                    return true;
                }
            });
            return task;
        }

        public Task<bool> SelectByTypeAndFrameTask(VehicleType type, MyPoint leftTop, MyPoint rigthBot)
        {
            var task = new Task<bool>(() =>
            {
                _str.Action.Action = ActionType.ClearAndSelect;
                _str.Action.VehicleType = type;
                _str.Action.Left = leftTop.X;
                _str.Action.Top = leftTop.Y;
                _str.Action.Right = rigthBot.X;
                _str.Action.Bottom = rigthBot.Y;

                return true;
            });

            return task;
        }

        public Task<bool> SetupVehicleProduction(VehicleType vehicleType, long id)
        {
            var task = new Task<bool>(() =>
            {
                _str.Action.Action = ActionType.SetupVehicleProduction;
                _str.Action.VehicleType = vehicleType;
                _str.Action.FacilityId = id;
                return true;
            });

            return task;
        }

        #endregion Public Methods
    }
}