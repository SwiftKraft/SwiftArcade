namespace SwiftArcadeMode.Features.Humans.Perks.Content
{
    using UnityEngine;

    [Perk("Regeneration", Rarity.Common)]
    public class Regeneration(PerkInventory inv) : PerkBase(inv)
    {
        public override string Name => "Regeneration";

        public override string Description => $"When you have >{HealthThresholdPercentage * 100f}% HP, heal {Rate} HP/s.";

        public virtual float HealthThresholdPercentage => 0.6f;

        public virtual float Rate => 3f;

        public override void Tick()
        {
            base.Tick();

            if (Player.Health / Player.MaxHealth >= HealthThresholdPercentage && Player.Health < Player.MaxHealth)
                Player.Heal((Player.IsSCP ? 0.5f : 1f) * (Rate / (Player.Room.Name == MapGeneration.RoomName.Pocket ? 3f : 1f)) * Time.fixedDeltaTime);
        }
    }
}
