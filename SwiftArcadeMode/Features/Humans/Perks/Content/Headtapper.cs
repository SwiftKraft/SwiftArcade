namespace SwiftArcadeMode.Features.Humans.Perks.Content
{
    using LabApi.Events.Handlers;
    using PlayerStatsSystem;

    [Perk("Headtapper", Rarity.Uncommon)]
    public class Headtapper(PerkInventory inv) : PerkBase(inv)
    {
        public override string Name => "Headtapper";

        public override string Description => "Deal bonus damage when headshotting a human, \ntorso and limb shots damage reduced. \nSCP damage unchanged.";

        public virtual float HeadMultiplier => 2f;

        public virtual float BodyMultiplier => 0.5f;

        public override void Init()
        {
            base.Init();
            PlayerEvents.Hurting += OnPlayerHurting;
        }

        public override void Remove()
        {
            base.Remove();
            PlayerEvents.Hurting -= OnPlayerHurting;
        }

        private void OnPlayerHurting(LabApi.Events.Arguments.PlayerEvents.PlayerHurtingEventArgs ev)
        {
            if (ev.Attacker != Player || ev.Player.IsSCP || ev.DamageHandler is not FirearmDamageHandler handler)
                return;

            handler.Damage *= handler.Hitbox == HitboxType.Headshot ? HeadMultiplier : BodyMultiplier;
        }
    }
}
