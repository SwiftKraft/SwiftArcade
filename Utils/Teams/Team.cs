namespace SwiftArcadeMode.Utils.Teams
{
    using System.Collections.Generic;
    using LabApi.Features.Wrappers;

    public class Team(string name, string description)
    {
        public string Name { get; set; } = name;

        public string Description { get; set; } = description;

        public HashSet<Player> Members { get; } = [];

        public bool Has(Player player) => Members.Contains(player);

        public bool Add(Player player)
        {
            player.LeaveTeam();
            return Members.Add(player);
        }

        public bool Remove(Player player) => Members.Remove(player);
    }
}
