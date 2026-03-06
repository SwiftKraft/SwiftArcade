namespace SwiftArcadeMode.Features.Humans.Perks.Content
{
    using LabApi.Events.Handlers;
    using PlayerStatsSystem;

    [Perk("Resilience", Rarity.Uncommon)]
    public class Resilience(PerkInventory inv) : PerkBase(inv)
    {
        public override string Name => "Resilience";

        public override string Description => "What doesn't kill you makes you stronger.\nTaking damage increases your max health.\nResets on death.";

        public virtual float Percentage => 0.1f;

        public virtual float Cap => 400f;

        public override void Init()
        {
            base.Init();
            PlayerEvents.Hurt += OnHurt;
        }

        public override void Remove()
        {
            base.Remove();
            PlayerEvents.Hurt -= OnHurt;
        }

        private void OnHurt(LabApi.Events.Arguments.PlayerEvents.PlayerHurtEventArgs ev)
        {
            if (ev.Player != Player || ev.DamageHandler is not StandardDamageHandler dmg)
                return;

            if (Player.MaxHealth < Cap)
                Player.MaxHealth += dmg.Damage * Percentage;
        }
    }
}
