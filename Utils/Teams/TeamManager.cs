namespace SwiftArcadeMode.Utils.Teams
{
    using System.Collections.Generic;
    using System.Linq;
    using LabApi.Features.Wrappers;

    public static class TeamManager
    {
        public static HashSet<Team> Teams { get; } = [];

        extension(Player player)
        {
            public void LeaveTeam() => Teams.FirstOrDefault(t => t.Has(player))?.Remove(player);

            public void JoinTeam(string teamName) => Teams.FirstOrDefault(t => t.Name.Equals(teamName))?.Add(player);
        }
    }
}
