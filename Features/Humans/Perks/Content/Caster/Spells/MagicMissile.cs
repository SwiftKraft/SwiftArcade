namespace SwiftArcadeMode.Features.Humans.Perks.Content.Caster.Spells
{
    using System.Collections.Generic;
    using System.Linq;
    using Footprinting;
    using LabApi.Features.Wrappers;
    using MEC;
    using PlayerRoles;
    using PlayerRoles.FirstPersonControl;
    using PlayerStatsSystem;
    using SwiftArcadeMode.Utils.Projectiles;
    using UnityEngine;

    public class MagicMissile : SpellBase
    {
        private CoroutineHandle coroutine;

        public MagicMissile(CasterBase caster)
            : base(caster)
        {
        }

        public override string Name => "Magic Missile";

        public override Color BaseColor => Color.magenta;

        public override int RankIndex => 2;

        public override float CastTime => 0.5f;

        public override void Cast()
        {
            Shoot();

            coroutine = Timing.CallPeriodically(1.78f, 0.25f, () =>
            {
                if (!Caster.Player.IsAlive)
                {
                    Timing.KillCoroutines(coroutine);
                    return;
                }

                Shoot();
            });
        }

        public void Shoot()
        {
            List<SphereCollider> colliders = [];

            for (int i = -1; i < 2; i++)
            {
                Quaternion rotation = Quaternion.Euler(0f, 10f * i, 0f);
                Vector3 direction = rotation * Caster.Player.Camera.forward;

                Projectile projectile = new(this, Caster.Player, Caster.Player.Camera.position + (direction * 0.3f), Quaternion.LookRotation(direction), direction * 13f);
                projectile.Init();
                if (!projectile.Collider)
                {
                    LabApi.Features.Console.Logger.Error("Failed to create thorn shot projectile, collider is null!");
                    continue;
                }

                colliders.Add(projectile.Collider);
            }

            for (int i = 0; i < colliders.Count; i++)
            {
                for (int j = 0; j < colliders.Count; j++)
                    Physics.IgnoreCollision(colliders[i], colliders[j], true);
            }

            PlaySound("cast");
        }

        public class Projectile(SpellBase spell, Player owner, Vector3 initialPosition, Quaternion initialRotation, Vector3 initialVelocity, float lifetime = 10f)
            : CasterBase.MagicProjectileBase(spell, owner, initialPosition, initialRotation, initialVelocity, lifetime)
        {
            private const float HomingRangeSqr = 25f;
            private float initialSpeed;
            private Player? homing;
            private List<Player> targets = null!;

            public override string SchematicName => "MagicMissile";

            public override bool UseGravity => false;

            public override float CollisionRadius => 0.05f;

            public override void Init()
            {
                base.Init();
                initialSpeed = InitialVelocity.magnitude;
                targets = Player.List.Where(p => p != Owner && p.IsAlive && p.Faction != Owner.Faction).ToList();
            }

            public override void Tick()
            {
                base.Tick();

                if (!Rigidbody)
                    return;

                if (homing == null)
                {
                    Player? targetHoming = null;
                    float dist = float.MaxValue;
                    foreach (Player p in targets)
                    {
                        float distSqr = (p.Position - Rigidbody.position).sqrMagnitude;

                        if (distSqr > HomingRangeSqr)
                            continue;

                        if (dist > distSqr)
                        {
                            targetHoming = p;
                            dist = distSqr;
                        }
                    }

                    homing = targetHoming;
                }
                else
                {
                    Vector3 dir = (homing.Position - Rigidbody.position).normalized;
                    Quaternion lookRot = Quaternion.LookRotation(dir);
                    Rigidbody.MoveRotation(Quaternion.RotateTowards(Rigidbody.rotation, lookRot, 180f * Time.fixedDeltaTime));
                    Rigidbody.linearVelocity = Rigidbody.transform.forward * initialSpeed;

                    if (!homing.IsAlive)
                    {
                        targets.Remove(homing);
                        homing = null;
                    }
                }
            }

            public override void Hit(Collision col, ReferenceHub? player)
            {
                if (player)
                {
                    player.playerStats.DealDamage(new ExplosionDamageHandler(new Footprint(Owner.ReferenceHub), InitialVelocity, 15f * (player.IsSCP() ? 2.25f : 1f), 100, ExplosionType.Disruptor));

                    if (player.roleManager.CurrentRole is IFpcRole role)
                    {
                        role.FpcModule.Motor.JumpController.ForceJump(1f);
                    }

                    Owner.SendHitMarker();
                }

                if (Rigidbody)
                {
                    LightSourceToy toy = LightSourceToy.Create(Rigidbody.position, null, false);
                    toy.Color = Color.magenta;
                    toy.Intensity = 10f;
                    LightExplosion.Create(toy, 40f);
                }

                Destroy();
            }
        }
    }
}
