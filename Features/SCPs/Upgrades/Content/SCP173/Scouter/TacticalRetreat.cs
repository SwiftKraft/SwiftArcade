namespace SwiftArcadeMode.Features.SCPs.Upgrades.Content.SCP173.Scouter
{
    using LabApi.Events.Arguments.PlayerEvents;
    using LabApi.Events.Arguments.Scp173Events;
    using LabApi.Events.Arguments.WarheadEvents;
    using LabApi.Events.Handlers;
    using LabApi.Features.Wrappers;
    using MEC;

    public class TacticalRetreat(UpgradePathPerkBase parent) : UpgradeCooldownBase<Scouter>(parent)
    {
        public override string Name => "Tactical Retreat";

        public override string UpgradeDescription => "Teleports you back to your latest tantrum location when taking heavy damage after hume shield breaks. ";

        public override float Cooldown => 100f;

        public virtual float RealHealthDamageThreshold => 400f;

        private TantrumHazard latestTantrum;
        private float recordedHealth;

        public override void Init()
        {
            base.Init();
            Scp173Events.CreatedTantrum += OnCreatedTantrum;
            WarheadEvents.Detonated += OnWarheadDetonated;
            PlayerEvents.Hurt += OnHurt;
        }

        public override void Remove()
        {
            base.Remove();
            Scp173Events.CreatedTantrum -= OnCreatedTantrum;
            WarheadEvents.Detonated -= OnWarheadDetonated;
            PlayerEvents.Hurt -= OnHurt;
        }

        private void OnWarheadDetonated(WarheadDetonatedEventArgs ev)
        {
            latestTantrum = null;
            SendMessage("Warhead Detonated! Tantrum teleport destroyed.");
        }

        private void OnHurt(PlayerHurtEventArgs ev)
        {
            if (ev.Player != Player)
                return;

            if (Player.HumeShield <= 0f && recordedHealth < 0f)
                recordedHealth = Player.Health;
            else if (recordedHealth - Player.Health >= RealHealthDamageThreshold)
                Trigger();
            else if (Player.HumeShield > 0f)
                recordedHealth = -1f;
        }

        private void OnCreatedTantrum(Scp173CreatedTantrumEventArgs ev)
        {
            if (ev.Player != Player)
                return;

            latestTantrum = ev.Tantrum;
        }

        public override void Effect()
        {
            if (latestTantrum == null || !latestTantrum.IsActive || latestTantrum.IsDestroyed)
                return;

            Timing.CallDelayed(0.2f, () => { Player.Position = latestTantrum.SyncedPosition; });
        }
    }
}
