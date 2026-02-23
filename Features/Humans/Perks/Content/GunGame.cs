namespace SwiftArcadeMode.Features.Humans.Perks.Content
{
    using LabApi.Events.Arguments.PlayerEvents;
    using SwiftArcadeMode.Utils.Extensions;

    [Perk("GunGame", Rarity.Rare)]
    public class GunGame(PerkInventory inv) : PerkKillBase(inv)
    {
        private int lastRandom;

        public static ItemType[] Pool { get; } = [
            ItemType.GunA7,
            ItemType.GunAK,
            ItemType.GunRevolver,
            ItemType.GunFRMG0,
            ItemType.GunFSP9,
            ItemType.GunLogicer,
            ItemType.GunCOM15,
            ItemType.GunCOM18,
            ItemType.GunCom45,
            ItemType.GunE11SR,
            ItemType.GunCrossvec,
            ItemType.GunShotgun,
            ItemType.ParticleDisruptor,
            ];

        public override string Name => "Gun Game";

        public override string Description => "Randomizes your weapon on kill.";

        protected override void OnPlayerDying(PlayerDyingEventArgs ev)
        {
            if (ev.Attacker != Player || Player.CurrentItem == null || !Pool.Contains(Player.CurrentItem.Type))
                return;

            Player.RemoveItem(Player.CurrentItem);
            Player.CurrentItem = Player.AddItem(Pool.GetRandom(ref lastRandom));
        }
    }
}
