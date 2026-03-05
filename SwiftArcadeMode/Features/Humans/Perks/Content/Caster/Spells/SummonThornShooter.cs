namespace SwiftArcadeMode.Features.Humans.Perks.Content.Caster.Spells
{
    using LabApi.Features.Wrappers;
    using SwiftArcadeMode.Utils.Deployable;
    using UnityEngine;

    public class SummonThornShooter : SummonSpell
    {
        public SummonThornShooter(CasterBase caster)
            : base(caster)
        {
        }

        public override string Name => "Summon Thorn Shooter";

        public override Color BaseColor => new(0f, 0.6f, 0.1f);

        public override int RankIndex => 1;

        public override float CastTime => 1f;

        public override int Limit => 2;

        public override DeployableBase Create(Vector3 loc)
        {
            Shooter shooter = new(this, Caster.Player.DisplayName + "'s Thorn Shooter", "ThornShooter", loc, Quaternion.identity);
            shooter.Initialize();
            return shooter;
        }

        public class Shooter(SpellBase spell, string name, string schematicName, Vector3 position, Quaternion rotation)
            : TurretSummon(spell, name, schematicName, position, rotation)
        {
            public override string TypeName => "Thorn Shooter";

            public override float MaxHealth => 50f;

            public override float Range => 5f;

            public override float Delay => 0.2f;

            public override float DestroyRange => 20f;

            public override void Attack(Player target)
            {
                Vector3 direction = Quaternion.Euler(Random.insideUnitSphere * 5f) * (target.Camera.position - Head.Transform.position).normalized;
                new ThornShot.Projectile(Spell, Owner, Head.Transform.position, Quaternion.LookRotation(direction), direction * 20f, 4f).Init();
            }
        }
    }
}
