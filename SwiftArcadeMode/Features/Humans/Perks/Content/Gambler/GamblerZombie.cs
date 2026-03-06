namespace SwiftArcadeMode.Features.Humans.Perks.Content.Gambler
{
    using LabApi.Features.Wrappers;

    public class GamblerZombie : GamblerEffectBase
    {
        public override bool Positive => false;

        public override int Weight => 1;

        public override string Explanation => "I guess you caught the virus...";

        public override void Effect(Player player) => player.SetRole(PlayerRoles.RoleTypeId.Scp0492, PlayerRoles.RoleChangeReason.Revived);
    }
}
