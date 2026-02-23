namespace SwiftArcadeMode.Features.Humans.Perks.Content.Caster.Spells
{
    using CustomPlayerEffects;
    using Footprinting;
    using LabApi.Features.Wrappers;
    using PlayerRoles;
    using PlayerStatsSystem;
    using SwiftArcadeMode.Utils.Projectiles;
    using UnityEngine;

    public class ElementalBolt : SpellBase
    {
        public override string Name => "Elemental Bolt";

        public override Color BaseColor => new(0.7f, 0.3f, 0f);

        public override int RankIndex => 1;

        public override float CastTime => 1f;

        public override void Cast()
        {
            new Projectile(this, Caster.Player.Camera.position + Caster.Player.Camera.forward * 0.4f, Caster.Player.Camera.rotation, Caster.Player.Camera.forward * 25f, 10f, Caster.Player);
            PlaySound("cast");
        }

        public class Projectile(SpellBase spell, Vector3 initialPosition, Quaternion initialRotation, Vector3 initialVelocity, float lifetime = 10f, Player owner = null) : CasterBase.MagicProjectileBase(spell, initialPosition, initialRotation, initialVelocity, lifetime, owner)
        {
            public override string SchematicName => "ElementalBolt";

            public override float CollisionRadius => 0.2f;

            public override bool UseGravity => false;

            public override void Hit(Collision col, ReferenceHub player)
            {
                if (player != null)
                {
                    float damage = 100f;

                    if (player.playerEffectsController.TryGetEffect<Burned>(out var playerEffect) && playerEffect != null)
                    {
                        if (!playerEffect.IsEnabled)
                            player.playerEffectsController.EnableEffect<Burned>(5f, true);
                        else
                            damage = 150f;
                    }

                    player.playerStats.DealDamage(new ExplosionDamageHandler(new Footprint(Owner.ReferenceHub), InitialVelocity, damage * (player.IsSCP() ? 4f : 1f), 100, ExplosionType.Disruptor));
                    Owner?.SendHitMarker(2f);
                }

                LightSourceToy toy = LightSourceToy.Create(Rigidbody.position, null, false);
                toy.Color = Color.white;
                toy.Intensity = 15f;

                LightExplosion.Create(toy, 30f);
                Destroy();
            }
        }
    }
}
