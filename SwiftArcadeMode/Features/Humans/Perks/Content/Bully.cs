namespace SwiftArcadeMode.Features.Humans.Perks.Content
{
    using System.Collections.Generic;
    using System.Linq;
    using LabApi.Features.Wrappers;
    using PlayerRoles.FirstPersonControl;
    using UnityEngine;

    [Perk("Bully", Rarity.Rare)]
    public class Bully(PerkInventory inv) : PerkBase(inv)
    {
        private readonly Dictionary<Player, Vector3> pushes = [];

        public override string Name => "Bully";

        public override string Description => "Push people when you are near them.";

        public virtual float Strength => 3f;

        public virtual float Distance => 0.4f;

        public override void Tick()
        {
            if (Player.RoleBase is not IFpcRole r || !r.FpcModule.IsGrounded || !r.FpcModule.Motor.MovementDetected)
                return;

            foreach (Player player in pushes.Keys)
            {
                if (player.RoleBase is IFpcRole role)
                    role.FpcModule.ServerOverridePosition(pushes[player]);
            }

            pushes.Clear();

            foreach (Player p in Player.List.Where(p => p != Player && (p.Position - Player.Position).sqrMagnitude <= Distance * Distance))
            {
                Vector3 dirForce = (p.Position - Player.Position).normalized * (Strength * Time.fixedDeltaTime);
                pushes.Add(p, p.Position + dirForce);
            }
        }
    }
}
