namespace SwiftArcadeMode.Commands.RA
{
    using System;
    using CommandSystem;
    using LabApi.Features.Wrappers;
    using SwiftArcadeMode.Features;
    using SwiftArcadeMode.Features.SCPs.Upgrades;

    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class UpgradeCommand : ICommand
    {
        public string Command => "upgradeperk";

        public string[] Aliases => ["uperk", "upgradep", "up"];

        public string Description => "Upgrades an upgrade path for you or a player.";

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

            if (arguments.Count > 1)
                p = int.TryParse(arguments.At(1), out int id) ? Player.Get(id) : Player.Get(arguments.At(1));

            if (p is null)
            {
                response = "Could not find target player from second argument";
                return false;
            }

            if (!Player.TryGetPerkInventory(out PerkInventory inv) || !inv.TryGetPerk(t.Perk, out PerkBase perk) || perk is not UpgradePathPerkBase upg)
            {
                response = "Couldn't find the upgrade path!";
                return false;
            }

            upg.Progress++;

            response = "Upgraded perk: " + t.ID + " for " + p.Nickname;
            return true;
        }
    }
}
