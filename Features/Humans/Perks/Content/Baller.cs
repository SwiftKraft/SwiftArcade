using LabApi.Features.Wrappers;
using PlayerRoles.FirstPersonControl;
using UnityEngine;

namespace SwiftArcadeMode.Features.Humans.Perks.Content
{
    [Perk("Baller", Rarity.Secret)]
    public class Baller(PerkInventory inv) : PerkBase(inv)
    {
        public override string Name => "Baller";
        public override string Description => "Baller.\nSneak to move the ball towards you.";

        PrimitiveObjectToy ball;
        Rigidbody rb;

        public override void Init()
        {
            base.Init();
            ball = PrimitiveObjectToy.Create(Player.Position, Quaternion.identity, Vector3.one * 0.4f, networkSpawn: false);
            ball.Type = PrimitiveType.Sphere;
            ball.Color = Color.red;
            Object.Destroy(ball.Base.GetComponent<Collider>());
            PhysicsMaterial mat = new()
            {
                bounciness = 0.8f,
                bounceCombine = PhysicsMaterialCombine.Multiply,
                dynamicFriction = 0.01f,
                staticFriction = 0.01f,
                frictionCombine = PhysicsMaterialCombine.Multiply
            };
            ball.Base.gameObject.AddComponent<SphereCollider>().material = mat;
            rb = ball.Base.gameObject.AddComponent<Rigidbody>();
            rb.mass = 15f;
            rb.interpolation = RigidbodyInterpolation.Interpolate;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            ball.IsStatic = false;
            ball.SyncInterval = 0f;
            ball.Flags = AdminToys.PrimitiveFlags.Visible;
            ball.MovementSmoothing = 1;
            ball.Spawn();
        }

        public override void Tick()
        {
            base.Tick();
            foreach (Player p in Player.List)
            {
                if (!p.IsAlive || p.RoleBase is not IFpcRole role)
                    continue;

                Vector3 baller = p.Position;
                baller.y = rb.position.y;
                Vector3 dir = rb.position - baller;
                float sqrMag = (p.Position - ball.Position).sqrMagnitude;
                if ((role.FpcModule.Motor.MovementDetected || role.FpcModule.CurrentMovementState == PlayerMovementState.Sneaking) && sqrMag <= 2f && sqrMag >= 0.3f)
                    rb.AddForce((role.FpcModule.CurrentMovementState == PlayerMovementState.Sneaking ? -dir : role.FpcModule.Motor.Velocity) + Vector3.up * 0.1f, ForceMode.Impulse);
            }
        }

        public override void Remove()
        {
            base.Remove();
            ball.Destroy();
        }
    }
}
