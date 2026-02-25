namespace SwiftArcadeMode
{
    using System;
    using System.IO;
    using System.Linq;
    using AdminToys;
    using CustomPlayerEffects;
    using HarmonyLib;
    using LabApi.Events.Handlers;
    using LabApi.Features;
    using LabApi.Loader.Features.Plugins;
    using MEC;
    using ProjectMER.Events.Arguments;
    using SwiftArcadeMode.Features;
    using SwiftArcadeMode.Features.Game.Modes;
    using SwiftArcadeMode.Features.Humans.Perks;
    using SwiftArcadeMode.Features.Humans.Perks.Crafting;
    using SwiftArcadeMode.Features.Scoring;
    using SwiftArcadeMode.Features.Scoring.Saving;
    using SwiftArcadeMode.Features.SCPs.Upgrades;
    using SwiftArcadeMode.Features.ServerSpecificSettings;
    using SwiftArcadeMode.Utils.Deployable;
    using SwiftArcadeMode.Utils.Effects;
    using SwiftArcadeMode.Utils.Projectiles;
    using SwiftArcadeMode.Utils.Sounds;
    using Logger = LabApi.Features.Console.Logger;

    public class Core : Plugin<Config>
    {
        public static Harmony Harmony { get; private set; } = null!;

        public static Core Instance { get; private set; } = null!;

        public static Config CoreConfig { get; private set; } = null!;

        public override string Name => "SCPSL Arcade Mode";

        public override string Description => "Adds various interesting and fun mechanics to the game! ";

        public override string Author => "SwiftKraft";

        public override Version Version => new(2, 4, 0);

        public override Version RequiredApiVersion => new(LabApiProperties.CompiledVersion);

        public override void Enable()
        {
            Instance = this;

            if (Config is null)
            {
                Logger.Error("Failed to load config!");
                return;
            }

            CoreConfig = Config;

            Logger.Info($"Arcade Mode {Version} by SwiftKraft: Loaded!");

            Harmony = new Harmony(Name);
            Harmony.PatchAll();

            SaveManager.GeneralDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SCP Secret Laboratory", "Swift Arcade Mode");
            SaveManager.SaveDirectory = Path.Combine(SaveManager.GeneralDirectory, "Scoring");
            SaveManager.SaveFileName = "scores.txt";
            SaveManager.SavePath = Path.Combine(SaveManager.SaveDirectory, SaveManager.SaveFileName);

            string soundEffectDir = Path.Combine(SaveManager.GeneralDirectory, "Sounds");
            Directory.CreateDirectory(soundEffectDir);
            SoundEffectManager.BasePath = soundEffectDir;
            SoundEffectManager.DebugLogs = Config.SoundLogs;

            if (AppDomain.CurrentDomain.GetAssemblies().All(assembly => !assembly.FullName.Contains("AudioPlayerApi")))
                SoundEffectManager.Disable();

            Logger.Info($"Scoring save file path: {SaveManager.SavePath}");

            StaticUnityMethods.OnFixedUpdate += FixedUpdate;

            PerkManager.Enable();
            PerkSpawner.Enable();
            UpgradePathManager.Enable();
            UpgradePathGiver.Enable();
            ScoringManager.Enable();
            RecipeManager.Enable();
            SSSManager.Enable();
            GameModeManager.Enable();
            CustomEffectManager.Enable();

            ServerEvents.RoundEnded += OnRoundEnded;
            ServerEvents.RoundStarted += OnRoundStarted;
            ServerEvents.RoundRestarted += OnRoundRestarted;
            ServerEvents.MapGenerated += OnMapGenerated;
            Shutdown.OnQuit += OnQuit;
            PlayerEvents.UpdatedEffect += OnUpdatedEffect;
            PlayerEvents.ChangedRole += OnChangedRole;
            PlayerEvents.Left += OnLeft;
            ProjectMER.Events.Handlers.Schematic.SchematicSpawned += OnSchematicSpawned;

            SaveManager.LoadScores();
        }

        public override void Disable()
        {
            Harmony.UnpatchAll(Harmony.Id);

            StaticUnityMethods.OnFixedUpdate -= FixedUpdate;

            PerkManager.Disable();
            PerkSpawner.Disable();
            UpgradePathManager.Disable();
            UpgradePathGiver.Disable();
            ScoringManager.Disable();
            SSSManager.Disable();
            GameModeManager.Disable();
            CustomEffectManager.Disable();

            ServerEvents.RoundEnded -= OnRoundEnded;
            ServerEvents.RoundStarted -= OnRoundStarted;
            ServerEvents.RoundRestarted -= OnRoundRestarted;
            ServerEvents.MapGenerated -= OnMapGenerated;
            Shutdown.OnQuit -= OnQuit;
            PlayerEvents.UpdatedEffect -= OnUpdatedEffect;
            PlayerEvents.ChangedRole -= OnChangedRole;
            PlayerEvents.Left -= OnLeft;

            ProjectMER.Events.Handlers.Schematic.SchematicSpawned -= OnSchematicSpawned;

            SaveManager.SaveScores();
        }

        private static void OnSchematicSpawned(SchematicSpawnedEventArgs ev)
        {
            if (!CoreConfig.SpeedUpSchematics)
                return;

            ev.Schematic.gameObject.GetComponent<AdminToyBase>().syncInterval = 0;

            foreach (AdminToyBase toyBase in ev.Schematic.AdminToyBases.Where(toy => toy.name.StartsWith("anim_")))
            {
                toyBase.syncInterval = 0;
            }
        }

        private static void OnRoundRestarted() => SaveManager.SaveScores();

        private static void OnLeft(LabApi.Events.Arguments.PlayerEvents.PlayerLeftEventArgs ev)
        {
            if (ev.Player.TryGetPerkInventory(out PerkInventory inv))
                inv.ClearPerks();
        }

        private static void OnMapGenerated(LabApi.Events.Arguments.ServerEvents.MapGeneratedEventArgs ev) => SaveManager.LoadScores();

        private static void OnRoundStarted() => SaveManager.LoadScores();

        private static void OnRoundEnded(LabApi.Events.Arguments.ServerEvents.RoundEndedEventArgs ev) => SaveManager.SaveScores();

        private static void OnChangedRole(LabApi.Events.Arguments.PlayerEvents.PlayerChangedRoleEventArgs ev)
        {
            if (CoreConfig.Replace096 && ev.NewRole.RoleTypeId == PlayerRoles.RoleTypeId.Scp096 && ev.ChangeReason == PlayerRoles.RoleChangeReason.RoundStart)
                Timing.CallDelayed(0.1f, () => ev.Player.SetRole(PlayerRoles.RoleTypeId.Scp3114));
        }

        private static void OnUpdatedEffect(LabApi.Events.Arguments.PlayerEvents.PlayerEffectUpdatedEventArgs ev)
        {
            if (CoreConfig.SkeletonBalance && ev.Effect is Strangled str)
                str.ServerChangeDuration(2f);
        }

        private static void OnQuit() => SaveManager.SaveScores();

        private static void FixedUpdate()
        {
            GameModeManager.Tick();
            PerkManager.Tick();
            ProjectileManager.Tick();
            ScoringManager.Tick();
            CustomEffectManager.Tick();
            DeployableManager.Tick();
        }

        /*
        Ideas:

        Random round events and mini modes.
        */
    }
}
