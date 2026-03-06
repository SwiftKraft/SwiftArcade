namespace SwiftArcadeMode.Features.Humans.Perks.Content.Caster
{
    using System.Linq;
    using LabApi.Features.Wrappers;
    using PlayerRoles;
    using PlayerRoles.FirstPersonControl;
    using SwiftArcadeMode.Utils.Structures;
    using UnityEngine;

    public abstract class TurretSummon(SpellBase spell, string name, string schematicName, Vector3 position, Quaternion rotation)
        : Summon(spell, name, schematicName, position, rotation)
    {
        public static readonly LayerMask LineOfSightMask = FpcStateProcessor.Mask;

        public Timer Timer { get; } = new();

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

            if (!Timer.Ended)
                return;

            Timer.Reset(Delay);
            Player[] targets = Player.GetAll().Where(p => p.IsAlive && p.Faction != Footprint.Role.GetFaction() && !p.IsDisarmed && (Position - p.Position).sqrMagnitude <= Range * Range && CheckLOS(p.Camera.position)).ToArray();
            if (targets.Length is 0)
                return;

            Player? target = null;
            foreach (Player p in targets)
            {
                if (target == null || (p.Position - Position).sqrMagnitude < (target.Position - Position).sqrMagnitude)
                    target = p;
            }

            if (target == null)
                return;

            Attack(target);
        }

        public abstract void Attack(Player target);

        public bool CheckLOS(Vector3 pos) => !Physics.Linecast(Head.Transform.position, pos, out RaycastHit hitInfo, LineOfSightMask, QueryTriggerInteraction.Ignore);
    }
}
