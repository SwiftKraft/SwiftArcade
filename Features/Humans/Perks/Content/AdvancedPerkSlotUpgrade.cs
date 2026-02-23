using LabApi.Features.Wrappers;

namespace SwiftArcadeMode.Features.Humans.Perks.Content
{
    using System;
    using LabApi.Events.Handlers;
    using UnityEngine;
    using Random = UnityEngine.Random;

    [Perk("AdvSlotUpgrade", Rarity.Mythic)]
    public class AdvancedPerkSlotUpgrade(PerkInventory inv) : PerkSlotUpgrade(inv)
    {
        public override string Name => "Advanced Perk Slot Upgrade";

        public override string Description => $"Gives you +{Amount} perk slots, takes up {SlotUsage} slots.\nGuaranteed loss of <color=red><b>ALL PERKS</b></color> when dying. \n{DropChance * 100f}% chance of dropping each removed perk, always drops itself.";

        public override int Amount => 12;

        public override int SlotUsage => 0;

        public virtual float DropChance => 0.3f;

        public override void Init()
        {
            base.Init();
            PlayerEvents.Dying += OnPlayerDying;
        }

        public override void Remove()
        {
            base.Remove();
            PlayerEvents.Dying -= OnPlayerDying;
        }

        private void OnPlayerDying(LabApi.Events.Arguments.PlayerEvents.PlayerDyingEventArgs ev)
        {
            if (ev.Player != Player || !Player.TryGetPerkInventory(out PerkInventory inv))
                return;

            Vector3 pos = Player.Position;

            PerkBase[] perks = [.. inv.Perks];
            foreach (PerkBase type in perks)
            {
                Type removed = type.GetType();
                inv.RemovePerk(removed);
                if (Random.Range(0f, 1f) <= DropChance || type == this)
                    PerkSpawner.SpawnPerk(PerkManager.GetPerk(removed), pos);
            }
        }
    }
}
