namespace SwiftArcadeMode.Features.SCPs.Upgrades
{
    public abstract class UpgradeBase<T>(UpgradePathPerkBase parent) : UpgradeBase(parent)
        where T : UpgradePathPerkBase
    {
        public new T Parent => (T)base.Parent;
    }
}
