namespace SwiftArcadeMode.Features.Scoring
{
    public abstract class ScoreEventBase
    {
        public abstract void Enable();

        public abstract void Tick();

        public abstract void Disable();
    }
}
