namespace SwiftArcadeMode.Features.Humans.Perks.Content
{
    using LabApi.Features.Wrappers;

    [Perk("TrickOrTreater", Rarity.Legendary)]
    public class TrickOrTreater(PerkInventory inv) : PerkItemReceiveBase(inv)
    {
        public override string Name => "Trick Or Treater";

        public override string PerkDescription => "Receive a candy. ";

        public override ItemType ItemType => ItemType.SCP330;

        public override float GetCooldown(Player player) => 25f;
    }
}
