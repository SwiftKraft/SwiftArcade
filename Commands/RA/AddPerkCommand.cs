namespace SwiftArcadeMode.Commands.RA
{
    using System;
    using CommandSystem;
    using LabApi.Features.Wrappers;
    using SwiftArcadeMode.Features;

    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class AddPerkCommand : ICommand
    {
        public string Command => "addperk";

        public string[] Aliases => ["aperk", "addp", "ap"];

        public string Description => "Adds a perk to you or a player.";

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

            if (arguments.Count < 1 || !PerkManager.TryGetPerk(arguments.At(0).ToLower(), out PerkAttribute? t))
            {
                response = "Unknown perk! ";
                return false;
            }

            if (arguments.Count > 1)
                p = int.TryParse(arguments.At(1), out int id) ? Player.Get(id) : Player.Get(arguments.At(1));

            if (p is null)
            {
                response = "Could not find target player from second argument";
                return false;
            }

            p.GivePerk(t);

            response = "Added perk: " + t.ID + " to " + p.Nickname;
            return true;
        }
    }
}
