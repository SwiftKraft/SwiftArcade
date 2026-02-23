namespace SwiftArcadeMode.Features.Humans.Perks.Content.Gambler
{
    using LabApi.Features.Wrappers;

    public class GamblerAmmo : GamblerEffectBase
    {
        public override bool Positive => true;

        public override int Weight => 1;

        public override string Explanation => "Added ammo...";

        public override void Effect(Player player)
        {
            player.AddAmmo(ItemType.Ammo12gauge, 20);
            player.AddAmmo(ItemType.Ammo44cal, 20);
            player.AddAmmo(ItemType.Ammo556x45, 40);
            player.AddAmmo(ItemType.Ammo762x39, 40);
            player.AddAmmo(ItemType.Ammo9x19, 60);
        }
    }
}
