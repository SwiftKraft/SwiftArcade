namespace SwiftArcadeMode.Features.SCPs.Upgrades.Content.SCP106.TeamPlayer
{
    using System.Linq;
    using LabApi.Features.Wrappers;
    using UnityEngine;

    public class SharedConsumption(UpgradePathPerkBase parent) : LifeConsumption(parent)
    {
        public override string Name => "Shared Consumption";

        public override string Description => base.Description + " Your team receives this as well.";

        public override void Effect()
        {
            foreach (Player p in Player.List.Where((p) => p.IsSCP && p.Role != PlayerRoles.RoleTypeId.Scp0492))
            {
                if (p.HumeShield < p.MaxHumeShield)
                    p.HumeShield = Mathf.Clamp(p.HumeShield + CurrentAmount, 0f, p.MaxHumeShield);
            }
        }
    }
}
