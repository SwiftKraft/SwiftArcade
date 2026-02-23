namespace SwiftArcadeMode.Features.SCPs.Upgrades.Content.SCP049.ArmyBuilder
{
    using System.Linq;
    using LabApi.Events.Arguments.Scp049Events;
    using LabApi.Events.Handlers;
    using LabApi.Features.Wrappers;
    using Mirror;
    using PlayerRoles;

    public class MassOperation(UpgradePathPerkBase parent) : UpgradeBase<ArmyBuilder>(parent)
    {
        public override string Name => "Mass Operation";

        public override string Description => $"Reviving a dead human also revives every available dead human in a {Range}m radius around it.";

        public virtual float Range => 5f;

        private bool midMassRevive;

        public override void Init()
        {
            base.Init();
            Scp049Events.ResurrectedBody += OnResurrected;
        }

        public override void Remove()
        {
            base.Remove();
            Scp049Events.ResurrectedBody -= OnResurrected;
        }

        private void OnResurrected(Scp049ResurrectedBodyEventArgs ev)
        {
            if (midMassRevive || ev.Player != Player)
                return;

            midMassRevive = true;
            foreach (Ragdoll rag in Ragdoll.List.Where((r) => r.IsRevivableBy(Player) && (r.Position - ev.Target.Position).sqrMagnitude < Range * Range))
            {
                rag.Base.Info.OwnerHub.roleManager.ServerSetRole(RoleTypeId.Scp0492, RoleChangeReason.Revived);
                NetworkServer.Destroy(rag.Base.gameObject);
                Scp049Events.OnResurrectedBody(new Scp049ResurrectedBodyEventArgs(rag.Base.Info.OwnerHub, Player.ReferenceHub));
            }

            midMassRevive = false;
        }
    }
}
