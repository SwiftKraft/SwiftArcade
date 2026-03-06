namespace SwiftArcadeMode.Features.Humans.Perks.Content
{
    using LabApi.Features.Wrappers;
    using SwiftArcadeMode.Utils.Structures;
    using UnityEngine;

    public abstract class PerkCooldownBase(PerkInventory inv) : PerkBase(inv)
    {
        public override string Description => $"{PerkDescription}\nCooldown: {{0}}s.";

        public abstract string PerkDescription { get; }

        public virtual string ReadyMessage => "Ready!";

        protected Timer CooldownTimer { get; } = new();

        public override string GetDescription(Player player) => string.Format(Description, GetCooldown(player));

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

        public virtual float GetCooldown(Player player) => 10f;

        public virtual void Trigger()
        {
            if (!CooldownTimer.Ended)
                return;

            Effect();
            CooldownTimer.Reset(GetCooldown(Player));
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
