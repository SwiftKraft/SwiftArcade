namespace SwiftArcadeMode.Features.Humans.Perks.Content.Caster.Spells
{
    using CustomPlayerEffects;
    using Footprinting;
    using LabApi.Features.Wrappers;
    using PlayerRoles;
    using PlayerStatsSystem;
    using UnityEngine;

    public class IceBolt : SpellBase
    {
        public override string Name => "Ice Bolt";

        public override Color BaseColor => Color.cyan;

        public override int RankIndex => 3;

        public override float CastTime => 0.5f;

        public override void Cast()
        {
            new Projectile(this, Caster.Player.Camera.position + Caster.Player.Camera.forward * 0.4f, Caster.Player.Camera.rotation, Caster.Player.Camera.forward * 30f, 10f, Caster.Player);
            PlaySound("cast");
        }

        public class Projectile(SpellBase spell, Vector3 initialPosition, Quaternion initialRotation, Vector3 initialVelocity, float lifetime = 10f, Player owner = null) : CasterBase.MagicProjectileBase(spell, initialPosition, initialRotation, initialVelocity, lifetime, owner)
        {
            public override string SchematicName => "IceBolt";

            public override float CollisionRadius => 0.1f;

            public override bool UseGravity => false;

            public override void Construct()
            {
                base.Construct();
            }

            public override void Hit(Collision col, ReferenceHub player)
            {
                if (player != null)
                {
                    float damage = 40f;

                    if (player.playerEffectsController.TryGetEffect<Ensnared>(out var playerEffect) && playerEffect != null)
                    {
                        if (!playerEffect.IsEnabled)
                            player.playerEffectsController.EnableEffect<Ensnared>(3f);
                        else
                            damage = 95f;
                    }

                    player.playerStats.DealDamage(new ExplosionDamageHandler(new Footprint(Owner.ReferenceHub), InitialVelocity, damage * (player.IsSCP() ? 3f : 1f), 100, ExplosionType.Disruptor));
                    Owner?.SendHitMarker(2f);
                    TimedGrenadeProjectile.PlayEffect(player.transform.position - Vector3.up, ItemType.SCP2176);
                }
                else
                    TimedGrenadeProjectile.PlayEffect(Rigidbody.position, ItemType.SCP2176);

                Destroy();
            }
        }
    }
}
