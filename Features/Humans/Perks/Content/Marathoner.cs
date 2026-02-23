namespace SwiftArcadeMode.Features.Humans.Perks.Content
{
    using PlayerStatsSystem;

    [Perk("Marathoner", Rarity.Common)]
    public class Marathoner(PerkInventory inv) : PerkTriggerCooldownBase(inv)
    {
        public override string Name => "Marathoner";

        public override string Description => $"Regenerates your stamina by {Amount * 100f}% every {Cooldown} seconds.";

        public override string PerkDescription => string.Empty;

        public virtual float Amount => 0.1f;

        public override string ReadyMessage => string.Empty;

        public override float Cooldown => 8f;

        public override void Effect()
        {
            StaminaStat stat = Player.GetStatModule<StaminaStat>();
            if (Player.StaminaRemaining < stat.MaxValue)
                Player.StaminaRemaining += stat.MaxValue * Amount;
        }
    }
}
