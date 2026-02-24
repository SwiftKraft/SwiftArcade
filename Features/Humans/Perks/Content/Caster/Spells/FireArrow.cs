namespace SwiftArcadeMode.Features.Humans.Perks.Content.Caster.Spells
{
    using System.Collections.Generic;
    using System.Linq;
    using CustomPlayerEffects;
    using Footprinting;
    using LabApi.Features.Wrappers;
    using MEC;
    using PlayerRoles;
    using PlayerRoles.FirstPersonControl;
    using PlayerStatsSystem;
    using SwiftArcadeMode.Utils.Projectiles;
    using UnityEngine;

    public class FireArrow : SpellBase
    {
        private CoroutineHandle coroutine;

        public FireArrow(CasterBase caster)
            : base(caster)
        {
        }

        public override string Name => "Fire Arrow";

        public override Color BaseColor => Color.red;

        public override int RankIndex => 0;

        public override float CastTime => 0.5f;

        public override void Cast()
        {
            new Projectile(this, Caster.Player, Caster.Player.Camera.position, Caster.Player.Camera.rotation, Caster.Player.Camera.forward * 20f).Init();
            PlaySound("cast");

            coroutine = Timing.CallPeriodically(0.42f, 0.1f, () =>
            {
                if (!Caster.Player.IsAlive)
                {
                    Timing.KillCoroutines(coroutine);
                    return;
                }

                new Projectile(this, Caster.Player, Caster.Player.Camera.position + (Caster.Player.Camera.forward * 0.4f), Caster.Player.Camera.rotation, Caster.Player.Camera.forward * 20f).Init();
                PlaySound("cast");
            });
        }

        public class Projectile(SpellBase spell, Player owner, Vector3 initialPosition, Quaternion initialRotation, Vector3 initialVelocity, float lifetime = 10f)
            : CasterBase.MagicProjectileBase(spell, owner, initialPosition, initialRotation, initialVelocity, lifetime)
        {
            private const float HomingRangeSqr = 25f;
            private float initialSpeed;
            private Player? homing;

            private List<Player> targets = null!;

            public override string SchematicName => "FireArrow";

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
                    Rigidbody.MoveRotation(Quaternion.RotateTowards(Rigidbody.rotation, lookRot, 320f * Time.fixedDeltaTime));
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
                    player.playerStats.DealDamage(new ExplosionDamageHandler(new Footprint(Owner.ReferenceHub), InitialVelocity, 40f * (player.IsSCP() ? 3f : 1f), 100, ExplosionType.Disruptor));
                    player.playerEffectsController.EnableEffect<Burned>(3f, true);

                    if (player.roleManager.CurrentRole is IFpcRole role)
                    {
                        role.FpcModule.Motor.JumpController.ForceJump(1f);
                    }

                    Owner?.SendHitMarker();
                }

                if (Rigidbody)
                {
                    LightSourceToy toy = LightSourceToy.Create(Rigidbody.position, null, false);
                    toy.Color = Color.red;
                    toy.Intensity = 10f;
                    LightExplosion.Create(toy, 40f);
                }

                Destroy();
            }
        }
    }
}
