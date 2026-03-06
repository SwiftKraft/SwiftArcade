namespace SwiftArcadeMode.Features.SCPs.Upgrades.Content.SCP106.TeamPlayer
{
    using System;
    using PlayerRoles;

    [UpgradePath(RoleTypeId.Scp106)]
    [Perk("106.TeamPlayer", Rarity.Legendary, PerkRestriction.SCP)]
    public class TeamPlayer(PerkInventory inv) : UpgradePathPerkBase(inv)
    {
        public override string PathName => "Team Player";

        public override string PathDescription => "Provides team bonuses.";

        public override Type[] AllUpgrades => [
            typeof(LifeConsumption),
            typeof(SharedConsumption)
            ];
    }
}
