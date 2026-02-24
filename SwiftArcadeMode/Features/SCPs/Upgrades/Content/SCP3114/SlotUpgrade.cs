namespace SwiftArcadeMode.Features.SCPs.Upgrades.Content.SCP3114
{
    using SwiftArcadeMode.Features.Humans.Perks.Content;

    public class SlotUpgrade(UpgradePathPerkBase parent) : UpgradeBase<RandomPerks>(parent)
    {
        public override string Name => $"Slot Upgrade";

        public override string Description => "Get a slot upgrade.";

        public override void Init()
        {
            base.Init();
            if (!Player.HasPerk(typeof(PerkSlotUpgrade)))
                Player.GivePerk(typeof(PerkSlotUpgrade));
        }
    }
}
