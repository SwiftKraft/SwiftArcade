namespace SwiftArcadeMode.Features.SCPs.Upgrades.Content.SCP939.Diseased
{
    using System;

    [UpgradePath(PlayerRoles.RoleTypeId.Scp939)]
    [Perk("939.Diseased", Rarity.Epic, PerkRestriction.SCP)]
    public class Diseased(PerkInventory inv) : UpgradePathPerkBase(inv)
    {
        public override string PathName => "Diseased";

        public override string PathDescription => "Spread rabies.";

        public override Type[] AllUpgrades => [
            typeof(PoisonCloud),
            typeof(Bleeding)
            ];
    }
}
