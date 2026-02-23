namespace SwiftArcadeMode.Commands.RA
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using CommandSystem;
    using SwiftArcadeMode.Features.Scoring;

    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class ShowScoresCommand : ICommand
    {
        public string Command => "showscores";

        public string[] Aliases => ["sscores", "ss"];

        public string Description => "Shows everyone's scores.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            StringBuilder builder = new("All scores: \n");

            foreach (KeyValuePair<string, int> att in ScoringManager.Scores)
            {
                builder.Append("  ");
                builder.Append(ScoringManager.IDToName[att.Key]);
                builder.Append(" - ");
                builder.Append(att.Value);
                builder.Append("\n");
            }

            response = builder.ToString();

            return true;
        }
    }
}
