namespace SwiftArcadeMode.Features.SCPs.Upgrades
{
    using System;
    using System.Linq;
    using LabApi.Events.Arguments.PlayerEvents;
    using LabApi.Events.Handlers;
    using LabApi.Features.Wrappers;
    using PlayerStatsSystem;
    using SwiftArcadeMode.Features.Events;

    public static class UpgradePathGiver
    {
        public static bool AllowLeveling { get; set; } = true;

        public static int SCPTeamExperience
        {
            get;
            set
            {
                if (field == value || !AllowLeveling)
                    return;

                field = value;

                while (field >= Requirement)
                {
                    field -= Requirement;
                    SCPLevel++;
                }
            }
        }

        public static int Requirement => SCPLevel * 4;

        public static int SCPLevel
        {
            get;
            set
            {
                if (field == value)
                    return;

                if (field < value)
                {
                    for (int i = 0; i < value - field; i++)
                    {
                        foreach (Player p in Player.List)
                        {
                            if (p.IsSCP && p.TryGetPerkInventory(out PerkInventory inv))
                            {
                                inv.UpgradeQueue.Create(
                                    3,
                                    UpgradePathManager.RegisteredUpgrades
                                        .Where(u => inv.TryGetPerk(u.Perk.Perk, out PerkBase? perkBase) &&
                                                    perkBase is UpgradePathPerkBase { Maxed: true })
                                        .ToList());

                                p.SendBroadcast("SCP Team Leveled Up! \nCurrent Level: " + value, 5);
                                p.Heal(p.MaxHealth * 0.2f);
                            }
                        }
                    }
                }

                PerkEvents.OnScpTeamLevelUp(new ScpTeamLevelUpEventArgs(field, value));
                field = value;
            }
        }

        = 1;

        public static void Enable()
        {
            PlayerEvents.Dying += OnPlayerDying;
            ServerEvents.WaveRespawned += OnWaveRespawned;
            ServerEvents.RoundStarted += OnRoundStarted;
            AllowLeveling = Core.CoreConfig.AllowScpLeveling;
        }

        public static void Disable()
        {
            PlayerEvents.Dying -= OnPlayerDying;
            ServerEvents.WaveRespawned -= OnWaveRespawned;
            ServerEvents.RoundStarted -= OnRoundStarted;
        }

        private static void OnRoundStarted()
        {
            SCPLevel = 0;
            SCPTeamExperience = 0;
        }

        private static void OnWaveRespawned(LabApi.Events.Arguments.ServerEvents.WaveRespawnedEventArgs ev)
        {
            if (ev.Players.Count > 4)
                SCPLevel++;
        }

        private static void OnPlayerDying(PlayerDyingEventArgs ev)
        {
            if (ev.Player.IsHuman && ((ev.Attacker != null && ev.Attacker.IsSCP) || (ev.DamageHandler is UniversalDamageHandler dmg && dmg.TranslationId == DeathTranslations.PocketDecay.Id)))
            {
                SCPTeamExperience++;
                ev.Attacker?.SendHitMarker(2f);
            }
        }

        public class ScpTeamLevelUpEventArgs(int prevLevel, int newLevel) : EventArgs
        {
            public int PreviousLevel { get; } = prevLevel;

            public int NewLevel { get; } = newLevel;
        }
    }
}
