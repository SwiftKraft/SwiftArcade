namespace SwiftArcadeMode.Features.SCPs.Upgrades.Content.SCP173.Tank
{
    using System;
    using PlayerRoles;

    [UpgradePath(RoleTypeId.Scp173)]
    [Perk("173.Tank", Rarity.Rare, PerkRestriction.SCP)]
    public class Tank(PerkInventory inv) : UpgradePathPerkBase(inv)
    {
        public override string PathName => "Tank";

        public override string PathDescription => "Focuses on mitigating damage.";

        public override Type[] AllUpgrades => [
            typeof(DefensiveSpeed),
            typeof(BlastResistant)
            ];
    }
}
