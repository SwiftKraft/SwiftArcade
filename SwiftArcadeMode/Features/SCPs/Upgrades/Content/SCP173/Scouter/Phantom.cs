namespace SwiftArcadeMode.Features.SCPs.Upgrades.Content.SCP173.Scouter
{
    using LabApi.Events.Handlers;
    using LabApi.Features.Wrappers;
    using MEC;
    using Mirror;
    using NetworkManagerUtils.Dummies;
    using PlayerRoles;
    using PlayerRoles.FirstPersonControl;
    using PlayerRoles.Spectating;
    using UnityEngine;

    public class Phantom(UpgradePathPerkBase parent) : UpgradeBase<Scouter>(parent)
    {
        private ReferenceHub? phantom;

        public override string Name => "Phantom";

        public override string Description => "Spawns a phantom 173 when you start breakneck speeds.";

        public override void Init()
        {
            base.Init();
            Scp173Events.BreakneckSpeedChanged += OnBreakneckSpeedChanged;
            PlayerEvents.Hurt += OnHurt;
            PlayerEvents.ChangedRole += OnChangedRole;
        }

        public override void Remove()
        {
            base.Remove();
            Scp173Events.BreakneckSpeedChanged -= OnBreakneckSpeedChanged;
            PlayerEvents.Hurt -= OnHurt;
            PlayerEvents.ChangedRole -= OnChangedRole;

            DeletePhantom(true);
        }

        public void DeletePhantom(bool destroy = false, Player? attacker = null)
        {
            if (!phantom)
                return;

            if (phantom.roleManager.CurrentRole.RoleTypeId == RoleTypeId.Scp173)
            {
                SendMessage("Phantom destroyed! " + (attacker == null ? string.Empty : "Attacker: " + attacker.DisplayName));
                TimedGrenadeProjectile.SpawnActive(phantom.GetPosition(), ItemType.GrenadeFlash, Player, 0.1f);

                if (Room.TryGetRoomAtPosition(phantom.GetPosition(), out Room? room))
                    room.LightController?.FlickerLights(5f);

                phantom.roleManager.ServerSetRole(RoleTypeId.Filmmaker, RoleChangeReason.RemoteAdmin);
            }

            if (destroy)
                Timing.CallDelayed(0.1f, () => NetworkServer.Destroy(phantom.gameObject));
        }

        private void OnHurt(LabApi.Events.Arguments.PlayerEvents.PlayerHurtEventArgs ev)
        {
            if (ev.Player.ReferenceHub != phantom)
                return;

            DeletePhantom(false, ev.Attacker);
        }

        private void OnChangedRole(LabApi.Events.Arguments.PlayerEvents.PlayerChangedRoleEventArgs ev)
        {
            if (ev.Player != Player || ev.OldRole == ev.NewRole.RoleTypeId)
                return;

            DeletePhantom(true);
        }

        private void OnBreakneckSpeedChanged(LabApi.Events.Arguments.Scp173Events.Scp173BreakneckSpeedChangedEventArgs ev)
        {
            if (ev.Player != Player || !ev.Active)
                return;

            Vector3 pos = Player.Position;

            if (!phantom)
            {
                phantom = DummyUtils.SpawnDummy(Player.DisplayName + " (Phantom)");
                phantom.serverRoles.NetworkHideFromPlayerList = true;
                SpectatableVisibilityManager.SetHidden(phantom, true);
            }

            Timing.CallDelayed(0.1f, () =>
            {
                phantom.roleManager.ServerSetRole(RoleTypeId.Scp173, RoleChangeReason.RemoteAdmin);
                phantom.TryOverridePosition(pos);
            });
        }
    }
}
