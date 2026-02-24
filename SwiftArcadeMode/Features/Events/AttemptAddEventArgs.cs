namespace SwiftArcadeMode.Features.Events
{
    using System;
    using LabApi.Events.Arguments.Interfaces;
    using LabApi.Events.Arguments.PlayerEvents;
    using LabApi.Features.Wrappers;

    public class AttemptAddEventArgs(PlayerPickedUpItemEventArgs ev, PerkAttribute att) : EventArgs, IPlayerEvent, IItemEvent, ICancellableEvent
    {
        public bool IsAllowed { get; set; } = true;

        public PerkAttribute Perk { get; } = att;

        public Player Player { get; } = ev.Player;

        public Item Item { get; } = ev.Item;
    }
}
