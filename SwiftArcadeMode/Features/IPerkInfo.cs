namespace SwiftArcadeMode.Features
{
    public interface IPerkInfo
    {
        Rarity Rarity { get; }

        PerkRestriction Restriction { get; }
    }
}
