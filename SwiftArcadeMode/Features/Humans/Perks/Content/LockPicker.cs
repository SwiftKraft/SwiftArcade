namespace SwiftArcadeMode.Features.Humans.Perks.Content
{
    using Interactables.Interobjects.DoorUtils;
    using LabApi.Features.Enums;

    // [Perk("LockPicker", Rarity.Mythic)]
    public class LockPicker(PerkInventory inv) : PerkDoorBase(inv)
    {
        public override string Name => "Lock Picker";

        public override string Description => "Open any door, even locked ones.";

        public override void OnDoorAction(DoorVariant door, DoorAction act, ReferenceHub hub)
        {
            if (!door || hub != Player.ReferenceHub || act == DoorAction.Destroyed || act == DoorAction.Opened || act == DoorAction.Closed || door.DoorName.Equals(nameof(DoorName.Hcz079FirstGate)) || door.DoorName.Equals(nameof(DoorName.Hcz079SecondGate)))
                return;

            door.NetworkTargetState = true;
        }
    }
}
