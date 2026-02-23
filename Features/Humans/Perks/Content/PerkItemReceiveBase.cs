namespace SwiftArcadeMode.Features.Humans.Perks.Content
{
    using System.Linq;
    using InventorySystem.Items;
    using LabApi.Features.Wrappers;

    public abstract class PerkItemReceiveBase(PerkInventory inv) : PerkTriggerCooldownBase(inv)
    {
        public abstract ItemType ItemType { get; }

        public virtual int Limit => 1;

        public override void Effect()
        {
            if (GetCount() <= Limit)
                GiveItem();
        }

        public virtual int GetCount() => Player.Items.Count((i) => i.Type == ItemType && AdditionalCondition(i));

        public virtual Item GiveItem() => Player.AddItem(ItemType, ItemAddReason.PickedUp);

        public virtual bool AdditionalCondition(Item i) => true;
    }
}
