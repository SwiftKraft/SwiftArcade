namespace SwiftArcadeMode.Utils.Effects
{
    using System.Collections.Generic;
    using LabApi.Events.Handlers;
    using LabApi.Features.Wrappers;

    public static class CustomEffectManager
    {
        public static readonly Dictionary<Player, CustomEffectContainer> Containers = [];

        public static void Enable()
        {
            PlayerEvents.Dying += OnDying;
            PlayerEvents.Left += OnLeft;
        }

        public static void Disable()
        {
            PlayerEvents.Dying -= OnDying;
            PlayerEvents.Left -= OnLeft;
        }

        public static void Tick()
        {
            foreach (CustomEffectContainer cont in Containers.Values)
                cont?.Tick();
        }

        public static void Register(Player player)
        {
            if (!Containers.ContainsKey(player))
                Containers.Add(player, new CustomEffectContainer(player));
        }

        extension(Player player)
        {
            public void AddCustomEffect(CustomEffectBase effect)
            {
                Register(player);
                Containers[player].AddEffect(effect);
            }

            public void RemoveCustomEffect(CustomEffectBase effect)
            {
                if (Containers.TryGetValue(player, out CustomEffectContainer? container))
                    container.RemoveEffect(effect);
            }

            public void ClearCustomEffects()
            {
                if (Containers.TryGetValue(player, out CustomEffectContainer? container))
                    container.ClearEffects();
            }
        }

        private static void OnLeft(LabApi.Events.Arguments.PlayerEvents.PlayerLeftEventArgs ev)
        {
            if (Containers.ContainsKey(ev.Player))
            {
                ev.Player.ClearCustomEffects();
                Containers.Remove(ev.Player);
            }
        }

        private static void OnDying(LabApi.Events.Arguments.PlayerEvents.PlayerDyingEventArgs ev) => ev.Player.ClearCustomEffects();
    }
}
