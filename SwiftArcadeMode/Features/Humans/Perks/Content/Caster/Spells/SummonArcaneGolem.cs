namespace SwiftArcadeMode.Features.Humans.Perks.Content.Caster.Spells
{
    using LabApi.Features.Wrappers;
    using SwiftArcadeMode.Utils.Deployable;
    using UnityEngine;

    public class SummonArcaneGolem : SummonSpell
    {
        public SummonArcaneGolem(CasterBase caster)
            : base(caster)
        {
        }

        public override string Name => "Summon Arcane Golem";

        public override Color BaseColor => new(0.3f, 0f, 0.4f);

        public override int RankIndex => 2;

        public override float CastTime => 1f;

        public override int Limit => 1;

        public override DeployableBase Create(Vector3 loc)
        {
            Golem golem = new(this, Caster.Player.DisplayName + "'s Golem", "ArcaneGolem", loc, Quaternion.identity);
            golem.Initialize();
            return golem;
        }

        public class Golem(SpellBase spell, string name, string schematicName, Vector3 position, Quaternion rotation) : TurretSummon(spell, name, schematicName, position, rotation)
        {
            public override string TypeName => "Arcane Golem";

            public override float MaxHealth => 70f;

            public override float Range => 10f;

            public override float Delay => 1f;

            public override float DestroyRange => 20f;

            public override void Attack(Player target)
            {
                Vector3 direction = (target.Position - Head.Transform.position).normalized;
                new MagicMissile.Projectile(Spell, Owner, Head.Transform.position, Quaternion.LookRotation(direction), direction * 9f, 4f).Init();
            }
        }
    }
}
