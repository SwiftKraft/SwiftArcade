namespace SwiftArcadeMode.Features.SCPs.Upgrades.Content.SCP3114
{
    using System;
    using PlayerRoles;

    [UpgradePath(RoleTypeId.Scp3114)]
    [Perk("3114.RandomPerks", Rarity.Legendary, PerkRestriction.SCP)]
    public class RandomPerks(PerkInventory inv) : UpgradePathPerkBase(inv)
    {
        public override int SlotUsage => 0;

        public override string PathName => "Random Perks";

        public override string PathDescription => "Gives you a random perk every upgrade. \nThis upgrade path takes up 0 slots.";

        public override Type[] AllUpgrades => [
            typeof(SlotUpgrade),
            typeof(RandomPerk),
            typeof(RandomPerk),
            typeof(RandomPerk),
            typeof(RandomPerk),
            ];
    }
}
