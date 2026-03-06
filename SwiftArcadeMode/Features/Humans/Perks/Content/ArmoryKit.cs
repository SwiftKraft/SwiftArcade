namespace SwiftArcadeMode.Features.Humans.Perks.Content
{
    using LabApi.Features.Wrappers;

    [Perk("ArmoryKit", Rarity.Uncommon)]
    public class ArmoryKit(PerkInventory inv) : PerkItemReceiveBase(inv)
    {
        public override string Name => "Armory Kit";

        public override string PerkDescription => "Receive a grenade. ";

        public override ItemType ItemType => ItemType.GrenadeHE;

        public override float GetCooldown(Player player) => 60f;
    }
}
