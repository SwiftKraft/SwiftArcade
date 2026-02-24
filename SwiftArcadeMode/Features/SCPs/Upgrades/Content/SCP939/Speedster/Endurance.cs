namespace SwiftArcadeMode.Features.SCPs.Upgrades.Content.SCP939.Speedster
{
    using LabApi.Events.Arguments.PlayerEvents;
    using PlayerStatsSystem;

    public class Endurance(UpgradePathPerkBase parent) : UpgradeKillBase<Speedster>(parent)
    {
        public override string Name => "Endurance";

        public override string Description => "Replenish your stamina on kill.";

        protected override void OnPlayerDying(PlayerDyingEventArgs ev)
        {
            if (ev.Attacker != Player)
                return;

            Player.StaminaRemaining = Player.ReferenceHub.playerStats.GetModule<StaminaStat>().MaxValue;
        }
    }
}
