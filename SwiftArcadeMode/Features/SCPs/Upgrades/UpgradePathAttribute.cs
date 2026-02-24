namespace SwiftArcadeMode.Features.SCPs.Upgrades
{
    using System;
    using PlayerRoles;
    using SwiftArcadeMode.Utils.Interfaces;

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class UpgradePathAttribute(params RoleTypeId[] roles) : Attribute, IWeight
    {
        public RoleTypeId[] Roles { get; } = roles;

        // Initialized in UpgradePathManager before this perk can be accessed through arcade mode.
        public PerkAttribute Perk { get; set; } = null!;

        public int Weight => Perk.Weight;
    }
}
