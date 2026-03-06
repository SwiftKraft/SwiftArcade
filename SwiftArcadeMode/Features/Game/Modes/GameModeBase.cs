namespace SwiftArcadeMode.Features.Game.Modes
{
    using SwiftArcadeMode.Features.Humans.Perks;

    public abstract class GameModeBase
    {
        public abstract PerkSpawnRulesBase? OverrideSpawnRules { get; }

        public abstract void Start();

        public abstract void Tick();

        public abstract void End();
    }
}
