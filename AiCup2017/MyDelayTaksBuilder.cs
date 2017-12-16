using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk
{
    public class MyDelayTaksBuilder
    {
        #region Private Fields

        private MyStrategy _strategy;

        #endregion Private Fields

        #region Public Constructors

        public MyDelayTaksBuilder(MyStrategy strategy)
        {
            _strategy = strategy;
        }

        #endregion Public Constructors

        #region Public Methods

        public void Create(Group group, Queue<Task<bool>> delayedTask, int abortAfter = 600)
        {
            new MyUnitWithDelayedTask(_strategy, group, delayedTask, abortAfter);
        }

        public void Create(Group group, Action action, int abortAfter = 600)
        {
            new MyUnitWithDelayedTask(_strategy, group, action, abortAfter);
        }

        public void Create(int group, Action action, int abortAfter = 600)
        {
            new MyUnitWithDelayedTask(_strategy, group, action, abortAfter);
        }

        #endregion Public Methods
    }
}