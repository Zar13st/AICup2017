using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;
using System.Linq;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk
{
    public class MyEnemyStrategyRecognizer
    {
        #region Private Fields

        private MyStrategy _strategy;

        #endregion Private Fields

        #region Public Constructors

        public MyEnemyStrategyRecognizer(MyStrategy strategy)
        {
            _strategy = strategy;
        }

        #endregion Public Constructors

        #region Public Methods

        public EnemyStrategyType Recognize()
        {
            var randomTank = _strategy.EnemyVehicles.FirstOrDefault(v => v.Type == VehicleType.Tank);
            if (randomTank == null)
            {
                return EnemyStrategyType.Solo;
            }

            var groupSize = _strategy.EnemyVehicles.Count(v => v.GetDistanceTo(randomTank.X, randomTank.Y) < 150);

            if (groupSize >= 160)
            {
                return EnemyStrategyType.Solo;
            }
            else
            {
                return EnemyStrategyType.Spread;
            }
        }

        #endregion Public Methods
    }
}