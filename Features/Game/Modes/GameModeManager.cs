using LabApi.Events.Handlers;
using SwiftArcadeMode.Features.Humans.Perks;
using SwiftArcadeMode.Utils.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Random = UnityEngine.Random;

namespace SwiftArcadeMode.Features.Game.Modes
{
    public static class GameModeManager
    {
        public static float Chance { get; set; } = 0.25f;
        public static bool Allow { get; set; } = true;

        public static readonly List<Type> Registry = [];

        public static GameModeBase Current
        {
            get => current;
            set
            {
                if (current == value)
                    return;

                current?.End();
                current = value;
                PerkSpawner.PerkSpawnRules = current != null && current.OverrideSpawnRules != null ? current.OverrideSpawnRules : PerkSpawner.DefaultSpawnRules;
                current?.Start();
            }
        }
        private static GameModeBase current;

        public static bool ForcedRound = false;

        public static void Enable()
        {
            Allow = Core.Instance.Config.AllowCustomGameModes;
            Chance = Core.Instance.Config.CustomGameModeChance;

            RegisterModes();

            ServerEvents.RoundStarted += OnRoundStarted;
            ServerEvents.RoundRestarted += OnRoundRestarted;
        }

        private static void OnRoundRestarted()
        {
            Current = null;
            ForcedRound = false;
        }

        private static void OnRoundStarted()
        {
            if (ForcedRound || !Allow)
                return;

            Current = null;
            if (Random.Range(0f, 1f) <= Chance && Registry.Count > 0)
                Current = (GameModeBase)Activator.CreateInstance(Registry.GetRandom());
        }

        public static void Disable()
        {
            ServerEvents.RoundStarted -= OnRoundStarted;
            ServerEvents.RoundRestarted -= OnRoundRestarted;
        }

        public static void RegisterModes()
        {
            Assembly callingAssembly = Assembly.GetCallingAssembly();

            List<Type> types =
                [.. callingAssembly
                .GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && typeof(GameModeBase).IsAssignableFrom(t))];

            foreach (Type t in types)
                Registry.Add(t);
        }

        public static void Tick() => Current?.Tick();
    }
}
