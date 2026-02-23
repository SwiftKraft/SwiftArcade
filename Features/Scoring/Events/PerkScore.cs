namespace SwiftArcadeMode.Features.Scoring.Events
{
    using SwiftArcadeMode.Features.Humans.Perks;

    public class PerkScore : ScoreEventBase
    {
        public override void Disable() => PerkEvents.PickedUpPerk -= OnPickedUpPerk;

        public override void Enable() => PerkEvents.PickedUpPerk += OnPickedUpPerk;

        protected virtual void OnPickedUpPerk(PickedUpPerkEventArgs ev) => ev.Player.AddScore(ev.Perk.Profile.Rarity switch
        {
            Rarity.Secret => 50,
            Rarity.Mythic => 35,
            Rarity.Legendary => 20,
            Rarity.Epic => 10,
            Rarity.Rare => 6,
            Rarity.Uncommon => 4,
            Rarity.Common => 2,
            _ => 1,
        });

        public override void Tick()
        {
        }
    }
}
