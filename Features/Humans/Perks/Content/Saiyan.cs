namespace SwiftArcadeMode.Features.Humans.Perks.Content
{
    using InventorySystem.Items.ThrowableProjectiles;
    using LabApi.Events.Handlers;
    using LabApi.Features.Wrappers;
    using SwiftArcadeMode.Utils.Voice;
    using UnityEngine;

    [Perk("Saiyan", Rarity.Epic)]
    public class Saiyan(PerkInventory inv) : PerkBase(inv)
    {
        private LightSourceToy toy = null!;
        private bool ascended = true;

        public override string Name => "Saiyan";

        public override string Description => "Max out your microphone to power up!\nPlease do this responsibly.";

        public float Degree { get; private set; }

        public virtual float DecayRate => 0.5f;

        public virtual float GainRate => 3f;

        public override void Init()
        {
            base.Init();
            PlayerEvents.SendingVoiceMessage += OnSendVoiceMessage;
            PlayerEvents.Death += OnDeath;
            toy = LightSourceToy.Create(Player.Position, networkSpawn: false);
            toy.Color = Color.yellow;
            toy.Intensity = 0f;
            toy.MovementSmoothing = 1;
            toy.SyncInterval = 0f;
            toy.Spawn();
        }

        public override void Remove()
        {
            base.Remove();
            PlayerEvents.SendingVoiceMessage -= OnSendVoiceMessage;
            PlayerEvents.Death -= OnDeath;
            toy.Destroy();
        }

        public override void Tick()
        {
            base.Tick();

            toy.Position = Player.Position;

            if (Degree > 0f)
                Degree -= Time.fixedDeltaTime * DecayRate;
            else
                Degree = 0f;

            toy.Intensity = Mathf.MoveTowards(toy.Intensity, Degree >= 10f ? Degree * Degree : 0f, (Degree >= 20f ? 2000f : 10f) * Time.fixedDeltaTime);

            if (Degree >= 30f && !ascended)
            {
                (TimedGrenadeProjectile.SpawnActive(Player.Position, ItemType.GrenadeHE, Player, 0f)?.Base as ExplosionGrenade)?.ScpDamageMultiplier = 6f;
                ascended = true;
            }
            else if (Degree >= 10f)
            {
                Player.StaminaRemaining += Time.fixedDeltaTime;
                Player.Heal(Time.fixedDeltaTime);

                Room? r = Player.Room;
                if (r != null && (!r.LightController?.LightsEnabled ?? false))
                    r.LightController.LightsEnabled = true;
            }
        }

        private void OnDeath(LabApi.Events.Arguments.PlayerEvents.PlayerDeathEventArgs ev)
        {
            if (ev.Player != Player)
                return;

            ascended = false;
            Degree = 0f;
            toy.Intensity = 0f;
        }

        private void OnSendVoiceMessage(LabApi.Events.Arguments.PlayerEvents.PlayerSendingVoiceMessageEventArgs ev)
        {
            if (ev.Player != Player || !Player.IsAlive)
                return;

            double loudnessDb = VoiceDecoding.CalculateLoudnessDB(ev.Message.ToPcm());
            if (loudnessDb > -30f)
                Degree += Time.fixedDeltaTime * GainRate;
        }
    }
}
