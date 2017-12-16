using System.Collections.Generic;
using System.Threading.Tasks;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk
{
    public enum MissionType
    {
        GetEmptyFacility,
        GetEnemyFacilityWithFight,
        Move,
        Defend,
        Remont,
        ForTank,
        ForZenitka,
        ForRemont,
        ForSamolet,
        ForCopter,
    }

    public class MyMission
    {
        #region Public Properties

        public Queue<Task<bool>> Mission { get; set; }
        public MissionType Type { get; set; }

        #endregion Public Properties
    }
}