namespace SwiftArcadeMode.Features.Humans.Perks.Content.Gambler
{
    using CustomPlayerEffects;

    public class GamblerPocketDimension : GamblerStatusEffectBase<PocketCorroding>
    {
        public override bool Positive => false;

        public override int Weight => 1;

        public override string Explanation => "Let's gamble again.";
    }
}
