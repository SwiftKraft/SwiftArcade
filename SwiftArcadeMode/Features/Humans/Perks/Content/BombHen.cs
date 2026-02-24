namespace SwiftArcadeMode.Features.Humans.Perks.Content
{
    using LabApi.Features.Wrappers;

    [Perk("BombHen", Rarity.Legendary)]
    public class BombHen(PerkInventory inv) : PerkTriggerCooldownBase(inv)
    {
        public override string Name => "Bomb Hen";

        public override string PerkDescription => "Lay an explosive egg (it can also kill you). ";

        public override void Effect() => TimedGrenadeProjectile.SpawnActive(Player.Position, ItemType.GrenadeHE, Player, 5f);

        public override float GetCooldown(Player player) => 35f;
    }
}
