namespace SwiftArcadeMode.Features.Humans.Perks.Content.Caster.Spells
{
    using InventorySystem.Items.ThrowableProjectiles;
    using LabApi.Features.Wrappers;
    using PlayerRoles;
    using SwiftArcadeMode.Utils.Deployable;
    using SwiftArcadeMode.Utils.Structures;
    using SwiftArcadeMode.Utils.Visuals;
    using UnityEngine;

    public class SummonPylon : SummonSpell
    {
        public SummonPylon(CasterBase caster)
            : base(caster)
        {
        }

        public override string Name => "Summon Pylon";

        public override Color BaseColor => Color.cyan;

        public override int RankIndex => 1;

        public override float CastTime => 1f;

        public override DeployableBase Create(Vector3 loc)
        {
            Pylon pylon = new(this, Caster.Player.DisplayName + "'s Pylon", "Pylon", loc, Quaternion.identity);
            pylon.Initialize();
            return pylon;
        }

        public class Pylon(SpellBase spell, string name, string schematicName, Vector3 position, Quaternion rotation)
            : Summon(spell, name, schematicName, position, rotation)
        {
            public override string TypeName => "Healing Pylon";

            public Timer HealDelay { get; set; } = new();

            public override float MaxHealth => 150f;

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
                    SchematicEffect.Create("PylonVfx", Position, Rotation, Vector3.one, 0.5f);
                    foreach (Player p in Player.List)
                    {
                        if (p.Faction != Footprint.Role.GetFaction() || !p.IsAlive || (p.Position - Position).sqrMagnitude > 16f)
                            continue;

                        p.Heal(10f);
                    }
                }
            }

            public override void Destroy()
            {
                TimedGrenadeProjectile? proj = TimedGrenadeProjectile.SpawnActive(Position, ItemType.GrenadeHE, null, 0f);
                if (proj?.Base is ExplosionGrenade gr)
                {
                    gr.ScpDamageMultiplier = 1f;
                    gr.MaxRadius = 4f;
                }

                base.Destroy();
            }
        }
    }
}
