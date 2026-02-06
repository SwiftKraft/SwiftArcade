using Hints;
using InventorySystem.Items.ThrowableProjectiles;
using LabApi.Features.Wrappers;
using PlayerRoles;
using SwiftArcadeMode.Utils.Deployable;
using SwiftArcadeMode.Utils.Extensions;
using SwiftArcadeMode.Utils.Structures;
using SwiftArcadeMode.Utils.Visuals;
using UnityEngine;

namespace SwiftArcadeMode.Features.Humans.Perks.Content.Caster
{
    public class SummonPylon : SummonSpell
    {
        public override string Name => "Summon Pylon";

        public override Color BaseColor => Color.cyan;

        public override int RankIndex => 1;

        public override float CastTime => 1f;

        public override DeployableBase Create(Vector3 loc) => new Pylon(this, Caster.Player.DisplayName + "'s Pylon", "Pylon".ApplySchematicPrefix(), Caster.Player.Role, new(1f, 0.5f, 1f), loc, Quaternion.identity)
        {
            Owner = Caster.Player
        };

        public class Pylon(SpellBase spell, string name, string schematicName, RoleTypeId role, Vector3 colliderScale, Vector3 position, Quaternion rotation) : Summon(spell, name, schematicName, role, colliderScale, position, rotation)
        {
            public override string TypeName => "Healing Pylon";

            public Timer HealDelay = new();

            public override float Health => 500f;

            public override float DestroyRange => 25f;

            public override void Initialize()
            {
                base.Initialize();
                HealDelay.Reset(2f);
            }

            public override void Tick()
            {
                base.Tick();
                HealDelay.Tick(Time.fixedDeltaTime);
                if (HealDelay.Ended)
                {
                    HealDelay.Reset();
                    SchematicEffect.Create("PylonVfx".ApplySchematicPrefix(), Position, Rotation, Vector3.one, 0.5f);
                    foreach (Player p in Player.List)
                    {
                        if (p == Dummy || p.Faction != Owner.Faction || !p.IsAlive || (p.Position - Dummy.Position).sqrMagnitude > 16f)
                            continue;

                        p.Heal(10f);
                    }
                }
            }

            public override void Destroy()
            {
                TimedGrenadeProjectile proj = TimedGrenadeProjectile.SpawnActive(Position, ItemType.GrenadeHE, null, 0f);
                if (proj.Base is ExplosionGrenade gr)
                {
                    gr.ScpDamageMultiplier = 1f;
                    gr.MaxRadius = 4f;
                }
                base.Destroy();
            }
        }
    }
}
