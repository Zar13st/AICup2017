using System.Collections.Generic;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk
{
    public class MyGroupManager
    {
        #region Private Fields

        private MyGridCellInfo[,] _grid;
        private MyStrategy _str;

        #endregion Private Fields

        #region Public Constructors

        public MyGroupManager(MyStrategy myStrategy)
        {
            _str = myStrategy;
            _grid = myStrategy.GameGrid.Grid;
        }

        #endregion Public Constructors

        #region Public Properties

        public List<MySquad> Squads { get; } = new List<MySquad>();

        #endregion Public Properties

        #region Public Methods

        public void AddBigSquad(MySquad squad)
        {
            Squads.Add(squad);
        }

        public void AddSquad(MySquad squad)
        {
            _str.DelayTaksBuilder.Create((Group)squad.Id, () =>
           {
               Squads.Add(squad);
               if (Squads.Count >= 5)
               {
                   _str.GroupingEnded = true;
               }
           });
        }

        #endregion Public Methods
    }
}