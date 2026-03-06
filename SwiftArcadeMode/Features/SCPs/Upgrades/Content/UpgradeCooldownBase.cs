namespace SwiftArcadeMode.Features.SCPs.Upgrades.Content
{
    using SwiftArcadeMode.Utils.Structures;
    using UnityEngine;

    public abstract class UpgradeCooldownBase<T>(UpgradePathPerkBase parent) : UpgradeBase<T>(parent)
        where T : UpgradePathPerkBase
    {
        public override string Description => $"{UpgradeDescription}\nCooldown: {Cooldown}s.";

        public abstract string UpgradeDescription { get; }

        public virtual string ReadyMessage => "Ready!";

        public virtual float Cooldown => 10f;

        protected Timer CooldownTimer { get; } = new();

        public override void Init()
        {
            base.Init();
            CooldownTimer.OnTimerEnd += OnCooldownEnd;
        }

        public override void Remove()
        {
            base.Remove();
            CooldownTimer.OnTimerEnd -= OnCooldownEnd;
        }

        public virtual void Trigger()
        {
            if (!CooldownTimer.Ended)
                return;

            Effect();
            CooldownTimer.Reset(Cooldown);
        }

        public abstract void Effect();

        public override void Tick()
        {
            base.Tick();
            CooldownTimer.Tick(Time.fixedDeltaTime);
        }

        protected virtual void OnCooldownEnd()
        {
            if (!string.IsNullOrWhiteSpace(ReadyMessage))
                SendMessage(ReadyMessage);
        }
    }
}
