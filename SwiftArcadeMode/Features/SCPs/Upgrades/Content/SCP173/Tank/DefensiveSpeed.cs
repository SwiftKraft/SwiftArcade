namespace SwiftArcadeMode.Features.SCPs.Upgrades.Content.SCP173.Tank
{
    using LabApi.Events.Handlers;
    using PlayerStatsSystem;

    public class DefensiveSpeed(UpgradePathPerkBase parent) : UpgradeBase<Tank>(parent)
    {
        private bool enabled;

        public override string Name => "Defensive Speed";

        public override string Description => "Take reduced damage when in breakneck speeds.";

        public virtual float Percentage => 0.25f;

        public override void Init()
        {
            base.Init();
            Scp173Events.BreakneckSpeedChanged += OnBreakneckSpeedsChanged;
            PlayerEvents.Hurting += OnPlayerHurting;
        }

        public override void Remove()
        {
            base.Remove();
            Scp173Events.BreakneckSpeedChanged -= OnBreakneckSpeedsChanged;
            PlayerEvents.Hurting -= OnPlayerHurting;
        }

        private void OnPlayerHurting(LabApi.Events.Arguments.PlayerEvents.PlayerHurtingEventArgs ev)
        {
            if (ev.Player != Player || !enabled)
                return;

            if (ev.DamageHandler is StandardDamageHandler dmg)
                dmg.Damage *= 1f - Percentage;
        }

        private void OnBreakneckSpeedsChanged(LabApi.Events.Arguments.Scp173Events.Scp173BreakneckSpeedChangedEventArgs ev)
        {
            if (ev.Player != Player)
                return;

            enabled = ev.Active;
        }
    }
}
