namespace SwiftArcadeMode.Utils.Teams
{
    using System.Collections.Generic;
    using System.Linq;
    using LabApi.Features.Wrappers;

    public static class TeamManager
    {
        public static HashSet<Team> Teams { get; } = [];

        public static void LeaveTeam(this Player player) => Teams.FirstOrDefault(t => t.Has(player))?.Remove(player);

        public static void JoinTeam(this Player player, string teamName) => Teams.FirstOrDefault(t => t.Name.Equals(teamName))?.Add(player);
    }
}
