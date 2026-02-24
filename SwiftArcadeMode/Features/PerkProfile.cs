namespace SwiftArcadeMode.Features
{
    using System;

    [Obsolete("This struct is obsolete, use methods found inside your target perk instead.")]
    public readonly struct PerkProfile(Rarity r, string name, string desc)
    {
        public Rarity Rarity { get; } = r;

        public string Name { get; } = name;

        public string Description { get; } = desc;

        public string FancyName => Name.FancifyPerkName(Rarity);
    }
}