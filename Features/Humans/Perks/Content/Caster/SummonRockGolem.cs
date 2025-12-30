using Footprinting;
using LabApi.Features.Wrappers;
using PlayerRoles;
using PlayerStatsSystem;
using SwiftArcadeMode.Utils.Deployable;
using SwiftArcadeMode.Utils.Extensions;
using SwiftArcadeMode.Utils.Sounds;
using SwiftArcadeMode.Utils.Visuals;
using UnityEngine;

namespace SwiftArcadeMode.Features.Humans.Perks.Content.Caster
{
    public class SummonRockGolem : SummonSpell
    {
        public override string Name => "Summon Rock Golem";

        public override Color BaseColor => new(0.3f, 0.3f, 0.3f);

        public override int RankIndex => 1;

        public override float CastTime => 1f;

        public override DeployableBase Create(Vector3 loc) => new Golem(this, Caster.Player.DisplayName + "'s Rock Golem", "RockGolem".ApplySchematicPrefix(), Caster.Player.Role, new Vector3(1f, 0.5f, 1f), loc, Quaternion.identity);

        public class Golem(SpellBase spell, string name, string schematicName, RoleTypeId role, Vector3 colliderScale, Vector3 position, Quaternion rotation) : TurretSummon(spell, name, schematicName, role, colliderScale, position, rotation)
        {
            public override string TypeName => "Rock Golem";

            public override float Range => 15;

            public override float Delay => 1.5f;

            public override float Health => 700f;

            Vector3 damp;
            Vector3 vel;
            Player prevTarget;

            public override void Attack(Player target)
            {
                prevTarget = target;

                if (!VectorExtensions.SolveBallisticArc(Dummy.Camera.position, Dummy.Camera.position.PredictPosition(target.Position, damp, 12f), 12f, false, out Vector3 vel))
                    return;

                Vector3 direction = vel.normalized;
                new Projectile(Spell, Dummy.Camera.position, Quaternion.LookRotation(direction), direction * 12f, 5f, Dummy);
            }

            public override void Tick()
            {
                base.Tick();

                if (prevTarget == null)
                    return;

                damp = Vector3.SmoothDamp(damp, prevTarget.Velocity, ref vel, 0.2f);
            }

            public class Projectile(SpellBase spell, Vector3 initialPosition, Quaternion initialRotation, Vector3 initialVelocity, float lifetime = 10, Player owner = null) : CasterBase.MagicProjectileBase(spell, initialPosition, initialRotation, initialVelocity, lifetime, owner)
            {
                public override bool UseGravity => true;
                public override float CollisionRadius => 0.2f;

                public override string SchematicName => "RockProjectile";

                public override void Hit(Collision col, ReferenceHub hit)
                {
                    if (hit != null)
                    {
                        float damage = 90f;

                        hit.playerStats.DealDamage(new ExplosionDamageHandler(new Footprint(Owner.ReferenceHub), InitialVelocity, damage * (hit.IsSCP() ? 2.5f : 1f), 30, ExplosionType.Grenade));
                        Owner?.SendHitMarker(2f);
                    }

                    SchematicEffect.Create("RockHit".ApplySchematicPrefix(), Rigidbody.position, Rigidbody.rotation, Vector3.one, 0.4f);
                    Spell?.PlaySound(Rigidbody.position, "hit");
                    Destroy();
                }
            }
        }
    }
}
