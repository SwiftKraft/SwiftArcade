namespace SwiftArcadeMode.Features.Humans.Perks.Content
{
    [Perk("Raise", Rarity.Rare)]
    public class Raise(PerkInventory inv) : PerkBase(inv)
    {
        public override string Name => "Raise";

        public override string Description => "You will receive a raise.";
    }
}
