namespace SwiftArcadeMode.Features
{
    using System;
    using Hints;
    using LabApi.Features.Wrappers;

    public enum Rarity : int
    {
        Common = 52,
        Uncommon = 38,
        Rare = 26,
        Epic = 12,
        Legendary = 7,
        Mythic = 3,
        Secret = 1,
    }

    public abstract class PerkBase(PerkInventory inv) : IPerkInfo
    {
        public Rarity Rarity { get; set; }

        public PerkRestriction Restriction { get; set; }

        [Obsolete("use GetFancyName instead.")]
        public string FancyName => Name.FancifyPerkName(Rarity);

        public virtual int SlotUsage => 1;

        public abstract string Name { get; }

        public abstract string Description { get; }

        public PerkInventory Inventory { get; } = inv;

        public Player Player => Inventory.Parent;

        public string GetFancyName(Player player) => GetName(player).FancifyPerkName(Rarity);

        /// <summary>
        /// Calls when the player acquires the perk.
        /// </summary>
        public virtual void Init()
        {
        }

        /// <summary>
        /// Runs every FixedUpdate.
        /// </summary>
        public virtual void Tick()
        {
        }

        /// <summary>
        /// Calls when the perk gets removed from the player.
        /// </summary>
        public virtual void Remove()
        {
        }

        /// <summary>
        /// Gets the name of the perk.
        /// </summary>
        /// <param name="player">The player requesting the name.</param>
        /// <returns>The name of the perk.</returns>
        /// <remarks>This is used to create the text when picking up perks so you can customize this to provide specific names to specific players.</remarks>
        public virtual string GetName(Player player) => Name;

        /// <summary>
        /// Gets the description of the perk.
        /// </summary>
        /// <param name="player">The player requesting the description.</param>
        /// <returns>The description of the perk.</returns>
        /// <remarks>This is used to create the text when picking up perks so you can customize this to provide specific descriptions to specific players.</remarks>
        public virtual string GetDescription(Player player) => Description;

        public virtual void SendMessage(string message, float duration = 3) => Player.SendHint($"<size=36>{GetFancyName(Player)}\n</size><size=24>{message}</size>", [HintEffectPresets.FadeOut()], duration);
    }
}
