using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk
{
    public class MyUnitWithDelayedTask
    {
        #region Private Fields

        private int _abortAfter;
        private Action _delayedAction;
        private Queue<Task<bool>> _delayedTask;
        private int _delayInTick = 70;
        private int _groupId;
        private int _startTick;
        private MyStrategy _strategy;

        #endregion Private Fields

        #region Public Constructors

        public MyUnitWithDelayedTask(MyStrategy strategy, Group group, Queue<Task<bool>> delayedTask, int abortAfter)
        {
            _abortAfter = abortAfter;
            _strategy = strategy;
            _groupId = (int)group;
            _strategy.UnitsForAdd.Add(this);
            _startTick = _strategy.World.TickIndex;

            _delayedTask = delayedTask;
            _delayedAction = () =>
            {
                while (_delayedTask.Any())
                {
                    var task = _delayedTask.Dequeue();
                    _strategy.MainGameTasks.Enqueue(task);
                }
            };
        }

        public MyUnitWithDelayedTask(MyStrategy strategy, Group group, Action delayedAction, int abortAfter)
        {
            _abortAfter = abortAfter;
            _strategy = strategy;
            _groupId = (int)group;
            _strategy.UnitsForAdd.Add(this);
            _startTick = _strategy.World.TickIndex;

            _delayedAction = delayedAction;
        }

        public MyUnitWithDelayedTask(MyStrategy strategy, int group, Action delayedAction, int abortAfter)
        {
            _abortAfter = abortAfter;
            _strategy = strategy;
            _groupId = group;
            _strategy.UnitsForAdd.Add(this);
            _startTick = _strategy.World.TickIndex;

            _delayedAction = delayedAction;
        }

        #endregion Public Constructors

        #region Public Methods

        public void CheckDelayedTask()
        {
            if (_strategy.World.TickIndex <= _startTick + _delayInTick) return;

            if (_strategy.World.TickIndex < _startTick + _abortAfter)
            {
                if (_strategy.World.TickIndex % 16 != 0) return;

                var vehicleIdInGroup = _strategy.MyVehicles.Where(v => v.Groups.Contains(_groupId) && v.RemainingAttackCooldownTicks == 0)
                    .Select(v => v.Id);
                if (vehicleIdInGroup.Any(vehicleId => _strategy.World.TickIndex - _strategy.UpdateTickByVehicleId[vehicleId] <= 2))
                {
                    return;
                }
            }

            _delayedAction?.Invoke();

            _strategy.UnitsForRemove.Add(this);
        }

        #endregion Public Methods
    }
}