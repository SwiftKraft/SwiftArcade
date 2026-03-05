namespace SwiftArcadeMode.Features.SCPs.Upgrades.Content.SCP049.Overlord
{
    using System.Linq;
    using LabApi.Events.Handlers;
    using LabApi.Features.Wrappers;
    using PlayerStatsSystem;

    public class Soulbound(UpgradePathPerkBase parent) : UpgradeBase<Overlord>(parent)
    {
        public override string Name => "Soulbound";

        public override string Description => "When you take damage and your Hume Shield is 0, damage is spread across the zombies near you. \nWhen zombies die, you gain Hume Shield.";

        public float HumeGain => 150f;

        public float Range => 15f;

        public override void Init()
        {
            base.Init();
            PlayerEvents.Hurting += OnHurting;
            PlayerEvents.Dying += OnDying;
        }

        public override void Remove()
        {
            base.Remove();
            PlayerEvents.Hurting -= OnHurting;
            PlayerEvents.Dying -= OnDying;
        }

        private void OnDying(LabApi.Events.Arguments.PlayerEvents.PlayerDyingEventArgs ev)
        {
            if (ev.Player.Role != PlayerRoles.RoleTypeId.Scp0492)
                return;

            Player.HumeShield += HumeGain;
        }

        private void OnHurting(LabApi.Events.Arguments.PlayerEvents.PlayerHurtingEventArgs ev)
        {
            if (ev.Player != Player || ev.Attacker is null || Player.HumeShield > 0f || ev.DamageHandler is not StandardDamageHandler stand)
                return;

            Player[] zombies = Player.List.Where(p => p.IsSCP && p.Role == PlayerRoles.RoleTypeId.Scp0492 && (p.Position - Player.Position).sqrMagnitude <= Range * Range).ToArray();
            int count = zombies.Length;

            if (count <= 0)
                return;

            float dmg = stand.Damage / count;

            foreach (Player zombie in zombies)
                zombie.Damage(dmg, ev.Attacker, (Player.Position - zombie.Position).normalized * 100f, 100);

            ev.Attacker.SendHitMarker();

            ev.IsAllowed = false;
        }
    }
}
