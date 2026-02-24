namespace SwiftArcadeMode.Features.SCPs.Upgrades.Content.SCP173.Tank
{
    using LabApi.Events.Handlers;
    using PlayerStatsSystem;

    public class BlastResistant(UpgradePathPerkBase parent) : UpgradeBase<Tank>(parent)
    {
        public override string Name => "Blast Resistant";

        public override string Description => $"Take {Percentage * 100f}% less damage from explosions.";

        public float Percentage => 0.5f;

        public override void Init()
        {
            base.Init();
            PlayerEvents.Hurting += OnHurting;
        }

        public override void Remove()
        {
            base.Remove();
            PlayerEvents.Hurting -= OnHurting;
        }

        private void OnHurting(LabApi.Events.Arguments.PlayerEvents.PlayerHurtingEventArgs ev)
        {
            if (ev.Player != Player || ev.DamageHandler is not ExplosionDamageHandler exp)
                return;

            exp.Damage *= 1f - Percentage;
        }
    }
}
