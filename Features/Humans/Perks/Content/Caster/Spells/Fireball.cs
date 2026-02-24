namespace SwiftArcadeMode.Features.Humans.Perks.Content.Caster.Spells
{
    using InventorySystem.Items.ThrowableProjectiles;
    using LabApi.Features.Wrappers;
    using PlayerRoles.FirstPersonControl;
    using UnityEngine;

    public class Fireball : SpellBase
    {
        public Fireball(CasterBase caster)
            : base(caster)
        {
        }

        public override string Name => "Fireball";

        public override Color BaseColor => Color.red;

        public override int RankIndex => 1;

        public override float CastTime => 1.3f;

        public override string[] SoundList => ["cast"];

        public override void Cast()
        {
            new Projectile(this, Caster.Player, Caster.Player.Camera.position + (Caster.Player.Camera.forward * 0.5f), Caster.Player.Camera.rotation, Caster.Player.Camera.forward * 11f, 15f).Init();
            PlaySound("cast");
        }

        public class Projectile(SpellBase spell, Player owner, Vector3 initialPosition, Quaternion initialRotation, Vector3 initialVelocity, float lifetime = 10f)
            : CasterBase.MagicProjectileBase(spell, owner, initialPosition, initialRotation, initialVelocity, lifetime)
        {
            public override string SchematicName => "Fireball";

            public override float CollisionRadius => 0.15f;

            public override bool UseGravity => false;

            public void Explode()
            {
                if (!Rigidbody)
                    return;

                TimedGrenadeProjectile? proj = TimedGrenadeProjectile.SpawnActive(Rigidbody.position, ItemType.GrenadeHE, Owner, 0f);
                if (proj?.Base is ExplosionGrenade gr)
                {
                    gr.ScpDamageMultiplier = 2f;
                    gr.MaxRadius = 3f;
                }

                foreach (Player player in Player.List)
                {
                    float distSqr = (player.Position - Rigidbody.position).sqrMagnitude;
                    if (distSqr <= 25f && player.RoleBase is IFpcRole fpc)
                        fpc.FpcModule.Motor.JumpController.ForceJump(Mathf.Lerp(1f, 10f, Mathf.InverseLerp(25f, 4f, distSqr)));
                }
            }

            public override void EndLife()
            {
                Explode();
                base.EndLife();
            }

            public override void Hit(Collision col, ReferenceHub? player)
            {
                Explode();
                Destroy();
            }
        }
    }
}
