namespace SwiftArcadeMode.Features.Humans.Perks.Content
{
    using CustomPlayerEffects;
    using LabApi.Events.Handlers;

    [Perk("Furnace", Rarity.Uncommon)]
    public class Furnace(PerkInventory inv) : PerkBase(inv)
    {
        public override string Name => "Furnace";

        public override string Description => "Burns people when you damage them. \nYou also get burned when you get damaged.";

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
            if (ev.Attacker == Player || ev.Player == Player)
                ev.Player.EnableEffect<Burned>(1, 1, true);
        }
    }
}
