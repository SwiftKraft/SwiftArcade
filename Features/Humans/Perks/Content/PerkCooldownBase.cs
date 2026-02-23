namespace SwiftArcadeMode.Features.Humans.Perks.Content
{
    using SwiftArcadeMode.Utils.Structures;
    using UnityEngine;

    public abstract class PerkCooldownBase(PerkInventory inv) : PerkBase(inv)
    {
        public override string Description => $"{PerkDescription}\nCooldown: {Cooldown}s.";

        public abstract string PerkDescription { get; }

        public virtual string ReadyMessage => "Ready!";

        public virtual float Cooldown => 10f;

        protected Timer CooldownTimer = new();

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

        protected virtual void OnCooldownEnd()
        {
            if (!string.IsNullOrWhiteSpace(ReadyMessage))
                SendMessage(ReadyMessage);
        }

        public abstract void Effect();

        public override void Tick()
        {
            base.Tick();
            CooldownTimer.Tick(Time.fixedDeltaTime);
        }
    }
}
