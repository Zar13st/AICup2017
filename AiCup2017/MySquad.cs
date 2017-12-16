using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk
{
    public class MySquad
    {
        #region Public Properties

        public bool HaveFacilityTarget { get; set; }
        public int Id { get; set; }

        public Queue<Task<bool>> NextTask { get; set; } = new Queue<Task<bool>>();
        public int NextX { get; set; }
        public int NextY { get; set; }
        public bool OnDuty { get; set; }
        public int TargetFacilityX { get; set; }
        public int TargetFacilityY { get; set; }
        public VehicleType VehicleType { get; set; }
        public int X { get; set; }

        public int Y { get; set; }

        #endregion Public Properties
    }
}