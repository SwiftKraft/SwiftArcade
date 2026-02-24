namespace SwiftArcadeMode.Features.Scoring.Events
{
    using LabApi.Events.Arguments.PlayerEvents;
    using LabApi.Events.Handlers;

    public class KillScore : ScoreEventBase
    {
        public override void Enable() => PlayerEvents.Dying += OnDying;

        public override void Tick()
        {
        }

        public override void Disable() => PlayerEvents.Dying -= OnDying;

        protected virtual void OnDying(PlayerDyingEventArgs ev) => ev.Attacker?.AddScore(5);
    }
}
