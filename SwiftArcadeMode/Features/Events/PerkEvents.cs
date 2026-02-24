namespace SwiftArcadeMode.Features.Events
{
    using LabApi.Events;

    using static SCPs.Upgrades.UpgradePathGiver;

    public static class PerkEvents
    {
        public static event LabEventHandler<AttemptAddEventArgs>? AttemptAdd;

        public static event LabEventHandler<CheckPickupEventArgs>? CheckPickup;

        public static event LabEventHandler<PickedUpPerkEventArgs>? PickedUpPerk;

        public static event LabEventHandler<ScpTeamLevelUpEventArgs>? ScpTeamLevelUp;

        public static void OnAttemptAdd(AttemptAddEventArgs ev) => AttemptAdd?.Invoke(ev);

        public static void OnCheckPickup(CheckPickupEventArgs ev) => CheckPickup?.Invoke(ev);

        public static void OnPickedUpPerk(PickedUpPerkEventArgs ev) => PickedUpPerk?.Invoke(ev);

        public static void OnScpTeamLevelUp(ScpTeamLevelUpEventArgs ev) => ScpTeamLevelUp?.Invoke(ev);
    }
}
