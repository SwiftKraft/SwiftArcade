namespace SwiftArcadeMode.Features.Events
{
    using System;
    using LabApi.Events.Arguments.Interfaces;
    using LabApi.Features.Wrappers;

    public class PickedUpPerkEventArgs(Player p, PerkAttribute att) : EventArgs, IPlayerEvent
    {
        public Player Player { get; } = p;

        public PerkAttribute Perk { get; } = att;
    }
}
