using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;
using System;
using System.Linq;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk
{
    public class MyFirstRoundController : MyIStrategyController
    {
        #region Private Fields

        private readonly MyPoint _nearestEnemyPoint = new MyPoint();
        private readonly MyStrategy _str;
        private double _speed = 0.3d;

        #endregion Private Fields

        #region Public Constructors

        public MyFirstRoundController(MyStrategy str)
        {
            _str = str;
        }

        #endregion Public Constructors

        #region Public Methods

        public void FindSingleEnemy(MySquad group)
        {
        }

        public MyPoint FindSingleEnemy(int group)
        {
            return new MyPoint();
        }

        public void Process()
        {
            _str.GroupingEnded = true;
            _str.EnemyType = _str.EnemyStrategyRecognizer.Recognize();

            switch (_str.EnemyType)
            {
                case EnemyStrategyType.Solo:
                    {
                        GoToBigGroup();
                        break;
                    }
                case EnemyStrategyType.Spread:
                    {
                        FindSingleEnemy();
                        break;
                    }
                default:
                    break;
            }
        }

        #endregion Public Methods

        #region Private Methods

        private void FindSingleEnemy()
        {
            var myRandomUnit = _str.MyVehicles.First();

            double distance = 10000;
            var enemys = _str.EnemyVehicles.Where(v => v.Type == VehicleType.Tank);
            if (!enemys.Any())
            {
                enemys = _str.EnemyVehicles.Where(v => v.Type == VehicleType.Ifv || v.Type == VehicleType.Arrv);
                _speed = 0.4d;
            }
            if (!enemys.Any())
            {
                enemys = _str.EnemyVehicles.Where(v => v.Type == VehicleType.Helicopter);
                _speed = 0.9d;
            }
            if (!enemys.Any())
            {
                enemys = _str.EnemyVehicles;
                _speed = 1.2d;
            }
            if (!enemys.Any())
            {
                return;
            }

            foreach (var vehicle in enemys)
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
                    _nearestEnemyPoint.X = vehicle.X;
                    _nearestEnemyPoint.Y = vehicle.Y;
                }
            }
            KillSingleEnemy();
        }

        private void GoToBigGroup()
        {
            var enemyCenter = _str.EnemyVehicles.CenterXY();

            _str.MainGameTasks.Enqueue(_str.Act.SelectByGroup((int)Group.All));
            _str.MainGameTasks.Enqueue(_str.Act.MoveSlowToPoint(new MyPoint(enemyCenter.X, enemyCenter.Y)));

            _str.DelayTaksBuilder.Create(Group.All, Process);
        }

        private void KillSingleEnemy()
        {
            _str.MainGameTasks.Enqueue(_str.Act.SelectByGroup((int)Group.All));
            _str.MainGameTasks.Enqueue(_str.Act.MoveSlowToPoint(_nearestEnemyPoint, _speed));

            _str.DelayTaksBuilder.Create(Group.All, FindSingleEnemy);
        }

        #endregion Private Methods
    }
}