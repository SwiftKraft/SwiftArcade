namespace SwiftArcadeMode.Commands.Client
{
    using System;
    using CommandSystem;
    using LabApi.Features.Wrappers;
    using SwiftArcadeMode.Features;

    [CommandHandler(typeof(ClientCommandHandler))]
    public class ClientChooseUpgradeCommand : ICommand
    {
        public string Command => "chooseupgrade";

        public string[] Aliases => ["choose", "c"];

        public string Description => "Chooses an upgrade.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player? p = Player.Get(sender);
            if (p is null)
            {
                response = "You must be a player to use this command!";
                return false;
            }

            if (arguments.Count < 1 || !int.TryParse(arguments.At(0), out int num))
            {
                response = "Please input a number!";
                return false;
            }

            if (p.TryGetPerkInventory(out PerkInventory inv) && inv.UpgradeQueue.Upgrades.Count > 0)
            {
                bool success = inv.UpgradeQueue.Choose(num - 1, out string name);

                response = success ? ("Upgrade chosen: " + name + (inv.UpgradeQueue.Upgrades.Count > 0 ? ", " + inv.UpgradeQueue.Upgrades.Count + " more upgrade choices remain." : string.Empty)) : "Invalid index.";
                return success;
            }

            response = "No upgrades available.";
            return false;
        }
    }
}
