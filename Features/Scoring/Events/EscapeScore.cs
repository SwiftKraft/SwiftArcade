namespace SwiftArcadeMode.Features.Scoring.Events
{
    using LabApi.Events.Arguments.PlayerEvents;
    using LabApi.Events.Handlers;

    public class EscapeScore : ScoreEventBase
    {
        public override void Disable() => PlayerEvents.Escaped -= OnEscaped;

        protected virtual void OnEscaped(PlayerEscapedEventArgs ev) => ev.Player.AddScore(12);

        public override void Enable() => PlayerEvents.Escaped += OnEscaped;

        public override void Tick()
        {
        }
    }
}
