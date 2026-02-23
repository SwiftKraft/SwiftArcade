namespace SwiftArcadeMode.Features.SCPs.Upgrades.Content.SCP049.Overlord
{
    using System;
    using PlayerRoles;

    [UpgradePath(RoleTypeId.Scp049)]
    [Perk("049.Overlord", Rarity.Epic, PerkRestriction.SCP)]
    public class Overlord(PerkInventory inv) : UpgradePathPerkBase(inv)
    {
        public override string PathName => "Overlord";

        public override string PathDescription => "Abuse your zombies!";

        public override Type[] AllUpgrades => [
            typeof(Siphon),
            typeof(Soulbound)
            ];
    }
}
