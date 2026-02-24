namespace SwiftArcadeMode.Features.Humans.Perks.Content
{
    using CustomPlayerEffects;
    using InventorySystem.Items.ThrowableProjectiles;
    using LabApi.Events.Arguments.PlayerEvents;
    using LabApi.Features.Wrappers;

    [Perk("Detonator", Rarity.Rare)]
    public class Detonator(PerkInventory inv) : PerkKillBase(inv)
    {
        public override string Name => "Detonator";

        public override string Description => "Upon killing a player that has the Burn effect,\ndrop a short fused grenade at their location.";

        protected override void OnPlayerDying(PlayerDyingEventArgs ev)
        {
            base.OnPlayerDying(ev);
            if (ev.Attacker != Player || !ev.Player.TryGetEffect(out Burned? burned) || !burned.IsEnabled)
                return;

            TimedGrenadeProjectile? proj = TimedGrenadeProjectile.SpawnActive(ev.Player.Position, ItemType.GrenadeHE, Player, 0.8f);
            if (proj?.Base is ExplosionGrenade gr)
                gr.ScpDamageMultiplier = 1.5f;
        }
    }
}
