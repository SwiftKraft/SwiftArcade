namespace SwiftArcadeMode.Features.Humans.Perks.Content.Caster.Spells
{
    using System.Collections.Generic;
    using Footprinting;
    using LabApi.Features.Wrappers;
    using PlayerRoles;
    using PlayerStatsSystem;
    using UnityEngine;

    public class ThornShot : SpellBase
    {
        private const float SpreadAngle = 65f;
        private const int ProjectileCount = 10;

        public ThornShot(CasterBase caster)
            : base(caster)
        {
        }

        public override string Name => "Thorn Shot";

        public override Color BaseColor => Color.green;

        public override int RankIndex => 2;

        public override float CastTime => 1.3f;

        public override void Cast()
        {
            const float AngleStep = SpreadAngle / (ProjectileCount - 1);
            const float StartAngle = -SpreadAngle / 2f;

            List<SphereCollider> colliders = [];
            for (int i = 0; i < ProjectileCount; i++)
            {
                float currentAngle = StartAngle + (AngleStep * i);
                Quaternion rotation = Quaternion.Euler(0f, currentAngle, 0f);
                Vector3 direction = rotation * Caster.Player.Camera.forward;

                Projectile projectile = new(this, Caster.Player, Caster.Player.Camera.position + (direction * 0.25f), Quaternion.LookRotation(direction), direction * 40f, 15f);
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
            public override string SchematicName => "ThornShot";

            public bool Sticked { get; private set; }

            public ReferenceHub? StuckPlayer { get; private set; }

            public Vector3 StuckOffset { get; private set; }

            public Quaternion StuckRotation { get; private set; }

            public override bool UseGravity => true;

            public override float CollisionRadius => 0.01f;

            public override void Tick()
            {
                if (!Sticked)
                    base.Tick();
                else
                {
                    if (StuckPlayer && Rigidbody)
                    {
                        Rigidbody.position = StuckPlayer.transform.position + (StuckPlayer.transform.rotation * StuckOffset);
                        Rigidbody.rotation = StuckPlayer.transform.rotation * StuckRotation;
                    }

                    Lifetime.Tick(Time.fixedDeltaTime);
                    if (Lifetime.Ended)
                        EndLife();
                }
            }

            public override void Hit(Collision col, ReferenceHub? hit)
            {
                if (hit && Rigidbody)
                {
                    hit.playerStats.DealDamage(new ExplosionDamageHandler(new Footprint(Owner.ReferenceHub), InitialVelocity, 20f * (hit.IsSCP() ? 3f : 1f), 25, ExplosionType.Grenade));

                    Owner.SendHitMarker();
                    StuckPlayer = hit;
                    StuckOffset = Rigidbody.position - StuckPlayer.transform.position;
                    StuckRotation = Quaternion.Inverse(StuckPlayer.transform.rotation) * Rigidbody.rotation;
                }

                Stick();
            }

            public void Stick()
            {
                if (!Rigidbody)
                    return;

                Lifetime.Reset();
                Sticked = true;
                Rigidbody.isKinematic = true;
                Rigidbody.detectCollisions = false;
            }
        }
    }
}
