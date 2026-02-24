namespace SwiftArcadeMode.Features.SCPs.Upgrades.Content.SCP939.Speedster
{
    using System;

    [UpgradePath(PlayerRoles.RoleTypeId.Scp939)]
    [Perk("939.Speedster", Rarity.Uncommon, PerkRestriction.SCP)]
    public class Speedster(PerkInventory inv) : UpgradePathPerkBase(inv)
    {
        public override string PathName => "Speedster";

        public override string PathDescription => "Focuses on speed.";

        public override Type[] AllUpgrades => [
            typeof(Endurance),
            typeof(ReadyUp)
            ];
    }
}
