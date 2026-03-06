namespace SwiftArcadeMode.Features.SCPs.Upgrades.Content.SCP173.Scouter
{
    using System;
    using PlayerRoles;

    [UpgradePath(RoleTypeId.Scp173)]
    [Perk("173.Scouter", Rarity.Rare, PerkRestriction.SCP)]
    public class Scouter(PerkInventory inv) : UpgradePathPerkBase(inv)
    {
        public override string PathName => "Scouter";

        public override string PathDescription => "Focuses on mobility and information gathering.";

        public override Type[] AllUpgrades => [
            typeof(TacticalRetreat),
            typeof(Phantom),
            ];
    }
}
