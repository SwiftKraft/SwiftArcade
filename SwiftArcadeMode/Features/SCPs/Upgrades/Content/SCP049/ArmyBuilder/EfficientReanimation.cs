namespace SwiftArcadeMode.Features.SCPs.Upgrades.Content.SCP049.ArmyBuilder
{
    using LabApi.Events.Arguments.PlayerEvents;
    using LabApi.Events.Handlers;
    using LabApi.Features.Wrappers;
    using PlayerRoles;

    public class EfficientReanimation(UpgradePathPerkBase parent) : UpgradeBase<ArmyBuilder>(parent)
    {
        public override string Name => "Efficient Reanimation";

        public override string Description => $"Revived zombies no longer receive max health debuffs, \nall zombies have {MaxHealth} max HP.\nZombies also heal on kill.";

        public virtual float MaxHealth => 600f;

        public virtual float Healing => 50f;

        public override void Init()
        {
            base.Init();
            Parent.OnAddedZombie += OnAddedZombie;
            PlayerEvents.Dying += OnPlayerDying;
        }

        public override void Remove()
        {
            base.Remove();
            Parent.OnAddedZombie -= OnAddedZombie;
            PlayerEvents.Dying -= OnPlayerDying;
        }

        private void OnAddedZombie(Player obj)
        {
            obj.MaxHealth = MaxHealth;
            obj.Health = MaxHealth;
        }

        private void OnPlayerDying(PlayerDyingEventArgs ev)
        {
            if (ev.Attacker is not { Role: RoleTypeId.Scp0492 } || !Parent.OwnedZombies.Contains(ev.Attacker))
                return;

            ev.Attacker.Heal(Healing);
        }
    }
}
