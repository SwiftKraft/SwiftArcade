namespace SwiftArcadeMode.Features.Humans.Perks.Content.Gambler
{
    using CustomPlayerEffects;

    public class GamblerEyes : GamblerStatusEffectBase<SeveredEyes>
    {
        public override bool Positive => false;

        public override int Weight => 1;

        public override string Explanation => "Thanks for those eyes.";
    }
}
