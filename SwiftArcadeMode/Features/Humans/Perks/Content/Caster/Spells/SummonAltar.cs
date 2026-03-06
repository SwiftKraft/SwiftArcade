namespace SwiftArcadeMode.Features.Humans.Perks.Content.Caster.Spells
{
    using System.Collections.Generic;
    using Footprinting;
    using LabApi.Features.Wrappers;
    using PlayerRoles;
    using PlayerStatsSystem;
    using SwiftArcadeMode.Utils.Deployable;
    using SwiftArcadeMode.Utils.Structures;
    using UnityEngine;
    using Random = UnityEngine.Random;

    public class SummonAltar : SummonSpell
    {
        public SummonAltar(CasterBase caster)
            : base(caster)
        {
        }

        public override string Name => "Summon Altar";

        public override Color BaseColor => new(0.5f, 0.05f, 0.05f);

        public override int RankIndex => 2;

        public override float CastTime => 3f;

        public override DeployableBase Create(Vector3 loc)
        {
            Altar altar = new(this, Caster.Player.DisplayName + "'s Altar", "Altar", loc, Quaternion.identity);
            altar.Initialize();
            return altar;
        }

        public class Altar(SpellBase spell, string name, string schematicName, Vector3 position, Quaternion rotation)
            : Summon(spell, name, schematicName, position, rotation)
        {
            private readonly List<Ghoul> spawned = [];

            public Timer SpawnTimer { get; set; } = new(10f);

            public int Limit { get; set; } = 3;

            public float Range { get; set; } = 3f;

            public float MaxHeight { get; set; } = 2f;

            public override float MaxHealth => 100f;

            public override float DestroyRange => 20f;

            public override void Initialize()
            {
                base.Initialize();
                SpawnTimer.Reset();
            }

            public override void Tick()
            {
                base.Tick();

                SpawnTimer.Tick(Time.fixedDeltaTime);
                if (SpawnTimer.Ended && Spawn())
                    SpawnTimer.Reset();
            }

            public bool Spawn()
            {
                Vector3 horizontalPos = (Random.insideUnitSphere * Range) + Position;
                horizontalPos.y = Position.y + (MaxHeight / 2f);

                if (Physics.Raycast(horizontalPos, Vector3.down, out RaycastHit hit, MaxHeight, LayerMask.GetMask("Default"), QueryTriggerInteraction.Ignore))
                {
                    Vector3 spawnPos = hit.point + Vector3.up;
                    Ghoul summon;
                    if (spawned.Count > 0 && spawned.Count >= Limit)
                        summon = spawned[0];
                    else
                    {
                        summon = new Ghoul(this, Spell, Owner.DisplayName + "'s Ghoul", "Ghoul", spawnPos, Quaternion.identity);
                        summon.Initialize();
                    }

                    spawned.Remove(summon);
                    spawned.Add(summon);
                    summon.Position = spawnPos;
                    return true;
                }

                return false;
            }

            public class Ghoul(Altar parent, SpellBase spell, string name, string schematicName, Vector3 position, Quaternion rotation)
                : TurretSummon(spell, name, schematicName, position, rotation)
            {
                public Altar Parent { get; } = parent;

                public override float Range => 7f;

                public override float Delay => 1f;

                public override float MaxHealth => 25f;

                public override float DestroyRange => 20f;

                public override void Attack(Player target)
                {
                    Vector3 direction = (target.Camera.position - Head.Transform.position).normalized;
                    new Projectile(Spell, Owner, Head.Transform.position, Quaternion.LookRotation(direction), direction * 15f, 5f).Init();
                }

                public override void Destroy()
                {
                    base.Destroy();
                    Parent.spawned.Remove(this);
                }

                public class Projectile(SpellBase spell, Player owner, Vector3 initialPosition, Quaternion initialRotation, Vector3 initialVelocity, float lifetime = 10)
                    : CasterBase.MagicProjectileBase(spell, owner, initialPosition, initialRotation, initialVelocity, lifetime)
                {
                    public override bool UseGravity => false;

                    public override float CollisionRadius => 0.1f;

                    public override string SchematicName => "GhoulProjectile";

                    public override void Init()
                    {
                        base.Init();
                        Collider?.material = new PhysicsMaterial { bounciness = 1f, bounceCombine = PhysicsMaterialCombine.Maximum, dynamicFriction = 0f, staticFriction = 0f, frictionCombine = PhysicsMaterialCombine.Multiply };
                    }

                    public override void Hit(Collision col, ReferenceHub? hit)
                    {
                        if (hit)
                        {
                            hit.playerStats.DealDamage(new ExplosionDamageHandler(new Footprint(Owner.ReferenceHub), InitialVelocity, 3f * (hit.IsSCP() ? 2f : 1f), 50, ExplosionType.Disruptor));
                            Owner.SendHitMarker(0.5f);
                        }
                    }
                }
            }
        }
    }
}
