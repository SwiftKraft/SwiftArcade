namespace SwiftArcadeMode.Features.Humans.Perks.Content
{
    [Perk("ArmoryKit", Rarity.Uncommon)]
    public class ArmoryKit(PerkInventory inv) : PerkItemReceiveBase(inv)
    {
        public override string Name => "Armory Kit";

        public override string PerkDescription => "Receive a grenade. ";

        public override float Cooldown => 60f;

        public override ItemType ItemType => ItemType.GrenadeHE;
    }
}
