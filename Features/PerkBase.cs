namespace SwiftArcadeMode.Features
{
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

        public string FancyName => Name.FancifyPerkName(Rarity);

        public virtual int SlotUsage => 1;

        public abstract string Name { get; }

        public abstract string Description { get; }

        public PerkInventory Inventory { get; } = inv;

        public Player Player => Inventory.Parent;

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

        public virtual void SendMessage(string message, float duration = 3) => Player.SendHint($"<size=36>{FancyName}\n</size><size=24>{message}</size>", [HintEffectPresets.FadeOut()], duration);
    }
}
