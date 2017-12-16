namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk
{
    public class MySelectingFrame
    {
        #region Public Constructors

        public MySelectingFrame()
        {
        }

        public MySelectingFrame(MyPoint leftTop, MyPoint rigthBot)
        {
            LeftTop = leftTop;
            RigthBot = rigthBot;
        }

        #endregion Public Constructors

        #region Public Properties

        public MyPoint LeftTop { get; set; }

        public MyPoint RigthBot { get; set; }

        #endregion Public Properties
    }
}