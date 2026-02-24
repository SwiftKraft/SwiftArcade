namespace SwiftArcadeMode.Features.Humans.Perks.Content.Caster.Spells
{
    using CustomPlayerEffects;
    using Footprinting;
    using LabApi.Features.Wrappers;
    using PlayerRoles;
    using PlayerRoles.FirstPersonControl;
    using PlayerStatsSystem;
    using UnityEngine;

    public class LightArrow : SpellBase
    {
        public LightArrow(CasterBase caster)
            : base(caster)
        {
        }

        public override string Name => "Light Arrow";

        public override Color BaseColor => Color.white;

        public override int RankIndex => 3;

        public override float CastTime => 0.5f;

        public override void Cast()
        {
            new Projectile(this, Caster.Player, Caster.Player.Camera.position + (Caster.Player.Camera.forward * 0.4f), Caster.Player.Camera.rotation, Caster.Player.Camera.forward * 30f, 6f).Init();
            PlaySound("cast");
        }

        public class Projectile(SpellBase spell, Player owner, Vector3 initialPosition, Quaternion initialRotation, Vector3 initialVelocity, float lifetime = 10f)
            : CasterBase.MagicProjectileBase(spell, owner, initialPosition, initialRotation, initialVelocity, lifetime)
        {
            private const int MaxBounces = 2;

            private int currentBounces;
            private float speed;
            private float currentDamage = 75f;
            private float currentScpMultiplier = 3f;
            private Vector3 vel;

            public override string SchematicName => "LightArrow";

            public override bool UseGravity => false;

            public override float CollisionRadius => 0.08f;

            public override void Init()
            {
                speed = InitialVelocity.magnitude;
                vel = InitialVelocity;
                base.Init();
            }

            public override void Tick()
            {
                base.Tick();
                Rigidbody?.linearVelocity = vel;
            }

            public override void Hit(Collision col, ReferenceHub? hit)
            {
                if (!Rigidbody)
                    return;

                if (!hit && currentBounces < MaxBounces)
                {
                    Vector3 normal = col.GetContact(0).normal.normalized;
                    Vector3 direction = Vector3.Reflect(vel.normalized, -normal);

                    if (FindTargetInDirection(direction, 25f, 30f, out Vector3 dir))
                        direction = dir;

                    Rigidbody.angularVelocity = Vector3.zero;
                    Rigidbody.transform.forward = direction.normalized;

                    speed *= 1.5f;
                    vel = direction.normalized * speed;

                    currentDamage += 25f;
                    currentScpMultiplier += 0.5f;
                    currentBounces++;
                }
                else
                {
                    if (hit)
                    {
                        hit.playerStats.DealDamage(new ExplosionDamageHandler(new Footprint(Owner.ReferenceHub), InitialVelocity, currentDamage * (hit.IsSCP() ? currentScpMultiplier : 1f), 100, ExplosionType.Disruptor));
                        hit.playerEffectsController.EnableEffect<Flashed>(3f, true);

                        if (hit.roleManager.CurrentRole is IFpcRole role)
                        {
                            role.FpcModule.Motor.JumpController.ForceJump(1f);
                        }

                        Owner.SendHitMarker(2f);
                    }

                    TimedGrenadeProjectile.PlayEffect(Rigidbody.position, ItemType.GrenadeFlash);
                    Destroy();
                }
            }

            private bool FindTargetInDirection(Vector3 dir, float maxDistance, float maxAngle, out Vector3 dirToTarget)
            {
                dirToTarget = default;

                if (!Rigidbody)
                    return false;

                Player? best = null;
                float bestDot = 0f;

                foreach (Player player in Player.List)
                {
                    if (player == Owner || !player.IsAlive || (Owner != null && player.Faction == Owner.Faction)) 
                        continue;

                    Vector3 toTarget = player.Position - Rigidbody.position;
                    float dist = toTarget.magnitude;

                    if (dist > maxDistance)
                        continue;

                    Vector3 dirToTarget1 = toTarget.normalized;
                    float dot = Vector3.Dot(dir.normalized, dirToTarget1);

                    if (dot > Mathf.Cos(maxAngle * Mathf.Deg2Rad) && dot > bestDot)
                    {
                        bestDot = dot;
                        best = player;
                        dirToTarget = dirToTarget1;
                    }
                }

                return best != null;
            }
        }
    }
}
