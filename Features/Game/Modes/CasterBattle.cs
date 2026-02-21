using LabApi.Events.Handlers;
using LabApi.Features.Wrappers;
using SwiftArcadeMode.Features.Humans.Perks;
using SwiftArcadeMode.Features.Humans.Perks.Content;
using SwiftArcadeMode.Features.Humans.Perks.Content.Caster;
using System;

namespace SwiftArcadeMode.Features.Game.Modes
{
    public class CasterBattle : GameModeBase
    {
        public override PerkSpawnRulesBase OverrideSpawnRules => new PerkRules();

        public override void End() => PlayerEvents.ChangedRole -= OnChangedRole;

        public override void Start()
        {
            Server.SendBroadcast("CASTER BATTLE MODE\nHas been activated.", 10, Broadcast.BroadcastFlags.Normal, true);

            PlayerEvents.ChangedRole += OnChangedRole;
        }

        private void OnChangedRole(LabApi.Events.Arguments.PlayerEvents.PlayerChangedRoleEventArgs ev)
        {
            ev.Player.MaxHealth *= 2.5f;
            ev.Player.Health = ev.Player.MaxHealth;
        }

        public override void Tick() { }

        public class PerkRules : PerkSpawnRulesBasic
        {
            public static Type[] Pool = [
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

            public override Func<PerkAttribute, bool> Criteria => (p) => Pool.Contains(p.Perk);
        }
    }
}
