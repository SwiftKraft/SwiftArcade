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
        public override string Name => "Bolt of Darkness";

        public override Color BaseColor => Color.black;

        public override int RankIndex => 1;

        public override float CastTime => 0.5f;

        public override void Cast()
        {
            _ = new Projectile(this, Caster.Player.Camera.position + Caster.Player.Camera.forward * 0.4f, Caster.Player.Camera.rotation, Caster.Player.Camera.forward * 35f, 10f, Caster.Player);
            PlaySound("cast");
        }

        public class Projectile(SpellBase spell, Vector3 initialPosition, Quaternion initialRotation, Vector3 initialVelocity, float lifetime = 10f, Player? owner = null) : CasterBase.MagicProjectileBase(spell, initialPosition, initialRotation, initialVelocity, lifetime, owner)
        {
            public override string SchematicName => "BoltOfDarkness";

            public override float CollisionRadius => 0.1f;

            public override bool UseGravity => false;

            public override void Hit(Collision col, ReferenceHub player)
            {
                if (player)
                {
                    float damage = 90f;
                    player.playerEffectsController.EnableEffect<Sinkhole>(5f);

                    player.playerStats.DealDamage(new ExplosionDamageHandler(new Footprint(Owner.ReferenceHub), InitialVelocity, damage * (player.IsSCP() ? 5f : 1f), 100, ExplosionType.Disruptor));
                    Owner.SendHitMarker(2f);
                }

                Spell.PlaySound(Rigidbody.position, "hit");

                LightSourceToy toy = LightSourceToy.Create(Rigidbody.position, null, false);
                toy.Color = Color.white;
                toy.Intensity = 1f;
                LightExplosion.Create(toy, 15f);

                Destroy();
            }
        }
    }
}
