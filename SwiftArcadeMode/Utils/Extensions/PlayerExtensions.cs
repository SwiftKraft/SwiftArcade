namespace SwiftArcadeMode.Utils.Extensions
{
    using System.Linq;
    using CustomPlayerEffects;
    using LabApi.Features.Wrappers;

    public static class PlayerExtensions
    {
        public static Elevator? GetElevator(this Player player)
        {
            foreach (Elevator e in Elevator.List.Where(e => e.WorldSpaceRelativeBounds.Bounds.Contains(player.Position)))
                return e;
            return null;
        }

        public static void ReapplyEffect<T>(this Player player, byte intensity = 1, float duration = 0, bool addDuration = false)
            where T : StatusEffectBase
        {
            if (player.TryGetEffect(out T? effect) && (!effect.IsEnabled || effect.Intensity != intensity))
                player.EnableEffect<T>(intensity, duration, addDuration);
        }
    }
}
