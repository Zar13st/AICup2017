using System;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk
{
    public interface MyIGroupMaker
    {
        #region Public Events

        event Action Ready;

        #endregion Public Events

        #region Public Methods

        void Make();

        #endregion Public Methods
    }
}