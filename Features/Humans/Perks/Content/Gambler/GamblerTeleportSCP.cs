namespace SwiftArcadeMode.Features.Humans.Perks.Content.Gambler
{
    using System.Linq;
    using LabApi.Features.Wrappers;
    using SwiftArcadeMode.Utils.Extensions;

    public class GamblerTeleportSCP : GamblerEffectBase
    {
        public override bool Positive => false;

        public override int Weight => 1;

        public override string Explanation => "Good luck.";

        public override void Effect(Player player)
        {
            Player scp = Player.List.Where((p) => p.IsSCP && p.Role != PlayerRoles.RoleTypeId.Scp079).ToList().GetRandom();
            if (scp != null && player.Room?.Name != MapGeneration.RoomName.Pocket)
                scp.Position = player.Position;
        }
    }
}
