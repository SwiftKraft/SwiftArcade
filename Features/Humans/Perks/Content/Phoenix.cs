namespace SwiftArcadeMode.Features.Humans.Perks.Content
{
    using CustomPlayerEffects;
    using LabApi.Events.Handlers;

    [Perk("Phoenix", Rarity.Legendary)]
    public class Phoenix(PerkInventory inv) : PerkBase(inv)
    {
        public override string Name => "Phoenix";

        public override string Description => "Saves you from death once per life. Get AHP and cardiac arrest on save.";

        public bool Triggered { get; private set; }

        public override void Init()
        {
            base.Init();
            PlayerEvents.Dying += OnPlayerDying;
            PlayerEvents.Death += OnPlayerDeath;
        }

        private void OnPlayerDeath(LabApi.Events.Arguments.PlayerEvents.PlayerDeathEventArgs ev)
        {
            if (ev.Player != Player)
                return;

            Triggered = false;
        }

        private void OnPlayerDying(LabApi.Events.Arguments.PlayerEvents.PlayerDyingEventArgs ev)
        {
            if (Triggered || ev.Player != Player)
                return;

            Triggered = true;
            ev.IsAllowed = false;

            if (Player.IsSCP)
                Player.MaxHealth = 500f;

            Player.Heal(500f);
            Player.ArtificialHealth += 75f;
            Player.EnableEffect<CardiacArrest>(1, 20f);
        }

        public override void Remove()
        {
            base.Remove();
            PlayerEvents.Dying -= OnPlayerDying;
            PlayerEvents.Death -= OnPlayerDeath;
        }
    }
}
