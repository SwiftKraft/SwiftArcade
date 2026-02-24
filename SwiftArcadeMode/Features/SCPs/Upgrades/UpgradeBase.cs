namespace SwiftArcadeMode.Features.SCPs.Upgrades
{
    using LabApi.Features.Wrappers;

    public abstract class UpgradeBase(UpgradePathPerkBase parent)
    {
        public UpgradePathPerkBase Parent { get; } = parent;

        public Player Player => Parent.Player;

        public abstract string Name { get; }

        public abstract string Description { get; }

        public virtual void Init()
        {
        }

        public virtual void Tick()
        {
        }

        public virtual void Remove()
        {
        }

        public virtual void SendMessage(string message) => Parent.SendMessage($"<size=28><b>{Name}</b></size>\n{message}");
    }
}
