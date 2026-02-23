namespace SwiftArcadeMode.Features.SCPs.Upgrades.Content.SCP106.EndlessDecay
{
    using System;
    using PlayerRoles;

    [UpgradePath(RoleTypeId.Scp106)]
    [Perk("106.EndlessDecay", Rarity.Epic, PerkRestriction.SCP)]
    public class EndlessDecay(PerkInventory inv) : UpgradePathPerkBase(inv)
    {
        public override string PathName => "Endless Decay";

        public override string PathDescription => "Decays various things.";

        public override Type[] AllUpgrades => [
            typeof(FleshDecay),
            typeof(LimbBreak),
            typeof(GunMelter)
            ];
    }
}
