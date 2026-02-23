namespace SwiftArcadeMode.Features.Humans.Perks.Content.Gambler
{
    using LabApi.Features.Wrappers;

    public class GamblerExplode : GamblerEffectBase
    {
        public override bool Positive => false;

        public override int Weight => 1;

        public override string Explanation => "Boom!";

        public override void Effect(Player player) => TimedGrenadeProjectile.SpawnActive(player.Position, ItemType.GrenadeHE, player, 0.1f);
    }
}
