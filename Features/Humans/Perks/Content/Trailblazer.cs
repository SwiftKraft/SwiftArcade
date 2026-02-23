namespace SwiftArcadeMode.Features.Humans.Perks.Content
{
    using LabApi.Events.Arguments.PlayerEvents;
    using LabApi.Events.Handlers;
    using LabApi.Features.Wrappers;
    using SwiftArcadeMode.Utils.Extensions;
    using UnityEngine;

    [Perk("Trailblazer", Rarity.Rare)]
    public class Trailblazer(PerkInventory inv) : PerkCooldownBase(inv)
    {
        public override string Name => "Trailblazer" + (initialized ? " | Tracked Item: " + TrackedType + (!CooldownTimer.Ended ? " | Cooldown: " + Mathf.Round(CooldownTimer.CurrentValue) + "s" : string.Empty) : string.Empty);

        public override string PerkDescription => $"Set a teleport point after using an item. \nTeleport to the point after using an item of the same type. \nNo item types will be tracked when a teleport point exists.";

        public override float Cooldown => 80f;

        public Vector3 TeleportPoint;
        public bool TeleportExists;
        public ItemType TrackedType = ItemType.None;
        public Elevator TrackedElevator;

        private bool initialized;

        public override void Effect()
        {
            if (!TeleportExists)
                return;

            Player.Position = TrackedElevator != null ? TrackedElevator.Base.transform.position + TeleportPoint : TeleportPoint;
            TrackedElevator = null;
            TrackedType = ItemType.None;
            TeleportExists = false;
            SendMessage("Teleported! Teleport point destroyed.");
        }

        public override void Init()
        {
            base.Init();

            WarheadEvents.Detonated += OnWarheadDetonated;
            PlayerEvents.UsingItem += OnUsingItem;
            PlayerEvents.UsedItem += OnUsedItem;

            if (Player != null)
                initialized = true;
        }

        private void OnWarheadDetonated(LabApi.Events.Arguments.WarheadEvents.WarheadDetonatedEventArgs ev)
        {
            if (TeleportExists && (TrackedElevator != null || !Room.TryGetRoomAtPosition(TeleportPoint, out Room room) || room.Zone != MapGeneration.FacilityZone.Surface))
            {
                TrackedElevator = null;
                TrackedType = ItemType.None;
                TeleportExists = false;
                SendMessage("Warhead detonated! Teleport point destroyed.");
            }
        }

        public override void Remove()
        {
            base.Remove();

            WarheadEvents.Detonated -= OnWarheadDetonated;
            PlayerEvents.UsingItem -= OnUsingItem;
            PlayerEvents.UsedItem -= OnUsedItem;
        }

        protected virtual void OnUsingItem(PlayerUsingItemEventArgs ev)
        {
            if (ev.Player != Player)
                return;

            if (TeleportExists && ev.UsableItem.Type == TrackedType)
                SendMessage($"{(CooldownTimer.Ended ? "<color=#00FF00>You will be teleported!</color>" : "<color=#FF0000>On cooldown, you will <b>NOT</b> be teleported!</color>")} Cancel to abort.");
            else if (!TeleportExists && Player.Room.Name == MapGeneration.RoomName.Pocket)
                SendMessage($"Cannot create teleport point in this room!");
        }

        protected virtual void OnUsedItem(PlayerUsedItemEventArgs ev) // Teleport point follows elevator
        {
            if (ev.Player != Player)
                return;

            if (TeleportExists)
            {
                if (ev.UsableItem.Type == TrackedType)
                    Trigger();
                return;
            }

            if (Player.Room.Name == MapGeneration.RoomName.Pocket)
            {
                SendMessage($"Cannot create teleport point in this room!");
                return;
            }

            TrackedElevator = Player.GetElevator();
            TeleportPoint = TrackedElevator != null ? Player.Position - TrackedElevator.Base.transform.position : Player.Position;
            TrackedType = ev.UsableItem.Type;
            TeleportExists = true;
            SendMessage($"Teleport point has been created{(!CooldownTimer.Ended ? " (teleport on cooldown)" : string.Empty)}!");
        }
    }
}
