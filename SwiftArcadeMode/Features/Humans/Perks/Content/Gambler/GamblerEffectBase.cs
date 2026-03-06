namespace SwiftArcadeMode.Features.Humans.Perks.Content.Gambler
{
    using LabApi.Features.Wrappers;
    using SwiftArcadeMode.Utils.Interfaces;

    public abstract class GamblerEffectBase : IWeight
    {
        public abstract bool Positive { get; }

        public abstract int Weight { get; }

        public abstract string Explanation { get; }

        public virtual float ExplanationDuration => 3f;

        public abstract void Effect(Player player);
    }
}
