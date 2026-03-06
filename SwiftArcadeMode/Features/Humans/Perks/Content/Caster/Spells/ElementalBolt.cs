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
        public ElementalBolt(CasterBase caster)
            : base(caster)
        {
        }

        public override string Name => "Elemental Bolt";

        public override Color BaseColor => new(0.7f, 0.3f, 0f);

        public override int RankIndex => 1;

        public override float CastTime => 1f;

        public override void Cast()
        {
            new Projectile(this, Caster.Player, Caster.Player.Camera.position + (Caster.Player.Camera.forward * 0.4f), Caster.Player.Camera.rotation, Caster.Player.Camera.forward * 25f).Init();
            PlaySound("cast");
        }

        public class Projectile(SpellBase spell, Player owner, Vector3 initialPosition, Quaternion initialRotation, Vector3 initialVelocity, float lifetime = 10f)
            : CasterBase.MagicProjectileBase(spell, owner, initialPosition, initialRotation, initialVelocity, lifetime)
        {
            public override string SchematicName => "ElementalBolt";

            public override float CollisionRadius => 0.2f;

            public override bool UseGravity => false;

            public override void Hit(Collision col, ReferenceHub? player)
            {
                if (player)
                {
                    float damage = 100f;

                    if (player.playerEffectsController.TryGetEffect<Burned>(out Burned? playerEffect) && playerEffect != null)
                    {
                        if (!playerEffect.IsEnabled)
                            player.playerEffectsController.EnableEffect<Burned>(5f, true);
                        else
                            damage = 150f;
                    }

                    player.playerStats.DealDamage(new ExplosionDamageHandler(new Footprint(Owner.ReferenceHub), InitialVelocity, damage * (player.IsSCP() ? 4f : 1f), 100, ExplosionType.Disruptor));
                    Owner?.SendHitMarker(2f);
                }

                if (Rigidbody)
                {
                    LightSourceToy toy = LightSourceToy.Create(Rigidbody.position, null, false);
                    toy.Color = Color.white;
                    toy.Intensity = 15f;
                    LightExplosion.Create(toy, 30f);
                }

                Destroy();
            }
        }
    }
}
