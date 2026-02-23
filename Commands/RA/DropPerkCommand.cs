namespace SwiftArcadeMode.Commands.RA
{
    using System;
    using CommandSystem;
    using LabApi.Features.Wrappers;
    using SwiftArcadeMode.Features;
    using SwiftArcadeMode.Features.Humans.Perks;

    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class DropPerkCommand : ICommand
    {
        public string Command => "dropperk";

        public string[] Aliases => ["dperk", "dropp", "dp"];

        public string Description => "Drops a perk.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission(PlayerPermissions.Effects))
            {
                response = "No permission! ";
                return false;
            }

            Player? p = Player.Get(sender);
            if (p is null)
            {
                response = "You must be a player to use this command!";
                return false;
            }

            if (arguments.Count < 1 || !PerkManager.TryGetPerk(arguments.At(0).ToLower(), out PerkAttribute t))
            {
                response = "Unknown perk! ";
                return false;
            }

            PerkSpawner.SpawnPerk(t, p.Position);

            response = "Dropped perk: " + t.ID;
            return true;
        }
    }
}
