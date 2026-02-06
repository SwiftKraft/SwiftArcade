using LabApi.Features.Wrappers;
using PlayerRoles;
using SwiftArcadeMode.Utils.Deployable;
using SwiftArcadeMode.Utils.Extensions;
using UnityEngine;

namespace SwiftArcadeMode.Features.Humans.Perks.Content.Caster
{
    public class SummonArcaneGolem : SummonSpell
    {
        public override string Name => "Summon Arcane Golem";

        public override Color BaseColor => new(0.3f, 0f, 0.4f);

        public override int RankIndex => 2;

        public override float CastTime => 1f;

        public override int Limit => 1;

        public override DeployableBase Create(Vector3 loc) => new Golem(this, Caster.Player.DisplayName + "'s Golem", "ArcaneGolem".ApplySchematicPrefix(), Caster.Player.Role, new Vector3(1f, 0.5f, 1f), loc, Quaternion.identity)
        {
            Owner = Caster.Player
        };

        public class Golem(SpellBase spell, string name, string schematicName, RoleTypeId role, Vector3 colliderScale, Vector3 position, Quaternion rotation) : TurretSummon(spell, name, schematicName, role, colliderScale, position, rotation)
        {
            public override string TypeName => "Arcane Golem";

            public override float Health => 70f;

            public override float Range => 10f;

            public override float Delay => 1f;

            public override float DestroyRange => 20f;

            public override void Attack(Player target)
            {
                Vector3 direction = (target.Position - Dummy.Camera.position).normalized;
                new MagicMissile.Projectile(Spell, Dummy.Camera.position, Quaternion.LookRotation(direction), direction * 9f, 4f, Dummy);
            }
        }
    }
}
