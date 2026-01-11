using LabApi.Features.Wrappers;
using System.Collections.Generic;
using Utils.NonAllocLINQ;

namespace SwiftArcadeMode.Utils.Teams
{
    public static class TeamManager
    {
        public static readonly HashSet<Team> Teams = [];

        public static void LeaveTeam(this Player player) => Teams.FirstOrDefault(t => t.Has(player), null)?.Remove(player);

        public static void JoinTeam(this Player player, string teamName) => Teams.FirstOrDefault(t => t.Name.Equals(teamName), null)?.Add(player);
    }

    public class Team(string name, string description)
    {
        public string Name { get; set; } = name;
        public string Description { get; set; } = description;
        public readonly HashSet<Player> Members = [];

        public bool Has(Player player) => Members.Contains(player);

        public bool Add(Player player)
        {
            player.LeaveTeam();
            return Members.Add(player);
        }

        public bool Remove(Player player) => Members.Remove(player);
    }
}
