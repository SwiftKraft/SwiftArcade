namespace SwiftArcadeMode.Features.Humans.Perks.Content.Caster.Perks
{
    using System;
    using SwiftArcadeMode.Features.Humans.Perks.Content.Caster.Spells;

    [Perk("Sorcerer", Rarity.Epic)]
    public class Sorcerer(PerkInventory inv) : CasterBase(inv)
    {
        public override float RegularCooldown => 10f;

        public override string Name => "Sorcerer";

        public override Type[] ListSpells() => [
            typeof(ElementalBolt),
            typeof(LightArrow),
            typeof(FireArrow),
            typeof(SummonRockGolem)
            ];
    }
}
