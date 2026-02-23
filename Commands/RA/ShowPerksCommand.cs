namespace SwiftArcadeMode.Commands.RA
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using CommandSystem;
    using SwiftArcadeMode.Features;

    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class ShowPerksCommand : ICommand
    {
        public string Command => "seeperks";

        public string[] Aliases => ["sperks", "showp", "sp"];

        public string Description => "Shows all perks.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            StringBuilder builder = new("All available perks: \n");

            foreach (KeyValuePair<string, PerkAttribute> att in PerkManager.RegisteredPerks)
            {
                builder.Append("  ");
                builder.Append(att.Key);
                builder.Append(" - ");
                builder.Append(att.Value.Profile.FancyName);
                builder.Append("\n");
            }

            response = builder.ToString();

            return true;
        }
    }
}
