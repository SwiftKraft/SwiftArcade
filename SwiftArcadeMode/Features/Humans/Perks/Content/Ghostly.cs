namespace SwiftArcadeMode.Features.Humans.Perks.Content
{
    using CustomPlayerEffects;
    using LabApi.Events.Handlers;
    using UnityEngine;

    [Perk("Ghostly", Rarity.Legendary)]
    public class Ghostly(PerkInventory inv) : PerkBase(inv)
    {
        private Vector3 lastCheckedPosition;

        public override string Name => "Ghostly";

        public override string Description => "Phase through doors, you get more transparent the lower your health is.\nStanding still will make you invisible.";

        public float HealthPercentage => Player.Health / Player.MaxHealth;

        public byte CurrentDegree
        {
            get;
            private set
            {
                if (field != value)
                    Player.EnableEffect<Fade>(value);

                field = value;
            }
        }

        public float Moving { get; private set; }

        public override void Init()
        {
            base.Init();
            PlayerEvents.ChangedRole += OnChangedRole;
            Player.EnableEffect<CustomPlayerEffects.Ghostly>();
            lastCheckedPosition = Player.Position;
        }

        public override void Tick()
        {
            base.Tick();

            CurrentDegree = (byte)Mathf.Lerp(255, Mathf.Lerp(255, 0, HealthPercentage), Moving);
            bool moving = lastCheckedPosition != Player.Position;
            Moving = Mathf.MoveTowards(Moving, moving ? 1f : 0f, Time.fixedDeltaTime * (moving ? 2f : 0.25f));

            lastCheckedPosition = Player.Position;
        }

        public override void Remove()
        {
            base.Remove();
            PlayerEvents.ChangedRole -= OnChangedRole;
            Player.DisableEffect<CustomPlayerEffects.Ghostly>();
        }

        private void OnChangedRole(LabApi.Events.Arguments.PlayerEvents.PlayerChangedRoleEventArgs ev)
        {
            if (ev.Player != Player)
                return;

            Player.EnableEffect<CustomPlayerEffects.Ghostly>();
            lastCheckedPosition = Player.Position;
        }
    }
}
