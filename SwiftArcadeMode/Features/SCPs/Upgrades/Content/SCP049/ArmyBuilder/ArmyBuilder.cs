namespace SwiftArcadeMode.Features.SCPs.Upgrades.Content.SCP049.ArmyBuilder
{
    using System;
    using System.Collections.Generic;
    using LabApi.Events.Arguments.PlayerEvents;
    using LabApi.Events.Arguments.Scp049Events;
    using LabApi.Events.Handlers;
    using LabApi.Features.Wrappers;
    using PlayerRoles;

    [UpgradePath(RoleTypeId.Scp049)]
    [Perk("049.ArmyBuilder", Rarity.Uncommon, PerkRestriction.SCP)]
    public class ArmyBuilder(PerkInventory inv) : UpgradePathPerkBase(inv)
    {
        public event Action<Player>? OnAddedZombie;

        public event Action<Player>? OnLostZombie;

        public event Action<Player>? OnZombieKilled;

        public override Type[] AllUpgrades => [
            typeof(EfficientReanimation),
            typeof(Infector),
            typeof(MassOperation)
            ];

        public List<Player> OwnedZombies { get; } = [];

        public override string PathName => "Army Builder";

        public override string PathDescription => "Focuses on zombie buffs.";

        public override void Init()
        {
            base.Init();
            Scp049Events.ResurrectedBody += OnResurrected;
            PlayerEvents.ChangedRole += OnChangedRole;
            PlayerEvents.Dying += OnDying;
        }

        public void AddZombie(Player p)
        {
            OnAddedZombie?.Invoke(p);
            OwnedZombies.Add(p);
            SendMessage("Added Zombie: " + p.DisplayName);
        }

        public override void Remove()
        {
            base.Remove();
            Scp049Events.ResurrectedBody -= OnResurrected;
            PlayerEvents.ChangedRole -= OnChangedRole;
            PlayerEvents.Dying -= OnDying;
        }

        private void OnDying(PlayerDyingEventArgs ev)
        {
            if (ev.Player.Role != RoleTypeId.Scp0492 || !OwnedZombies.Contains(ev.Player))
                return;

            OnZombieKilled?.Invoke(ev.Player);
        }

        private void OnChangedRole(PlayerChangedRoleEventArgs ev)
        {
            if (ev.OldRole == ev.NewRole.RoleTypeId || ev.OldRole != RoleTypeId.Scp0492 || !OwnedZombies.Contains(ev.Player))
                return;

            OnLostZombie?.Invoke(ev.Player);
            OwnedZombies.Remove(ev.Player);
            SendMessage("Lost Zombie: " + ev.Player.DisplayName);
        }

        private void OnResurrected(Scp049ResurrectedBodyEventArgs ev)
        {
            if (ev.Player != Player)
                return;

            AddZombie(ev.Target);
        }
    }
}
