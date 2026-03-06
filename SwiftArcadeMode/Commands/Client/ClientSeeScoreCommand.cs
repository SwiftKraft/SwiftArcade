namespace SwiftArcadeMode.Commands.Client
{
    using System;
    using CommandSystem;
    using LabApi.Features.Wrappers;
    using SwiftArcadeMode.Features.Scoring;

    [CommandHandler(typeof(ClientCommandHandler))]
    public class ClientSeeScoreCommand : ICommand
    {
        public string Command => "seescore";

        public string[] Aliases => ["ss", "sscore"];

        public string Description => "Shows you your current score.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player? p = Player.Get(sender);
            if (p is null)
            {
                response = "You must be a player to use this command!";
                return false;
            }

            response = $"Current score: {p.GetScore()}";
            return true;
        }
    }
}
