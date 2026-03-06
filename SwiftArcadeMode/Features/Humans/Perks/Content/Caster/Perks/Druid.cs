namespace SwiftArcadeMode.Features.Humans.Perks.Content.Caster.Perks
{
    using System;
    using SwiftArcadeMode.Features.Humans.Perks.Content.Caster.Spells;

    [Perk("Druid", Rarity.Epic)]
    public class Druid(PerkInventory inv) : CasterBase(inv)
    {
        public override float RegularCooldown => 8f;

        public override string Name => "Druid";

        public override Type[] ListSpells() => [
            typeof(ThornShot),
            typeof(OrbOfNature),
            typeof(ThornVolley),
            typeof(SummonPylon),
            typeof(SummonThornShooter)
            ];
    }
}
