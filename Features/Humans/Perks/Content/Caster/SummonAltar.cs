using Footprinting;
using LabApi.Features.Console;
using LabApi.Features.Wrappers;
using PlayerRoles;
using PlayerStatsSystem;
using SwiftArcadeMode.Utils.Deployable;
using SwiftArcadeMode.Utils.Extensions;
using SwiftArcadeMode.Utils.Structures;
using System.Collections.Generic;
using UnityEngine;
using Logger = LabApi.Features.Console.Logger;
using Random = UnityEngine.Random;

namespace SwiftArcadeMode.Features.Humans.Perks.Content.Caster
{
    public class SummonAltar : SummonSpell
    {
        public override string Name => "Summon Altar";

        public override Color BaseColor => new(0.5f, 0.05f, 0.05f);

        public override int RankIndex => 2;

        public override float CastTime => 3f;

        public override DeployableBase Create(Vector3 loc) => new Altar(this, Caster.Player.DisplayName + "'s Altar", "Altar".ApplySchematicPrefix(), Caster.Player.Role, new Vector3(1f, 0.25f, 1f), loc, Quaternion.identity)
        {
            Owner = Caster.Player
        };

        public class Altar(SpellBase spell, string name, string schematicName, RoleTypeId role, Vector3 colliderScale, Vector3 position, Quaternion rotation) : Summon(spell, name, schematicName, role, colliderScale, position, rotation)
        {
            public Timer SpawnTimer = new(10f);

            public int Limit = 3;
            public float Range = 3f;
            public float MaxHeight = 2f;

            readonly List<Ghoul> spawned = [];

            public override float Health => 100f;

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
                Vector3 horizontalPos = Random.insideUnitSphere * Range + Position;
                horizontalPos.y = Position.y + MaxHeight / 2f;

                if (Physics.Raycast(horizontalPos, Vector3.down, out RaycastHit hit, MaxHeight, LayerMask.GetMask("Default"), QueryTriggerInteraction.Ignore))
                {
                    Vector3 spawnPos = hit.point + Vector3.up;
                    Ghoul summon = spawned.Count > 0 && spawned.Count >= Limit ? spawned[0] : new(Spell, Owner.DisplayName + "'s Ghoul", "Ghoul".ApplySchematicPrefix(), Dummy.Role, new Vector3(0.5f, 0.5f, 0.5f), spawnPos, Quaternion.identity) { Owner = Owner };
                    spawned.Remove(summon);
                    spawned.Add(summon);
                    summon.Position = spawnPos;
                    summon.Parent = this;
                    return true;
                }

                return false;
            }

            public class Ghoul(SpellBase spell, string name, string schematicName, RoleTypeId role, Vector3 colliderScale, Vector3 position, Quaternion rotation) : TurretSummon(spell, name, schematicName, role, colliderScale, position, rotation)
            {
                public Altar Parent { get; set; }

                public override float Range => 7f;

                public override float Delay => 1f;

                public override float Health => 25f;

                public override float DestroyRange => 20f;

                public override void Attack(Player target)
                {
                    Vector3 direction = (target.Camera.position - Dummy.Camera.position).normalized;
                    new Projectile(Spell, Dummy.Camera.position, Quaternion.LookRotation(direction), direction * 15f, 5f, Dummy);
                }

                public override void Destroy()
                {
                    base.Destroy();
                    Parent.spawned.Remove(this);
                }

                public class Projectile(SpellBase spell, Vector3 initialPosition, Quaternion initialRotation, Vector3 initialVelocity, float lifetime = 10, Player owner = null) : CasterBase.MagicProjectileBase(spell, initialPosition, initialRotation, initialVelocity, lifetime, owner)
                {
                    public override bool UseGravity => false;

                    public override float CollisionRadius => 0.1f;

                    public override string SchematicName => "GhoulProjectile";

                    public override void Init()
                    {
                        base.Init();
                        Collider.material = new PhysicsMaterial() { bounciness = 1f, bounceCombine = PhysicsMaterialCombine.Maximum, dynamicFriction = 0f, staticFriction = 0f, frictionCombine = PhysicsMaterialCombine.Multiply };
                    }

                    public override void Hit(Collision col, ReferenceHub hit)
                    {
                        if (hit != null)
                        {
                            hit.playerStats.DealDamage(new ExplosionDamageHandler(new Footprint(Owner.ReferenceHub), InitialVelocity, 3f * (hit.IsSCP() ? 2f : 1f), 50, ExplosionType.Disruptor));
                            Owner?.SendHitMarker(0.5f);
                        }
                    }
                }
            }
        }
    }
}
