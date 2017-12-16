using System.Linq;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk
{
    public class MyFirstRoundNuckearEvader : MyINuclearEvader
    {
        #region Private Fields

        private int _nuclearAlarmTick = 20001;
        private MyPoint _nuclearPoint = new MyPoint();
        private MyStrategy _str;

        #endregion Private Fields

        #region Public Constructors

        public MyFirstRoundNuckearEvader(MyStrategy strategy)
        {
            _str = strategy;
        }

        #endregion Public Constructors

        #region Public Methods

        public bool CheckNuclearAlarm()
        {
            if (_str.GroupingEnded)
            {
                if (_str.World.GetOpponentPlayer().RemainingNuclearStrikeCooldownTicks > 512 && _str.EnemyNuclearReady)
                {
                    var x = _str.World.GetOpponentPlayer().NextNuclearStrikeX;
                    var y = _str.World.GetOpponentPlayer().NextNuclearStrikeY;
                    if (x <= 0 && y <= 0)
                    {
                        return false;
                    }
                    _str.EnemyNuclearReady = false;

                    _str.NuclearAlarm = true;

                    _nuclearAlarmTick = _str.World.TickIndex;
                    _nuclearPoint = new MyPoint(x, y);
                    _str.NuclearGameTasks.Enqueue(_str.Act.SelectByGroup((int)Group.All));
                    _str.NuclearGameTasks.Enqueue(_str.Act.Scale(10, _nuclearPoint));
                }
                if (!_str.EnemyNuclearReady)
                {
                    if (_str.World.GetOpponentPlayer().RemainingNuclearStrikeCooldownTicks == 64)
                    {
                        _str.EnemyNuclearReady = true;
                    }
                    if (_str.World.TickIndex == _nuclearAlarmTick + 31)
                    {
                        _str.NuclearGameTasks.Enqueue(_str.Act.Scale(0.1, _nuclearPoint));
                        _str.NuclearGameTasks.Enqueue(_str.Act.SelectByGroup(_str.CurrentGroup));
                    }
                    if (_str.World.TickIndex == _nuclearAlarmTick + 61)
                    {
                        _nuclearAlarmTick = 20001;
                        _str.NuclearAlarm = false;
                    }
                }
            }

            if (_str.NuclearGameTasks.Any())
            {
                _str.NuclearGameTasks.Dequeue().RunSynchronously();
                return true;
            }
            return false;
        }

        #endregion Public Methods
    }
}