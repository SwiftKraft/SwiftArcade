namespace SwiftArcadeMode.Features.Humans.Perks.Content
{
    using Interactables.Interobjects.DoorUtils;
    using LabApi.Features.Wrappers;

    [Perk("DoorTechnician", Rarity.Epic)]
    public class DoorTechnician(PerkInventory inv) : PerkDoorBase(inv)
    {
        public override string Name => "Door Technician";

        public override string Description => "Repair every door near you.";

        public virtual float Range => 5f;

        public override void Tick()
        {
            base.Tick();

            Door d = GetClosestDoor();
            if ((d.Position - Player.Position).sqrMagnitude <= Range * Range && d.Base is IDamageableDoor { IsDestroyed: true } door)
            {
                door.ServerRepair();
                d.IsOpened = true;
            }
        }
    }
}
