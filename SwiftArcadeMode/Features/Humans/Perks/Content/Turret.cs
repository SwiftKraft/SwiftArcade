namespace SwiftArcadeMode.Features.Humans.Perks.Content
{
    using CustomPlayerEffects;
    using LabApi.Events.Arguments.PlayerEvents;
    using LabApi.Events.Handlers;
    using PlayerStatsSystem;
    using SwiftArcadeMode.Utils.Structures;
    using UnityEngine;

    [Perk("Turret", Rarity.Rare)]
    public class Turret(PerkInventory inv) : PerkBase(inv)
    {
        private readonly Timer timer = new();

        private bool originallyEnsnared;

        public override string Name => "Turret";

        public override string Description => $"Aiming down sights for at least {Duration} seconds will \nincrease your damage by x{Multiplier}, but you cannot move.";

        public virtual float Duration => 0.5f;

        public virtual float Multiplier => 2f;

        public bool AimStatus { get; private set; }

        public bool Effect
        {
            get;
            private set
            {
                if (field == value)
                    return;

                field = value;

                if (field)
                {
                    originallyEnsnared = Player.GetEffect<Ensnared>()?.IsEnabled ?? false;
                    Player.EnableEffect<Ensnared>();
                    Player.Position = Player.Position;
                    SendMessage("Damage Boosted!");
                }
                else
                {
                    if (!originallyEnsnared)
                        Player.DisableEffect<Ensnared>();
                    else
                        Player.EnableEffect<Ensnared>();
                    SendMessage("Unboosted.");
                }
            }
        }

        public override void Init()
        {
            base.Init();
            PlayerEvents.AimedWeapon += OnAimedWeapon;
            PlayerEvents.ChangedItem += OnChangedItem;
            PlayerEvents.Hurting += OnHurting;
        }

        public override void Tick()
        {
            base.Tick();
            if (AimStatus && Player.CurrentItem != null)
            {
                timer.Tick(Time.fixedDeltaTime);
                if (timer.Ended)
                    Effect = true;
            }
            else
            {
                Effect = false;
                if (Player.CurrentItem == null)
                    AimStatus = false;
            }
        }

        public override void Remove()
        {
            base.Remove();
            PlayerEvents.AimedWeapon -= OnAimedWeapon;
            PlayerEvents.ChangedItem -= OnChangedItem;
            PlayerEvents.Hurting -= OnHurting;
        }

        private void OnHurting(PlayerHurtingEventArgs ev)
        {
            if (ev.Attacker != Player || !Effect)
                return;

            if (ev.DamageHandler is StandardDamageHandler stn)
                stn.Damage *= Multiplier;
        }

        private void OnChangedItem(PlayerChangedItemEventArgs ev)
        {
            if (ev.Player != Player)
                return;
            AimStatus = false;
        }

        private void OnAimedWeapon(PlayerAimedWeaponEventArgs ev)
        {
            if (ev.Player != Player)
                return;

            timer.Reset(Duration);
            AimStatus = ev.Aiming;
        }
    }
}
