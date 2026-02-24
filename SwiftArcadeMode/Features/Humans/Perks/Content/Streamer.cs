namespace SwiftArcadeMode.Features.Humans.Perks.Content
{
    using UnityEngine;

    [Perk("Streamer", Rarity.Secret)]
    public class Streamer(PerkInventory inv) : PerkBase(inv)
    {
        public override string Name => "Streamer";

        public override string Description => $"Every spectator currently watching you will heal you {RatePerPlayer} HP/s.\nDoesn't work in pocket dimension.";

        public virtual float RatePerPlayer => 3f;

        public override void Tick()
        {
            base.Tick();

            if (Player.CurrentSpectators.Count > 0 && Player.Room?.Name != MapGeneration.RoomName.Pocket)
                Player.Heal(RatePerPlayer * Time.fixedDeltaTime * Player.CurrentSpectators.Count);
        }
    }
}
