using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk
{
    public sealed class MyStrategy : IStrategy
    {
        #region Private Fields

        private int _turnsPer60Ticks;
        private int _turnsPer60TicksMin = 12;

        #endregion Private Fields

        #region Public Properties

        public MyActionMaker Act { get; set; }
        public Move Action { get; set; }
        public Queue<Task<bool>> CopterTasks { get; set; } = new Queue<Task<bool>>();
        public int CurrentGroup { get; set; }
        public MyDelayTaksBuilder DelayTaksBuilder { get; set; }
        public bool EnemyNuclearReady { get; set; } = true;
        public MyEnemyStrategyRecognizer EnemyStrategyRecognizer { get; set; }
        public EnemyStrategyType EnemyType { get; set; } = EnemyStrategyType.Unknown;
        public IEnumerable<Vehicle> EnemyVehicles { get { return VehicleById.Values.Where(v => v.PlayerId != Me.Id); } }
        public Game Game { get; set; }
        public MyGameGrid GameGrid { get; set; }
        public bool GroupingEnded { get; set; }
        public MyIGroupMaker GroupMaker { get; set; }
        public MyGroupManager GroupManager { get; set; }
        public MyIndicatorFacilites IndicatorFacillites { get; set; }
        public int LastGroup { get; set; }
        public Queue<Task<bool>> MainGameTasks { get; set; } = new Queue<Task<bool>>();
        public Player Me { get; set; }
        public IEnumerable<Vehicle> MyVehicles { get { return VehicleById.Values.Where(v => v.PlayerId == Me.Id); } }
        public bool NuclearAlarm { get; set; }
        public MyINuclearEvader NuclearEvader { get; set; }
        public Queue<Task<bool>> NuclearGameTasks { get; set; } = new Queue<Task<bool>>();
        public Queue<Task<bool>> RemontTasks { get; set; } = new Queue<Task<bool>>();
        public Queue<Task<bool>> SamoletTasks { get; set; } = new Queue<Task<bool>>();
        public MyIStrategyController StrategyController { get; set; }
        public Queue<Task<bool>> TankTasks { get; set; } = new Queue<Task<bool>>();
        public Queue<int> TurnAfterLast60Tick { get; set; } = new Queue<int>();
        public List<MyUnitWithDelayedTask> Units { get; set; } = new List<MyUnitWithDelayedTask>();
        public List<MyUnitWithDelayedTask> UnitsForAdd { get; set; } = new List<MyUnitWithDelayedTask>();
        public List<MyUnitWithDelayedTask> UnitsForRemove { get; set; } = new List<MyUnitWithDelayedTask>();
        public Dictionary<long, int> UpdateTickByVehicleId { get; set; } = new Dictionary<long, int>();
        public Dictionary<long, Vehicle> VehicleById { get; set; } = new Dictionary<long, Vehicle>();
        public World World { get; set; }
        public Queue<Task<bool>> ZenitkaTasks { get; set; } = new Queue<Task<bool>>();

        #endregion Public Properties

        #region Public Methods

        public void Move(Player me, World world, Game game, Move move)
        {
            InitializeTick(me, world, game, move);

            if (me.RemainingActionCooldownTicks != 0) { return; }

            var nuclearAlarm = NuclearEvader.CheckNuclearAlarm();
            if (nuclearAlarm) { return; }

            if (MainGameTasks.Any() && !NuclearAlarm)
            {
                ProcessTask(MainGameTasks.Dequeue());
                return;
            }
        }

        #endregion Public Methods

        #region Private Methods

        private void InitializeTick(Player me, World world, Game game, Move move)
        {
            Me = me;
            World = world;
            Game = game;
            Action = move;

            _turnsPer60Ticks = _turnsPer60TicksMin + 3 * world.Facilities.Count(f => f.OwnerPlayerId == Me.Id && f.Type == FacilityType.ControlCenter);

            if (TurnAfterLast60Tick.Any() && TurnAfterLast60Tick.Peek() <= World.TickIndex - 60)
            {
                TurnAfterLast60Tick.Dequeue();
            }

            foreach (Vehicle vehicle in World.NewVehicles)
            {
                VehicleById[vehicle.Id] = vehicle;
                UpdateTickByVehicleId[vehicle.Id] = world.TickIndex;
            }

            foreach (VehicleUpdate vehicleUpdate in world.VehicleUpdates)
            {
                long vehicleId = vehicleUpdate.Id;

                if (vehicleUpdate.Durability == 0)
                {
                    VehicleById.Remove(vehicleId);
                    UpdateTickByVehicleId.Remove(vehicleId);
                }
                else
                {
                    VehicleById[vehicleId] = new Vehicle(VehicleById[vehicleId], vehicleUpdate);
                    UpdateTickByVehicleId[vehicleId] = world.TickIndex;
                }
            }

            InitStrategy();

            if (World.TickIndex % 16 == 0)
            {
                IndicatorFacillites.Update(World.Facilities);
                GameGrid.Update(EnemyVehicles, World.Facilities);
            }

            if ((World.TickIndex + 64) % 512 == 0)
            {
                foreach (var squad in GroupManager.Squads.Where(s => s.Id >= (int)Group.NewGroup))
                {
                    StrategyController.FindSingleEnemy(squad);
                }
            }

            UpDateDelayedTasks();

            if (Me.RemainingNuclearStrikeCooldownTicks <= 0)
            {
                if (World.TickIndex % 2 == 0)
                {
                    if (!EnemyVehicles.Any() || !MyVehicles.Any()) { return; }

                    foreach (var cell in GameGrid.Grid)
                    {
                        if (cell.EnemyCount > 5)
                        {
                            foreach (var squad in GroupManager.Squads)
                            {
                                if (cell.Distance(squad) < 200)
                                {
                                    var enemies = EnemyVehicles.Where(v =>
                                        v.X > cell.X * 64 && v.X < (cell.X + 1) * 64 && v.Y > cell.Y * 64 &&
                                        v.Y < (cell.Y + 1) * 64 && v.Type != VehicleType.Arrv).ToList();

                                    if (!enemies.Any()) { continue; }

                                    var enemyCenter = enemies.CenterXY();

                                    var navodchick = MyVehicles.FirstOrDefault(v => v.Durability > 90 && v.VisionRange * 0.8 > v.GetDistanceTo(enemyCenter.X, enemyCenter.Y));
                                    if (navodchick != null)
                                    {
                                        MainGameTasks.Enqueue(Act.Nuclear(new MyPoint(enemyCenter.X, enemyCenter.Y), navodchick.Id));
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void InitStrategy()
        {
            if (World.TickIndex == 0)
            {
                DelayTaksBuilder = new MyDelayTaksBuilder(this);
                EnemyStrategyRecognizer = new MyEnemyStrategyRecognizer(this);
                Act = new MyActionMaker(this);
                GameGrid = new MyGameGrid(this);

                GroupManager = new MyGroupManager(this);
                IndicatorFacillites = new MyIndicatorFacilites(this);

                if (World.Facilities.Length == 0)
                {
                    NuclearEvader = new MyFirstRoundNuckearEvader(this);
                    GroupMaker = new MyFirstRoundGroupMaker(this);
                    StrategyController = new MyFirstRoundController(this);
                    GroupMaker.Ready += StrategyController.Process;
                    GroupMaker.Make();
                }
                else if (!Game.IsFogOfWarEnabled)
                {
                    NuclearEvader = new MySecondRoundNuclearEvader(this);
                    GroupMaker = new MySecondRoundGroupMaker(this);
                    StrategyController = new MySecondRoundController(this);
                    GroupMaker.Ready += StrategyController.Process;
                    GroupMaker.Make();
                }
                else
                {
                    NuclearEvader = new MySecondRoundNuclearEvader(this);
                    GroupMaker = new MySecondRoundGroupMaker(this);
                    StrategyController = new MySecondRoundController(this);
                    GroupMaker.Ready += StrategyController.Process;
                    GroupMaker.Make();
                }
            }
        }

        private void ProcessTask(Task<bool> task)
        {
            task.RunSynchronously();
            var isTask = task.Result;
            if (isTask)
            {
                TurnAfterLast60Tick.Enqueue(World.TickIndex);
            }
        }

        private void UpDateDelayedTasks()
        {
            foreach (var unit in UnitsForRemove)
            {
                Units.Remove(unit);
            }
            UnitsForRemove.Clear();

            foreach (var unit in UnitsForAdd)
            {
                Units.Add(unit);
            }
            UnitsForAdd.Clear();

            foreach (var unit in Units)
            {
                unit.CheckDelayedTask();
            }
        }

        #endregion Private Methods
    }
}