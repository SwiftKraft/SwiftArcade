namespace SwiftArcadeMode.Features.Humans.Perks.Content.Caster.Spells
{
    using CustomPlayerEffects;
    using Footprinting;
    using LabApi.Features.Wrappers;
    using PlayerRoles;
    using PlayerStatsSystem;
    using SwiftArcadeMode.Utils.Projectiles;
    using UnityEngine;

    public class BoltOfDarkness : SpellBase
    {
        public BoltOfDarkness(CasterBase caster)
            : base(caster)
        {
        }

        public override string Name => "Bolt of Darkness";

        public override Color BaseColor => Color.black;

        public override int RankIndex => 1;

        public override float CastTime => 0.5f;

        public override void Cast()
        {
            new Projectile(this, Caster.Player, Caster.Player.Camera.position + (Caster.Player.Camera.forward * 0.4f), Caster.Player.Camera.rotation, Caster.Player.Camera.forward * 35f).Init();
            PlaySound("cast");
        }

        public class Projectile(SpellBase spell, Player owner, Vector3 initialPosition, Quaternion initialRotation, Vector3 initialVelocity, float lifetime = 10f)
            : CasterBase.MagicProjectileBase(spell, owner, initialPosition, initialRotation, initialVelocity, lifetime)
        {
            public override string SchematicName => "BoltOfDarkness";

            public override float CollisionRadius => 0.1f;

            public override bool UseGravity => false;

            public override void Hit(Collision col, ReferenceHub? player)
            {
                if (player)
                {
                    float damage = 90f;
                    player.playerEffectsController.EnableEffect<Sinkhole>(5f);

                    player.playerStats.DealDamage(new ExplosionDamageHandler(new Footprint(Owner.ReferenceHub), InitialVelocity, damage * (player.IsSCP() ? 5f : 1f), 100, ExplosionType.Disruptor));
                    Owner.SendHitMarker(2f);
                }

                if (Rigidbody)
                {
                    Spell.PlaySound(Rigidbody.position, "hit");

                    LightSourceToy toy = LightSourceToy.Create(Rigidbody.position, null, false);
                    toy.Color = Color.white;
                    toy.Intensity = 1f;
                    LightExplosion.Create(toy, 15f);
                }

                Destroy();
            }
        }
    }
}
