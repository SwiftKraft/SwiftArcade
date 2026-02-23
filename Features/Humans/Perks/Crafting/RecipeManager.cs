namespace SwiftArcadeMode.Features.Humans.Perks.Crafting
{
    using System;
    using System.Collections.Generic;
    using Hints;
    using LabApi.Features.Wrappers;
    using SwiftArcadeMode.Features.Humans.Perks.Content;
    using SwiftArcadeMode.Utils.Interfaces;

    public static class RecipeManager
    {
        public static readonly List<Recipe> All = [];

        public static readonly Recipe[] BaseContent = [
            new Recipe(2, "What a level up! ", typeof(SuperRegeneration), typeof(Regeneration), typeof(Raise)),
            new Recipe(3, "More attacks.", typeof(MicroMissiles), typeof(Rocketeer), typeof(Raise)),
            new Recipe(5, "Become the undying.", typeof(Streamer), typeof(FlashCoin), typeof(SuperRegeneration)),
            new Recipe(4, "Faster and faster.", typeof(RaceCar), typeof(HitAndRun), typeof(Raise)),
            new Recipe(1, "Ducks.", typeof(BombDuck), typeof(BombHen), typeof(Raise))
            ];

        public static void Enable()
        {
            if (!Core.Instance.Config.AllowBaseContent)
                return;

            for (int i = 0; i < BaseContent.Length; i++)
                BaseContent[i].AddRecipe();
        }

        public static void CheckCrafts(this Player p)
        {
            if (!Player.TryGetPerkInventory(out PerkInventory inv))
                return;

            for (int i = 0; i < All.Count; i++)
            {
                if (All[i].ApplyCraft(inv))
                    break;
            }
        }

        public static void AddRecipe(this Recipe rec)
        {
            All.Add(rec);
            All.Sort((a, b) => a.Weight.CompareTo(b.Weight));
        }

        public static bool RemoveRecipe(this Recipe rec) => All.Remove(rec);
    }

    public class Recipe(int weight, string message, Type result, params Type[] perks) : IWeight
    {
        public readonly string Message = message;
        public readonly int Weight = weight;

        public readonly Type[] RequiredPerks = perks;
        public readonly PerkAttribute ResultingPerk = PerkManager.GetPerk(result);

        int IWeight.Weight => Weight;

        public bool CanCraft(Player p) => Player.TryGetPerkInventory(out PerkInventory inv) && CanCraft(inv);

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

        public bool ApplyCraft(Player p) => Player.TryGetPerkInventory(out PerkInventory inv) && ApplyCraft(inv);

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
