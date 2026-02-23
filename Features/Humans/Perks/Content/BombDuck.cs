namespace SwiftArcadeMode.Features.Humans.Perks.Content
{
    using LabApi.Events.Handlers;
    using PlayerStatsSystem;

    [Perk("BombDuck", Rarity.Legendary, PerkRestriction.DontSpawn)]
    public class BombDuck(PerkInventory inv) : PerkBase(inv)
    {
        public override string Name => "Bomb Duck";

        public override string Description => "Your grenades don't damage you. ";

        public override void Init()
        {
            base.Init();
            PlayerEvents.Hurting += OnHurting;
        }

        public override void Remove()
        {
            base.Remove();
            PlayerEvents.Hurting -= OnHurting;
        }

        private void OnHurting(LabApi.Events.Arguments.PlayerEvents.PlayerHurtingEventArgs ev)
        {
            if (ev.Player != Player || ev.DamageHandler is not ExplosionDamageHandler || ev.Attacker != Player)
                return;

            ev.IsAllowed = false;
        }
    }
}
