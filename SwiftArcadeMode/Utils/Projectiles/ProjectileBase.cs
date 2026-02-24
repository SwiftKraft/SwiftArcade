namespace SwiftArcadeMode.Utils.Projectiles
{
    using LabApi.Features.Wrappers;
    using PlayerRoles.FirstPersonControl;
    using ProjectMER.Features;
    using ProjectMER.Features.Objects;
    using SwiftArcadeMode.Utils.Extensions;
    using SwiftArcadeMode.Utils.Structures;
    using UnityEngine;
    using Logger = LabApi.Features.Console.Logger;

    public abstract class ProjectileBase
    {
        public static readonly LayerMask CollisionLayers = LayerMask.GetMask("Default", "Door", "Glass");
        public static readonly LayerMask IgnoreRaycastLayer = LayerMask.GetMask("Ignore Raycast");

        public ProjectileBase(Player owner, Vector3 initialPosition, Quaternion initialRotation, Vector3 initialVelocity, float lifetime = 10F)
        {
            InitialPosition = initialPosition;
            InitialRotation = initialRotation;
            InitialVelocity = initialVelocity;
            Lifetime.Reset(lifetime);
            Owner = owner;
            ProjectileManager.All.Add(this);
        }

        public Player Owner { get; private set; }

        public PrimitiveObjectToy? Parent { get; set; }

        public SchematicObject? Schematic { get; private set; }

        public Vector3 InitialPosition { get; private set; }

        public Quaternion InitialRotation { get; private set; }

        public Vector3 InitialVelocity { get; private set; }

        public abstract float CollisionRadius { get; }

        public Rigidbody? Rigidbody { get; private set; }

        public SphereCollider? Collider { get; private set; }

        public Timer Lifetime { get; } = new();

        /// <summary>
        /// Gets the name of the schematic corresponding to this projectile.
        /// </summary>
        /// <remarks>Leave empty for invisible projectile.</remarks>
        public virtual string SchematicName => string.Empty;

        /// <summary>
        /// Initializes this projectile.
        /// </summary>
        /// <remarks>MUST BE CALLED AFTER CONSTRUCTION.</remarks>
        public virtual void Init()
        {
            Parent = PrimitiveObjectToy.Create(InitialPosition, InitialRotation, networkSpawn: false);
            Parent.Flags = AdminToys.PrimitiveFlags.None;
            Parent.MovementSmoothing = 1;
            Parent.SyncInterval = 0f;
            Parent.Type = PrimitiveType.Cube;
            Parent.IsStatic = false;

            Rigidbody = Parent.GameObject.AddComponent<Rigidbody>();
            Rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            Rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
            Rigidbody.linearVelocity = InitialVelocity;

            Parent.Spawn();

            if (!string.IsNullOrWhiteSpace(SchematicName))
            {
                Schematic = ObjectSpawner.SpawnSchematic(SchematicName.ApplySchematicPrefix(), default, Quaternion.identity);
                Schematic?.transform.SetParent(Parent.Transform, false);
            }

            Construct();

            Collider = Parent.GameObject.AddComponent<SphereCollider>();
            if (CollisionRadius > 0f)
                Collider.radius = CollisionRadius;
            else
                Logger.Error("Collision radius is non-positive for projectile " + GetType().FullName);
            Collider.excludeLayers = LayerMask.GetMask("Hitbox");

            if (Owner is { RoleBase: IFpcRole role })
                Physics.IgnoreCollision(Collider, role.FpcModule.CharController, true);

            Parent.GameObject.AddComponent<ProjectileComponent>().Projectile = this;
        }

        public virtual void Construct()
        {
        }

        public virtual void Tick()
        {
            Lifetime.Tick(Time.fixedDeltaTime);

            if (Lifetime.Ended)
                EndLife();
        }

        public virtual void EndLife() => Destroy();

        public virtual void Destroy()
        {
            ProjectileManager.All.Remove(this);
            if (Parent?.Base)
                Parent.Destroy();
        }

        public void OnCollide(Collision cols)
        {
            cols.collider.TryGetComponent(out ReferenceHub? hub);
            Hit(cols, hub);
        }

        public abstract void Hit(Collision col, ReferenceHub? hit);
    }
}
