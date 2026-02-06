using LabApi.Features.Wrappers;
using PlayerRoles;
using SwiftArcadeMode.Utils.Structures;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SwiftArcadeMode.Features.Humans.Perks.Content.Caster
{
    public abstract class TurretSummon(SpellBase spell, string name, string schematicName, RoleTypeId role, Vector3 colliderScale, Vector3 position, Quaternion rotation) : Summon(spell, name, schematicName, role, colliderScale, position, rotation)
    {
        public static readonly LayerMask LOSMask = LayerMask.GetMask("Default", "Door", "Glass");

        public Timer Timer = new();

        public abstract float Range { get; }
        public abstract float Delay { get; }

        public override void Initialize()
        {
            base.Initialize();
            Timer.Reset(Delay);
        }

        public override void Tick()
        {
            base.Tick();

            Timer.Tick(Time.fixedDeltaTime);
            if (Timer.Ended)
            {
                Timer.Reset(Delay);
                IEnumerable<Player> targets = Player.GetAll().Where(p => p.IsAlive && p.Faction != Dummy.Faction && !p.IsDisarmed && (Position - p.Position).sqrMagnitude <= Range * Range && CheckLOS(p.Camera.position));
                if (targets.Count() <= 0)
                    return;

                Player target = null;
                foreach (Player p in targets)
                {
                    if (target == null || (p.Position - Position).sqrMagnitude < (target.Position - Position).sqrMagnitude)
                        target = p;
                }

                if (target == null)
                    return;

                Attack(target);
            }
        }

        public abstract void Attack(Player target);

        public bool CheckLOS(Vector3 pos) => !Physics.Linecast(Dummy.Camera.position, pos, LOSMask, QueryTriggerInteraction.Ignore);
    }
}
