namespace SwiftArcadeMode.Features.Humans.Perks.Content
{
    using CustomPlayerEffects;
    using InventorySystem.Items.Firearms.Modules;
    using LabApi.Features.Wrappers;

    [Perk("FastHands", Rarity.Common)]
    public class FastHands(PerkInventory inv) : PerkBase(inv)
    {
        private byte originalIntensity;

        public override string Name => "Fast Hands";

        public override string Description => "Reload and unload weapons faster.";

        public bool Reloading
        {
            get => field;
            private set
            {
                if (value == field)
                    return;

                field = value;

                if (field)
                {
                    originalIntensity = Player.GetEffect<Scp1853>()!.Intensity;
                    field = Player.TryGetEffect(out Poisoned? pois) && pois.IsEnabled;
                }

                Player.EnableEffect<Scp1853>(field ? (byte)50 : originalIntensity);

                if (!field)
                    Player.DisableEffect<Poisoned>();
            }
        }

        public override void Tick()
        {
            base.Tick();
            Reloading = Player.CurrentItem is FirearmItem f && f.Base.TryGetModule(out IReloaderModule mod) && mod.IsReloadingOrUnloading;
        }
    }
}
