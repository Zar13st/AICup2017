namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk
{
    public class MyPoint
    {
        #region Public Constructors

        public MyPoint()
        {
        }

        public MyPoint(double x, double y)
        {
            X = x;
            Y = y;
        }

        #endregion Public Constructors

        #region Public Properties

        public double X { get; set; }

        public double Y { get; set; }

        #endregion Public Properties
    }
}