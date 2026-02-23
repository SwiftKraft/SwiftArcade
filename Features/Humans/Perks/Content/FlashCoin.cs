namespace SwiftArcadeMode.Features.Humans.Perks.Content
{
    using CustomPlayerEffects;

    [Perk("FlashCoin", Rarity.Legendary)]
    public class FlashCoin(PerkInventory inv) : PerkBase(inv)
    {
        public override string Name => "God Mode";

        public override string Description => "The best perk.";

        public override void Init()
        {
            base.Init();
            Player.EnableEffect<Flashed>(1, 10, true);
        }
    }
}
