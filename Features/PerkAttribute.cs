namespace SwiftArcadeMode.Features
{
    using System;
    using SwiftArcadeMode.Utils.Interfaces;

    public enum PerkRestriction
    {
        None,
        Human,
        SCP,
        DontSpawn,
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class PerkAttribute(string id, Rarity rarity = Rarity.Common, PerkRestriction restriction = PerkRestriction.None, params Type[] conflictPerks) : Attribute, IWeight, IPerkInfo
    {
        public Type Perk { get; set; }

        public readonly Type[] Conflicts = conflictPerks;

        public readonly string ID = id;

        public readonly Rarity Rarity = rarity;

        public readonly PerkRestriction Restriction = restriction;

        public int Weight => (int)Rarity;

        Rarity IPerkInfo.Rarity => Rarity;

        PerkRestriction IPerkInfo.Restriction => Restriction;

        public PerkManager.PerkProfile Profile;

        public bool HasConflicts(PerkInventory perks, out PerkBase perk)
        {
            foreach (PerkBase v in perks.Perks)
            {
                if (Conflicts.Contains(v.GetType()) || (PerkManager.TryGetPerk(v.GetType(), out PerkAttribute att) && att.Conflicts.Contains(Perk)))
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
