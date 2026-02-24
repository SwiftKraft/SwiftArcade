namespace SwiftArcadeMode.Features
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using SwiftArcadeMode.Utils.Interfaces;

    public enum PerkRestriction
    {
        None,
        Human,
        SCP,
        DontSpawn,
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class PerkAttribute : Attribute, IWeight, IPerkInfo
    {
        public PerkAttribute(string id, Rarity rarity = Rarity.Common, PerkRestriction restriction = PerkRestriction.None, params Type[] conflictPerks)
        {
            ID = id;
            Rarity = rarity;
            Restriction = restriction;
            Conflicts = conflictPerks;
        }

        // assigned in perk manager shortly after the attribute it made accessible through arcade mode.
        public Type Perk { get; internal set; } = null!;

        public string ID { get; }

        public Rarity Rarity { get; }

        [Obsolete("This value is useless. Use GetFancyName / GetName on your target perk instead.")]
        public PerkProfile Profile { get; set; }

        /// <summary>
        /// Gets an instance of the perk this attribute is attached to with no owner or perk inventory. This is used to generate names and descriptions for players.
        /// </summary>
        public PerkBase HollowInstance { get; internal set; } = null!;

        public PerkRestriction Restriction { get; }

        public Type[] Conflicts { get; }

        public int Weight => (int)Rarity;

        Rarity IPerkInfo.Rarity => Rarity;

        PerkRestriction IPerkInfo.Restriction => Restriction;

        public bool HasConflicts(PerkInventory perks, [NotNullWhen(true)] out PerkBase? perk)
        {
            foreach (PerkBase v in perks.Perks)
            {
                if (Conflicts.Contains(v.GetType()) || (PerkManager.TryGetPerk(v.GetType(), out PerkAttribute? att) && att.Conflicts.Contains(Perk)))
                {
                    perk = v;
                    return true;
                }
            }

            perk = null;
            return false;
        }
    }
}
