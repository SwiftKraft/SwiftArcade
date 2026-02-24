namespace SwiftArcadeMode.Features.Humans.Perks.Content.Gambler
{
    using CustomPlayerEffects;

    public class GamblerHands : GamblerStatusEffectBase<SeveredHands>
    {
        public override bool Positive => false;

        public override int Weight => 1;

        public override string Explanation => "Hey buddy, need a hand?";
    }
}
