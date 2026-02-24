namespace SwiftArcadeMode.Features.Humans.Perks.Content
{
    using CustomPlayerEffects;
    using LabApi.Events.Arguments.PlayerEvents;
    using UnityEngine;

    [Perk("RaceCar", Rarity.Legendary)]
    public class RaceCar(PerkInventory inv) : PerkKillBase(inv)
    {
        private byte currentStack;

        public override string Name => "Race Car";

        public override string Description => "Every kill increases your speed.";

        public virtual byte Amount => (byte)(Player.IsSCP ? 3 : 15);

        public virtual byte MaxValue => byte.MaxValue;

        public override void Remove()
        {
            base.Remove();
            Player.DisableEffect<MovementBoost>();
        }

        protected override void OnPlayerDying(PlayerDyingEventArgs ev)
        {
            if (ev.Attacker != Player)
                return;

            if (currentStack < MaxValue)
            {
                currentStack = (byte)Mathf.Clamp(currentStack + Amount, 0, MaxValue);
                Player.EnableEffect<MovementBoost>(currentStack);
                SendMessage("Gained Kill, Speed Level: " + currentStack);
            }
        }
    }
}
