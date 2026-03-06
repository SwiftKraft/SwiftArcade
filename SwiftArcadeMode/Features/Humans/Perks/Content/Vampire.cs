namespace SwiftArcadeMode.Features.Humans.Perks.Content
{
    using LabApi.Events.Handlers;

    [Perk("Vampire", Rarity.Epic, PerkRestriction.Human, conflictPerks: [typeof(Reaper), typeof(Regeneration), typeof(SuperRegeneration)])]
    public class Vampire(PerkInventory inv) : Reaper(inv)
    {
        public override string Name => "Vampire";

        public override string Description => base.Description + " \nBut you can no longer use medical items.";

        public override float Percentage => 0.5f;

        public override void Init()
        {
            base.Init();
            PlayerEvents.UsingItem += OnUsingItem;
        }

        public override void Remove()
        {
            base.Remove();
            PlayerEvents.UsingItem -= OnUsingItem;
        }

        private void OnUsingItem(LabApi.Events.Arguments.PlayerEvents.PlayerUsingItemEventArgs ev)
        {
            if (ev.Player != Player)
                return;

            if (ev.UsableItem.Category == ItemCategory.Medical)
            {
                SendMessage("You cannot use medical items!");
                ev.IsAllowed = false;
            }
        }
    }
}
