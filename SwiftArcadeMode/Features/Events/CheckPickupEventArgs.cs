namespace SwiftArcadeMode.Features.Events
{
    using System;
    using LabApi.Events.Arguments.Interfaces;
    using LabApi.Events.Arguments.PlayerEvents;
    using LabApi.Features.Wrappers;

    public class CheckPickupEventArgs : EventArgs, IPlayerEvent, IPickupEvent
    {
#pragma warning disable CS0618 // Type or member is obsolete
        public CheckPickupEventArgs(Type perk, PerkAttribute attribute, PerkProfile prof, PlayerSearchingPickupEventArgs ev)
        {
            Perk = perk;
            Attribute = attribute;
            Player = ev.Player;
            Pickup = ev.Pickup;
            Profile = prof;
        }
#pragma warning restore CS0618 // Type or member is obsolete

        public Type Perk { get; }

        public PerkAttribute Attribute { get; }

        public Player Player { get; }

        public Pickup Pickup { get; }

        public string OverrideHint { get; set; } = string.Empty;

        [Obsolete("You can access the name of a perk now from ev.Perk -> TryGetPerk -> .HollowInstance -> GetFancyName / GetName / GetDescription. This is more ideal as it gives the developer more control over what name each player sees for a perk.")]
        public PerkProfile Profile { get; }
    }
}
