namespace SwiftArcadeMode.Features.Humans.Perks.Crafting
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Hints;
    using LabApi.Features.Wrappers;
    using SwiftArcadeMode.Features.Humans.Perks.Content;
    using SwiftArcadeMode.Utils.Interfaces;

    public class Recipe(int weight, string message, Type result, params Type[] perks) : IWeight
    {
        public string Message { get; } = message;

        public int Weight { get; } = weight;

        public Type[] RequiredPerks { get; } = perks;

        public PerkAttribute ResultingPerk { get; }
            = PerkManager.GetPerk(result) ?? throw new InvalidOperationException("Recipe crafted from perks:\n" + perks
                .Select(perk => perk.Name)
                .Aggregate((curr, next) => curr + "\n- " + next) + "\nhad an invalid result!");

        int IWeight.Weight => Weight;

        public bool CanCraft(Player p) => p.TryGetPerkInventory(out PerkInventory inv) && CanCraft(inv);

        public bool CanCraft(PerkInventory inv)
        {
            if (inv.HasPerk(ResultingPerk.Perk))
                return false;

            for (int i = 0; i < RequiredPerks.Length; i++)
            {
                if (!inv.HasPerk(RequiredPerks[i]))
                    return false;
            }

            return true;
        }

        public bool ApplyCraft(Player p) => p.TryGetPerkInventory(out PerkInventory inv) && ApplyCraft(inv);

        public bool ApplyCraft(PerkInventory inv)
        {
            if (!CanCraft(inv))
                return false;

            for (int i = 0; i < RequiredPerks.Length; i++)
                inv.RemovePerk(RequiredPerks[i]);
            bool success = inv.AddPerk(ResultingPerk);

            if (!success)
                PerkSpawner.SpawnPerk(ResultingPerk, inv.Parent.Position);
            else
                inv.Parent.SendHint(Message, [HintEffectPresets.FadeOut()]);

            return success;
        }
    }
}
