namespace SwiftArcadeMode.Features.SCPs.Upgrades.Content.SCP939.Diseased
{
    using System.Collections.Generic;
    using LabApi.Events.Arguments.PlayerEvents;
    using LabApi.Events.Handlers;
    using LabApi.Features.Wrappers;
    using PlayerRoles.PlayableScps.Scp939;
    using SwiftArcadeMode.Utils.Structures;
    using UnityEngine;

    public class Bleeding(UpgradePathPerkBase parent) : UpgradeBase<Diseased>(parent)
    {
        public Dictionary<Player, Timer> Players { get; } = [];

        public override string Name => "Bleeding";

        public override string Description => $"Hitting a human will cause {TotalDamage} damage over {Duration}s.";

        public float TotalDamage => 15f;

        public float Duration => 5f;

        public override void Init()
        {
            base.Init();
            PlayerEvents.Hurt += OnHurt;
        }

        public override void Tick()
        {
            base.Tick();
            HashSet<Player> removal = [];

            foreach (Player player in Players.Keys)
            {
                if (!player.IsAlive)
                {
                    removal.Add(player);
                    continue;
                }

                player.Damage(TotalDamage / Duration * Time.fixedDeltaTime, Player, armorPenetration: 100);
                Players[player].Tick(Time.fixedDeltaTime);

                if (Players[player].Ended)
                    removal.Add(player);
            }

            foreach (Player pl in removal)
                Players.Remove(pl);
        }

        public override void Remove()
        {
            base.Remove();
            PlayerEvents.Hurt -= OnHurt;
        }

        private void OnHurt(PlayerHurtEventArgs ev)
        {
            if (ev.Attacker != Player || ev.DamageHandler is not Scp939DamageHandler)
                return;

            if (!Players.TryGetValue(ev.Player, out Timer? timer))
            {
                timer = new Timer(Duration);
                timer.Reset();
                Players.Add(ev.Player, timer);
            }
            else
                timer.Reset();
        }
    }
}
