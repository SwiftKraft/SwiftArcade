namespace SwiftArcadeMode.Commands.RA
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using CommandSystem;
    using LabApi.Features.Wrappers;
    using SwiftArcadeMode.Features;

    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class ShowPerksCommand : ICommand
    {
        public string Command => "seeperks";

        public string[] Aliases => ["sperks", "showp", "sp"];

        public string Description => "Shows all perks.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player? user = Player.Get(sender);

            StringBuilder builder = new();

            if (user is null)
            {
                builder.AppendLine("<color=red>You are running this command as the server, there can be an error!</color>");
            }

            builder.AppendLine("All available perks:");

            foreach (KeyValuePair<string, PerkAttribute> att in PerkManager.RegisteredPerks)
            {
                builder.Append("  ");
                builder.Append(att.Key);
                builder.Append(" - ");
                builder.Append(att.Value.HollowInstance.GetFancyName(user!));
                builder.Append("\n");
            }

            response = builder.ToString();

            return true;
        }
    }
}
