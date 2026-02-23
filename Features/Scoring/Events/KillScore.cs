namespace SwiftArcadeMode.Features.Scoring.Events
{
    using LabApi.Events.Arguments.PlayerEvents;
    using LabApi.Events.Handlers;

    public class KillScore : ScoreEventBase
    {
        public override void Enable() => PlayerEvents.Dying += OnDying;

        protected virtual void OnDying(PlayerDyingEventArgs ev)
        {
            if (ev.Attacker == null)
                return;

            ev.Attacker.AddScore(5);
        }

        public override void Tick()
        {
        }

        public override void Disable() => PlayerEvents.Dying -= OnDying;
    }
}
