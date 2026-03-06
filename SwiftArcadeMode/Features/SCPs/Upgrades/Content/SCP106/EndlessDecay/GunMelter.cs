namespace SwiftArcadeMode.Features.SCPs.Upgrades.Content.SCP106.EndlessDecay
{
    using System.Collections.Generic;
    using System.Linq;
    using InventorySystem.Items.Firearms.Modules;
    using LabApi.Features.Wrappers;

    public class GunMelter(UpgradePathPerkBase parent) : UpgradeCooldownTriggerBase<EndlessDecay>(parent)
    {
        public static List<ItemType> BannedTypes { get; } =
        [
            ItemType.GunSCP127,
            ItemType.ParticleDisruptor
        ];

        public override string Name => "Gun Melter";

        public override string UpgradeDescription => $"Melts {Amount} bullets from every human's gun around you.";

        public override string ReadyMessage => string.Empty;

        public virtual int Amount => 3;

        public virtual float Radius => 5f;

        public override float Cooldown => 1f;

        public override void Effect()
        {
            foreach (Player p in Player.List.Where((p) => p.CurrentItem != null && !BannedTypes.Contains(p.CurrentItem.Type) && (p.Position - Player.Position).sqrMagnitude <= Radius * Radius))
            {
                if (p.CurrentItem is FirearmItem it && it.Base.TryGetModule(out IPrimaryAmmoContainerModule mod))
                    mod.ServerModifyAmmo(-Amount);
            }
        }
    }
}
