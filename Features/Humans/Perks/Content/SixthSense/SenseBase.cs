namespace SwiftArcadeMode.Features.Humans.Perks.Content.SixthSense
{
    using LabApi.Features.Wrappers;
    using SwiftArcadeMode.Utils.Interfaces;

    public abstract class SenseBase(SixthSense parent) : IWeight
    {
        public SixthSense Parent { get; private set; } = parent;

        public Player Player => Parent.Player;

        public virtual int Weight => 1;

        public abstract bool Message(out string msg);
    }
}
