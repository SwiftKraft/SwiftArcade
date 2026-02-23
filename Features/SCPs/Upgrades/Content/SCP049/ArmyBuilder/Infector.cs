namespace SwiftArcadeMode.Features.SCPs.Upgrades.Content.SCP049.ArmyBuilder
{
    using LabApi.Events.Arguments.PlayerEvents;
    using LabApi.Events.Handlers;
    using MEC;
    using PlayerRoles;
    using UnityEngine;

    public class Infector(UpgradePathPerkBase parent) : UpgradeBase<ArmyBuilder>(parent)
    {
        public override string Name => "Infector";

        public override string Description => $"Zombies have a chance of {Chance * 100f}% of infecting a human upon kill.";

        public virtual float Chance => 0.35f;

        public override void Init()
        {
            base.Init();
            PlayerEvents.Dying += OnPlayerDying;
        }

        private void OnPlayerDying(PlayerDyingEventArgs ev)
        {
            if (ev.Attacker == null || ev.Attacker.Role != RoleTypeId.Scp0492 || !Parent.OwnedZombies.Contains(ev.Attacker) || Random.Range(0f, 1f) <= Chance)
                return;

            Timing.CallDelayed(0.2f, () => 
            { 
                ev.Player.SetRole(RoleTypeId.Scp0492); 
                Parent.AddZombie(ev.Player); 
                ev.Player.SendHint("You've been infected!"); 
            });
        }

        public override void Remove()
        {
            base.Remove();
            PlayerEvents.Dying -= OnPlayerDying;
        }
    }
}
