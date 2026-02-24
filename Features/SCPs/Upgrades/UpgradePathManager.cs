namespace SwiftArcadeMode.Features.SCPs.Upgrades
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using PlayerRoles;
    using SwiftArcadeMode.Utils.Extensions;

    public static class UpgradePathManager
    {
        public static readonly HashSet<UpgradePathAttribute> RegisteredUpgrades = [];

        public static void Enable()
        {
            if (Core.CoreConfig.AllowBaseContent)
                RegisterUpgrades();
        }

        public static void Disable()
        {
        }

        public static void RegisterUpgrades()
        {
            Assembly callingAssembly = Assembly.GetCallingAssembly();

            Dictionary<Type, UpgradePathAttribute> atts = callingAssembly
                .GetTypes()
                .Select(t => (type: t, attr: t.GetCustomAttribute<UpgradePathAttribute>()))
                .Where(pair => pair.attr != null)
                .ToDictionary(pair => pair.type, pair => pair.attr);

            foreach (KeyValuePair<Type, UpgradePathAttribute> attr in atts)
            {
                if (!PerkManager.TryGetPerk(attr.Key, out PerkAttribute? attribute))
                    continue;

                attr.Value.Perk = attribute;
                RegisteredUpgrades.Add(attr.Value);
            }
        }

        extension(RoleTypeId role)
        {
            public UpgradePathAttribute? GetRandomUpgradePath() => RegisteredUpgrades.Where(t => t.Roles.Contains(role)).ToArray().GetWeightedRandom();

            public UpgradePathAttribute? GetRandomUpgradePath(ICollection<UpgradePathAttribute> noRep) => RegisteredUpgrades.Where(t => t.Roles.Contains(role) && !noRep.Contains(t)).ToArray().GetWeightedRandom();
        }
    }
}
