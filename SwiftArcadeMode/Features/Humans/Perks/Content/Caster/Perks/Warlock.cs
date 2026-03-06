namespace SwiftArcadeMode.Features.Humans.Perks.Content.Caster.Perks
{
    using System;
    using SwiftArcadeMode.Features.Humans.Perks.Content.Caster.Spells;

    [Perk("Warlock", Rarity.Epic)]
    public class Warlock(PerkInventory inv) : CasterBase(inv)
    {
        public override float RegularCooldown => 8f;

        public override float LessItemsCooldown => 5f;

        public override string Name => "Warlock";

        public override Type[] ListSpells() => [
            typeof(CurseOfPain),
            typeof(BoltOfDarkness),
            typeof(RayOfDarkness),
            typeof(SummonAltar)
            ];
    }
}
