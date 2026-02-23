namespace SwiftArcadeMode.Features.Humans.Perks.Content
{
    using LabApi.Events.Arguments.PlayerEvents;
    using LabApi.Events.Handlers;
    using PlayerRoles;
    using UnityEngine;

    [Perk("Medic", Rarity.Common)]
    public class Medic(PerkInventory inv) : PerkBase(inv)
    {
        public override string Name => "Medic";

        public override string Description => $"Shooting a teammate will heal them by {Amount} HP.";

        public virtual float Amount => 5f;

        public override void Init()
        {
            base.Init();
            PlayerEvents.Hurting += OnHurting;
        }

        public override void Remove()
        {
            base.Remove();
            PlayerEvents.Hurting -= OnHurting;
        }

        private void OnHurting(PlayerHurtingEventArgs ev)
        {
            if (ev.Attacker != Player || ev.Player.Role.GetFaction() != ev.Attacker.Role.GetFaction())
                return;

            ev.IsAllowed = false;
            if (ev.Player.Health < ev.Player.MaxHealth)
                Player.SendHitMarker(0.25f);

            ev.Player.Heal(Amount);
            SendMessage($"Healed <b>{ev.Player.DisplayName}</b>: (<color=#{ColorUtility.ToHtmlStringRGB(Color.HSVToRGB(Mathf.Lerp(0f, 1f / 3f, Mathf.InverseLerp(0f, ev.Player.MaxHealth, ev.Player.Health)), 1f, 1f))}>{ev.Player.Health}</color>/<color=green>{ev.Player.MaxHealth}</color>)");
        }
    }
}
