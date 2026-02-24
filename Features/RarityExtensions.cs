namespace SwiftArcadeMode.Features
{
    public static class RarityExtensions
    {
        public static string GetColor(this Rarity rarity) =>
            rarity switch
            {
                Rarity.Common => "#FFFFFF",
                Rarity.Uncommon => "#00FF00",
                Rarity.Rare => "#00FFFF",
                Rarity.Epic => "#FF00FF",
                Rarity.Legendary => "#FFFF00",
                Rarity.Mythic => "#0000FF",
                Rarity.Secret => "#FF0000",
                _ => "#FFFFFF"
            };
    }
}
