namespace SwiftArcadeMode.Features.Game.Modes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using LabApi.Events.Handlers;
    using SwiftArcadeMode.Features.Humans.Perks;
    using SwiftArcadeMode.Utils.Extensions;
    using Random = UnityEngine.Random;

    public static class GameModeManager
    {
        public static float Chance { get; set; }

        public static bool GameModesAllowed { get; set; }

        public static bool NextRoundForced { get; set; }

        public static List<Type> Registry { get; } = [];

        public static GameModeBase? Current
        {
            get;
            set
            {
                if (field == value)
                    return;

                field?.End();
                field = value;
                PerkSpawner.PerkSpawnRules = field is { OverrideSpawnRules: not null }
                    ? field.OverrideSpawnRules
                    : PerkSpawner.DefaultSpawnRules;
                field?.Start();
            }
        }

        public static void Enable()
        {
            GameModesAllowed = Core.CoreConfig.AllowCustomGameModes;
            Chance = Core.CoreConfig.CustomGameModeChance;

            RegisterModes();

            ServerEvents.RoundStarted += OnRoundStarted;
            ServerEvents.RoundRestarted += OnRoundRestarted;
        }

        public static void Disable()
        {
            ServerEvents.RoundStarted -= OnRoundStarted;
            ServerEvents.RoundRestarted -= OnRoundRestarted;
        }

        public static void RegisterModes()
        {
            Assembly callingAssembly = Assembly.GetCallingAssembly();

            List<Type> types = callingAssembly
                .GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && typeof(GameModeBase).IsAssignableFrom(t))
                .ToList();

            foreach (Type t in types)
                Registry.Add(t);
        }

        public static void Tick() => Current?.Tick();

        private static void OnRoundRestarted()
        {
            Current = null;
            NextRoundForced = false;
        }

        private static void OnRoundStarted()
        {
            if (NextRoundForced || !GameModesAllowed)
                return;

            Current = null;
            if (Random.Range(0f, 1f) <= Chance && Registry.Count > 0)
                Current = (GameModeBase)Activator.CreateInstance(Registry.GetRandom()!);
        }
    }
}
