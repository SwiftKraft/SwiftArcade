namespace SwiftArcadeMode.Commands.RA
{
    using CommandSystem;
    using SwiftArcadeMode.Features.Humans.Perks;

    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class EnableNaturalSpawningCommand : ConfigCommand
    {
        public override string Command => "allowperkspawning";

        public override string[] Aliases => ["spawnperks", "perkspawns", "pspawn", "aps"];

        public override string Description => "Enables or disables the natural spawning of perks (big coins).";

        public override string Name => "Allow Perk Spawning";

        public override void ChangeOption(bool option) => PerkSpawner.AllowSpawn = option;
    }
}
