namespace SwiftArcadeMode.Features.Humans.Perks.Content
{
    [Perk("SlotUpgrade", Rarity.Legendary)]
    public class PerkSlotUpgrade(PerkInventory inv) : PerkBase(inv)
    {
        public override string Name => "Perk Slot Upgrade";

        public override string Description => $"Gives you +{Amount} perk slots, takes up {SlotUsage} slots.";

        public virtual int Amount => 3;

        public override int SlotUsage => 0;

        private PerkInventory.LimitAdditive additive;

        public override void Init()
        {
            base.Init();
            additive = Inventory.CreateLimitAdditive(Amount);
        }

        public override void Remove()
        {
            base.Remove();
            Inventory.RemoveLimitAdditive(additive);
        }
    }
}
