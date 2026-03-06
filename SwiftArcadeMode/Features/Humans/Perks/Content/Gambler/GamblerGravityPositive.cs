namespace SwiftArcadeMode.Features.Humans.Perks.Content.Gambler
{
    using LabApi.Features.Wrappers;

    public class GamblerGravityPositive : GamblerEffectBase
    {
        public override bool Positive => true;

        public override int Weight => 1;

        public override string Explanation => "Halved your gravity";

        public override void Effect(Player player) => player.Gravity /= 2f;
    }
}
