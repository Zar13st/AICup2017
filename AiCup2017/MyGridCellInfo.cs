using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;
using System.Collections.Generic;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk
{
    public class MyGridCellInfo
    {
        #region Public Constructors

        public MyGridCellInfo(int x, int y)
        {
            X = x;
            Y = y;
        }

        #endregion Public Constructors

        #region Public Properties

        public List<long> EnemyCopters { get; } = new List<long>();

        public int EnemyCount
        {
            get { return EnemyCopters.Count + EnemySamolets.Count + EnemyTanks.Count; }
        }

        public List<long> EnemyRemonts { get; } = new List<long>();
        public List<long> EnemySamolets { get; } = new List<long>();
        public List<long> EnemyTanks { get; } = new List<long>();
        public List<long> EnemyZeneitkas { get; } = new List<long>();
        public Facility Facility { get; set; }
        public bool IsEnemyCell { get; set; }
        public bool IsFight { get; set; }
        public MySquad Squad { get; set; }
        public int TagetSquad { get; set; }
        public int X { get; }
        public int Y { get; }

        #endregion Public Properties

        #region Public Methods

        public void Clear()
        {
            EnemyTanks.Clear();
            EnemySamolets.Clear();
            EnemyCopters.Clear();
            EnemyZeneitkas.Clear();
            EnemyRemonts.Clear();

            Facility = null;
            Squad = null;
            TagetSquad = 0;

            IsEnemyCell = false;
            IsFight = false;
        }

        #endregion Public Methods
    }
}