namespace SwiftArcadeMode.Features.SCPs.Upgrades.Content
{
    public abstract class UpgradeCooldownTriggerBase<T>(UpgradePathPerkBase parent) : UpgradeCooldownBase<T>(parent) where T : UpgradePathPerkBase
    {
        public override void Tick()
        {
            base.Tick();
            Trigger();
        }
    }
}
