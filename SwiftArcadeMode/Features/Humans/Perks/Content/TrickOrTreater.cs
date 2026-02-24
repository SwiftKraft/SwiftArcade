namespace SwiftArcadeMode.Features.Humans.Perks.Content
{
    [Perk("TrickOrTreater", Rarity.Legendary)]
    public class TrickOrTreater(PerkInventory inv) : PerkItemReceiveBase(inv)
    {
        public override string Name => "Trick Or Treater";

        public override string PerkDescription => "Receive a candy. ";

        public override float Cooldown => 25f;

        public override ItemType ItemType => ItemType.SCP330;
    }
}
