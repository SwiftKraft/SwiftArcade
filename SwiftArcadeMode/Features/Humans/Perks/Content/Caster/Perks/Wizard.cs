namespace SwiftArcadeMode.Features.Humans.Perks.Content.Caster.Perks
{
    using System;
    using SwiftArcadeMode.Features.Humans.Perks.Content.Caster.Spells;

    [Perk("Wizard", Rarity.Epic)]
    public class Wizard(PerkInventory inv) : CasterBase(inv)
    {
        public override string Name => "Wizard";

        public override float RegularCooldown => 10f;

        public override Type[] ListSpells() => [
            typeof(Fireball),
            typeof(IceBolt),
            typeof(MagicMissile),
            typeof(SummonArcaneGolem)
            ];
    }
}
