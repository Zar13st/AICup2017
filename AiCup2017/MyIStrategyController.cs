namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk
{
    public interface MyIStrategyController
    {
        #region Public Methods

        void FindSingleEnemy(MySquad group);

        MyPoint FindSingleEnemy(int group);

        void Process();

        #endregion Public Methods
    }
}