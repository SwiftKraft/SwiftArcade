namespace SwiftArcadeMode.Commands.RA
{
    using CommandSystem;
    using SwiftArcadeMode.Features.SCPs.Upgrades;

    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class EnableSCPUpgradeCommand : ConfigCommand
    {
        public override string Command => "allowscpupgrades";

        public override string[] Aliases => ["scpupgrades", "scpup", "su"];

        public override string Description => "Enables or disables the natual gaining of upgrades for SCPs.";

        public override string Name => "Allow SCP Upgrades";

        public override void ChangeOption(bool option) => UpgradePathGiver.AllowLeveling = option;
    }
}
