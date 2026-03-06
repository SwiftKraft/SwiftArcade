namespace SwiftArcadeMode.Features.Humans.Perks.Content
{
    using System.Collections.Generic;
    using CustomPlayerEffects;
    using Hints;
    using LabApi.Events.Handlers;
    using LabApi.Features.Wrappers;

    [Perk("PizzaMan", Rarity.Uncommon)]
    public class PizzaMan(PerkInventory inv) : PerkItemReceiveBase(inv)
    {
        public List<ushort> Pizzas { get; } = [];

        public override ItemType ItemType => ItemType.Medkit;

        public override string Name => "Pizza Man";

        public override string PerkDescription => "Gives you a pizza.";

        public override int Limit => 2;

        public override float GetCooldown(Player player) => 30f;

        public override Item? GiveItem()
        {
            Item? item = base.GiveItem();
            if (item == null)
                return null;

            Pizzas.Add(item.Serial);
            return item;
        }

        public override bool AdditionalCondition(Item i) => Pizzas.Contains(i.Serial);

        public override void Init()
        {
            base.Init();
            PlayerEvents.UsedItem += OnUsedItem;
            PlayerEvents.ChangedItem += OnChangedItem;
        }

        public override void Remove()
        {
            base.Remove();
            PlayerEvents.UsedItem -= OnUsedItem;
            PlayerEvents.ChangedItem -= OnChangedItem;
        }

        private void OnChangedItem(LabApi.Events.Arguments.PlayerEvents.PlayerChangedItemEventArgs ev)
        {
            if (ev.NewItem == null || !Pizzas.Contains(ev.NewItem.Serial))
                return;

            if (ev.Player != Player)
                ev.Player.SendHint($"Equipped {Player.DisplayName}'s pizza.", [HintEffectPresets.FadeOut()]);
            else
                SendMessage("Equipped your pizza.");
        }

        private void OnUsedItem(LabApi.Events.Arguments.PlayerEvents.PlayerUsedItemEventArgs ev)
        {
            if (!Pizzas.Contains(ev.UsableItem.Serial))
                return;

            if (ev.Player != Player)
            {
                SendMessage($"{ev.Player.DisplayName} has eaten your pizza.");
                ev.Player.SendHint($"You've eaten {Player.DisplayName}'s pizza.", [HintEffectPresets.FadeOut()]);
            }
            else
                SendMessage("You've eaten your own pizza.");

            ev.Player.EnableEffect<Invigorated>(1, 5f, true);
            ev.Player.ArtificialHealth += 15f;
        }
    }
}
