namespace SwiftArcadeMode.Features.Humans.Perks.Content.Gambler
{
    using LabApi.Features.Wrappers;

    public class GamblerHeal : GamblerEffectBase
    {
        public override bool Positive => true;

        public override int Weight => 3;

        public override string Explanation => "Healed you.";

        public override void Effect(Player player) => player.Heal(15f);
    }
}
