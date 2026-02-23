namespace SwiftArcadeMode.Features.Humans.Perks.Content
{
    using LabApi.Events.Handlers;

    [Perk("SuperRegeneration", Rarity.Epic)]
    public class SuperRegeneration(PerkInventory inv) : Regeneration(inv)
    {
        public override string Name => $"Super {base.Name}";

        public override string Description => $"{base.Description} \nHowever, max HP is decreased by {DecreasePercentage * 100f}%.";

        public override float HealthThresholdPercentage => 0.35f;

        public override float Rate => 9f;

        public virtual float DecreasePercentage => 0.1f;

        private float originalHealth;

        public override void Init()
        {
            base.Init();
            originalHealth = Player.MaxHealth;
            Player.MaxHealth = originalHealth - DecreasePercentage * originalHealth;

            PlayerEvents.ChangedRole += OnPlayerChangedRole;
        }

        private void OnPlayerChangedRole(LabApi.Events.Arguments.PlayerEvents.PlayerChangedRoleEventArgs ev)
        {
            if (ev.Player != Player)
                return;

            originalHealth = Player.MaxHealth;
            Player.MaxHealth = originalHealth - DecreasePercentage * originalHealth;
        }

        public override void Remove()
        {
            base.Remove();

            Player.MaxHealth = originalHealth;

            PlayerEvents.ChangedRole -= OnPlayerChangedRole;
        }
    }
}
