namespace SwiftArcadeMode.Features.Humans.Perks.Crafting
{
    using System.Collections.Generic;
    using LabApi.Features.Wrappers;
    using SwiftArcadeMode.Features.Humans.Perks.Content;

    public static class RecipeManager
    {
        public static readonly List<Recipe> All = [];

        public static readonly Recipe[] BaseContent = [
            new(2, "What a level up! ", typeof(SuperRegeneration), typeof(Regeneration), typeof(Raise)),
            new(3, "More attacks.", typeof(MicroMissiles), typeof(Rocketeer), typeof(Raise)),
            new(5, "Become the undying.", typeof(Streamer), typeof(FlashCoin), typeof(SuperRegeneration)),
            new(4, "Faster and faster.", typeof(RaceCar), typeof(HitAndRun), typeof(Raise)),
            new(1, "Ducks.", typeof(BombDuck), typeof(BombHen), typeof(Raise))
            ];

        public static void Enable()
        {
            if (!Core.CoreConfig.AllowBaseContent)
                return;

            for (int i = 0; i < BaseContent.Length; i++)
                BaseContent[i].AddRecipe();
        }

        public static void CheckCrafts(this Player p)
        {
            if (!p.TryGetPerkInventory(out PerkInventory inv))
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
}
