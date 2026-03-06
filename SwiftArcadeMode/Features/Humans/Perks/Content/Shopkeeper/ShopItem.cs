namespace SwiftArcadeMode.Features.Humans.Perks.Content.Shopkeeper
{
    using Hints;
    using LabApi.Events.Handlers;
    using LabApi.Features.Wrappers;
    using SwiftArcadeMode.Utils.Extensions;
    using UnityEngine;

    public class ShopItem : ShopElementOffset
    {
        private int lastRandom;

        public ShopItem(Vector3 offset, params ItemType[][] pools)
            : base(offset)
        {
            Items = pools;
        }

        public static ItemType[][] PresetTiers { get; } =
        [
            [ItemType.Painkillers, ItemType.Medkit, ItemType.Radio, ItemType.Flashlight],
            [ItemType.KeycardZoneManager, ItemType.Adrenaline, ItemType.Medkit, ItemType.Flashlight, ItemType.GunCOM15],
            [ItemType.KeycardGuard, ItemType.Adrenaline, ItemType.Medkit, ItemType.GunCOM18],
            [ItemType.KeycardMTFOperative, ItemType.Adrenaline, ItemType.Medkit, ItemType.GunCOM18, ItemType.GunFSP9, ItemType.ArmorLight],
            [ItemType.KeycardMTFCaptain, ItemType.Adrenaline, ItemType.Medkit, ItemType.GunFSP9, ItemType.GunCrossvec, ItemType.ArmorCombat],
            [ItemType.KeycardFacilityManager, ItemType.SCP500, ItemType.Adrenaline, ItemType.Medkit, ItemType.GunRevolver, ItemType.GunAK, ItemType.GunE11SR, ItemType.GunCrossvec, ItemType.GunLogicer, ItemType.GunFRMG0, ItemType.ArmorHeavy],
            [ItemType.KeycardFacilityManager, ItemType.SCP500, ItemType.Adrenaline, ItemType.Medkit, ItemType.GunRevolver, ItemType.GunAK, ItemType.GunE11SR, ItemType.GunCrossvec, ItemType.GunLogicer, ItemType.GunFRMG0, ItemType.ArmorHeavy, ItemType.GunCom45, ItemType.SCP207, ItemType.AntiSCP207],
        ];

        public ItemType[][] Items { get; }

        public Pickup? Item { get; private set; }

        public override void Init(Shopkeeper parent)
        {
            base.Init(parent);
            Restock();

            PlayerEvents.SearchedPickup += OnSearchedPickup;
            PlayerEvents.SearchingPickup += OnSearchingPickup;
        }

        public override void Restock()
        {
            if (Items.Length <= 0)
                return;

            int level = Mathf.Clamp(Parent.ShopLevel - 1, 0, Items.Length);
            ItemType[] pool = Items[level];

            if (pool.Length <= 0)
                return;

            if (Item != null && Item.Base)
                Item?.Destroy();
            Item = Pickup.Create(pool.GetRandom(ref lastRandom), Position, Rotation);
            Item?.Spawn();
        }

        public override void Remove()
        {
            if (Item != null && Item.Base)
                Item?.Destroy();
        }

        private void OnSearchedPickup(LabApi.Events.Arguments.PlayerEvents.PlayerSearchedPickupEventArgs ev)
        {
            if (ev.Pickup != Item || Item == null || ev.Player.CurrentItem == null)
                return;

            Parent.ShopExperience++;
            ev.Player.SendHint("Traded " + Translations.Get(ev.Player.CurrentItem.Type) + " for " + Translations.Get(Item.Type), [HintEffectPresets.FadeOut()]);
            ev.Player.RemoveItem(ev.Player.CurrentItem);
            Item = null;
        }

        private void OnSearchingPickup(LabApi.Events.Arguments.PlayerEvents.PlayerSearchingPickupEventArgs ev)
        {
            if (ev.Pickup != Item)
                return;

            if (ev.Player.CurrentItem == null)
            {
                ev.IsAllowed = false;
                ev.Player.SendHint("You must hold an item to trade.", [HintEffectPresets.FadeOut()], 2f);
                return;
            }

            ev.Player.SendHint("Trading " + Translations.Get(ev.Player.CurrentItem.Type) + " for " + Translations.Get(Item.Type), [HintEffectPresets.FadeOut()]);
        }
    }
}
