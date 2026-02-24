namespace SwiftArcadeMode.Features.Humans.Perks.Content
{
    using CustomPlayerEffects;
    using LabApi.Features.Wrappers;
    using SwiftArcadeMode.Utils.Structures;
    using UnityEngine;

    [Perk("Ninjutsu", Rarity.Rare)]
    public class Ninjutsu(PerkInventory inv) : PerkCooldownBase(inv)
    {
        private readonly Timer activationTimer = new();

        public override string Name => "Ninjutsu";

        public override string PerkDescription => $"Turns you invisible at <{HealthThreshold} HP for {Duration}s.";

        public virtual float HealthThreshold => 20f;

        public virtual float Duration => 5f;

        public override float GetCooldown(Player player) => 120f;

        public override void Init()
        {
            base.Init();
            activationTimer.OnTimerEnd += OnTimerEnd;
        }

        public override void Remove()
        {
            base.Remove();
            activationTimer.OnTimerEnd -= OnTimerEnd;
        }

        public override void Effect()
        {
            Player.EnableEffect<Invisible>();
            Player.CurrentItem = null;
            activationTimer.Reset(Duration);
        }

        public override void Tick()
        {
            base.Tick();

            if (Player.Health <= HealthThreshold)
                Trigger();

            if (!activationTimer.Ended)
                SendMessage("Invisible For: " + Mathf.Round(activationTimer.CurrentValue) + "s");

            activationTimer.Tick(Time.fixedDeltaTime);
        }

        protected virtual void OnTimerEnd() => Player.DisableEffect<Invisible>();
    }
}
