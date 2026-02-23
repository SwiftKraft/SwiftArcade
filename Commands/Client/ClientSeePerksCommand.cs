namespace SwiftArcadeMode.Commands.Client
{
    using System;
    using System.Text;
    using CommandSystem;
    using Hints;
    using LabApi.Features.Wrappers;
    using SwiftArcadeMode.Features;

    [CommandHandler(typeof(ClientCommandHandler))]
    public class ClientSeePerksCommand : ICommand
    {
        public string Command => "seeperks";

        public string[] Aliases => ["sperk", "sp"];

        public string Description => "Shows you all the perks that you have.";

        public static string GetPerks(Player p, out string brief)
        {
            if (!PerkManager.Inventories.ContainsKey(p))
            {
                brief = "You have no perks.";
                return brief;
            }

            StringBuilder stringBuilder = new("\n\n");
            StringBuilder hint = new("<align=\"left\">Current Perks: \n");

            foreach (PerkBase perk in PerkManager.Inventories[p].Perks)
            {
                stringBuilder.AppendLine();
                stringBuilder.Append(perk.FancyName);
                hint.Append(perk.FancyName);
                hint.AppendLine();
                stringBuilder.Append(" - ");
                stringBuilder.AppendLine(perk.Description);
            }

            hint.Append("</align>");

            brief = hint.ToString();
            return stringBuilder.ToString();
        }

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player? p = Player.Get(sender);
            if (p is null)
            {
                response = "You must be a player to use this command!";
                return false;
            }

            if (!PerkManager.Inventories.ContainsKey(p) || PerkManager.Inventories[p].Perks.Count <= 0)
            {
                response = "\n\nYou have no perks.";
                return true;
            }

            response = GetPerks(p, out string b);
            p.SendHint(b, [HintEffectPresets.FadeOut()], 10f);
            return true;
        }
    }
}
