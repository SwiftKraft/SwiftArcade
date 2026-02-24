namespace SwiftArcadeMode.Features.Game.Modes
{
    using System;
    using LabApi.Events.Handlers;
    using LabApi.Features.Wrappers;
    using SwiftArcadeMode.Features.Humans.Perks;
    using SwiftArcadeMode.Features.Humans.Perks.Content;
    using SwiftArcadeMode.Features.Humans.Perks.Content.Caster.Perks;

    public class CasterBattle : GameModeBase
    {
        public override PerkSpawnRulesBase OverrideSpawnRules => new PerkRules();

        public override void End() => PlayerEvents.ChangedRole -= OnChangedRole;

        public override void Start()
        {
            Server.SendBroadcast("CASTER BATTLE MODE\nHas been activated.", 10, Broadcast.BroadcastFlags.Normal, true);

            PlayerEvents.ChangedRole += OnChangedRole;
        }

        public override void Tick()
        {
        }

        private static void OnChangedRole(LabApi.Events.Arguments.PlayerEvents.PlayerChangedRoleEventArgs ev)
        {
            ev.Player.MaxHealth *= 2.5f;
            ev.Player.Health = ev.Player.MaxHealth;
        }

        public class PerkRules : PerkSpawnRulesBasic
        {
            public static Type[] Pool { get; } =
            [
                typeof(SuperRegeneration),
                typeof(PerkSlotUpgrade),
                typeof(Ninjutsu),
                typeof(Vampire),
                typeof(Resilience),
                typeof(Streamer),
                typeof(Wizard),
                typeof(Druid),
                typeof(Sorcerer),
                typeof(Warlock)
            ];

            public override Func<PerkAttribute, bool> Criteria => p => Pool.Contains(p.Perk);
        }
    }
}
