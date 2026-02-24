namespace SwiftArcadeMode.Features.Humans.Perks.Content.Gambler
{
    using CustomPlayerEffects;
    using LabApi.Features.Wrappers;

    public abstract class GamblerStatusEffectBase<T> : GamblerEffectBase
        where T : StatusEffectBase
    {
        public virtual byte Intensity => 1;

        public virtual float Duration => 0f;

        public virtual bool AddDuration => false;

        public override void Effect(Player player) => player.EnableEffect<T>(Intensity, Duration, AddDuration);
    }
}
