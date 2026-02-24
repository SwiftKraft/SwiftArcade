namespace SwiftArcadeMode.Features.Humans.Perks.Content.Shopkeeper
{
    using System.Collections.Generic;
    using System.Linq;
    using LabApi.Events.Handlers;
    using LabApi.Features.Wrappers;
    using MapGeneration;
    using SwiftArcadeMode.Utils.Structures;
    using UnityEngine;

    [Perk("Shopkeeper", Rarity.Legendary)]
    public class Shopkeeper(PerkInventory inv) : PerkTriggerCooldownBase(inv)
    {
        private readonly Timer claimTimer = new(10f);
        private bool initialized;

        public static Dictionary<Room, Player> ClaimStatus { get; } = [];

        public override string Name => "Shopkeeper" + (initialized ? " | Shop Level " + ShopLevel : string.Empty);

        public override string PerkDescription => "Claim an entrance checkpoint to trade items. \nRestocks when you are in the room.";

        public int CustomerCount => Player.List.Count((p) => p.Room == Shop);

        public virtual ShopElement[] PresetElements => [
            new ShopItem(new(-5.5f, 1f, -5.25f), ShopItem.PresetTiers),
            new ShopItem(new(-5f, 1f, -5.25f), ShopItem.PresetTiers),
            new ShopItem(new(-4.5f, 1f, -5.25f), ShopItem.PresetTiers),
            new ShopItem(new(-4f, 1f, -5.25f), ShopItem.PresetTiers),
            new ShopItem(new(-3.5f, 1f, -5.25f), ShopItem.PresetTiers),
            new ShopItem(new(-3f, 1f, -5.25f), ShopItem.PresetTiers),
            new ShopItem(new(0.72f, 0.8f, -5.25f), ShopItem.PresetTiers),
            new ShopItem(new(1.84f, 0.8f, -5.25f), ShopItem.PresetTiers),
            new ShopItem(new(-1.37f, 0.8f, -5.25f), ShopItem.PresetTiers),
            new ShopItem(new(-0.4f, 0.8f, -5.25f), ShopItem.PresetTiers),
            new ShopItem(new(-2.22f, 0.3f, -2.59f), ShopItem.PresetTiers),
            new ShopItem(new(-2.22f, 0.3f, -4.13f), ShopItem.PresetTiers),
            ];

        public ShopElement[] Elements { get; private set; } = [];

        public int ShopExperience
        {
            get;
            set
            {
                if (field == value)
                    return;

                field = value;

                while (field >= RequiredExperience)
                {
                    field -= RequiredExperience;
                    ShopLevel++;
                }
            }
        }

        public int ShopLevel
        {
            get;
            set
            {
                if (field == value)
                    return;

                field = value;
                SendMessage("Shop is now Level " + field);
                Inventory.OnPerksUpdated();
            }
        }

        = 1;

        public int RequiredExperience => ShopLevel * Mathf.Max(Server.PlayerCount / 2, 4);

        public override string ReadyMessage => Shop == null ? "No shop found, please claim a shop." : CanRestock ? "Restocking..." : "Failed to restock, player is not in shop room.";

        public virtual bool CanRestock => Shop != null && Player.Room == Shop;

        public override float Cooldown => Mathf.Clamp(45f / Mathf.Max(CustomerCount / 5, 1), 5f, Mathf.Infinity);

        public Room? Shop { get; private set; }

        public override void Init()
        {
            base.Init();
            PlayerEvents.Death += OnDeath;
            PlayerEvents.ChangedRole += OnChangedRole;

            claimTimer.Reset();

            initialized = true;
        }

        public override void Effect() => Restock();

        public override void Tick()
        {
            base.Tick();

            if (Shop != null)
                return;

            Room? curRoom = Player.Room;
            if (curRoom is { Name: RoomName.HczCheckpointToEntranceZone, Zone: FacilityZone.HeavyContainment } && !ClaimStatus.ContainsKey(curRoom))
            {
                claimTimer.Tick(Time.fixedDeltaTime);

                if (claimTimer.Ended)
                    ClaimRoom(curRoom);
                else
                    SendMessage("Claiming checkpoint in: " + Mathf.Round(claimTimer.CurrentValue) + "s");
            }
            else
                claimTimer.Reset();
        }

        public void ClaimRoom(Room room)
        {
            UnclaimRoom();

            ClaimStatus[room] = Player;

            Elements = PresetElements;
            Shop = room;

            foreach (ShopElement element in Elements)
            {
                if (element is ShopElementOffset offset)
                    offset.UpdatePosition(room);

                element.Init(this);
            }

            SendMessage("Claimed checkpoint! Stay in the room to restock your shop.");
        }

        public void UnclaimRoom()
        {
            if (Shop == null)
                return;

            if (ClaimStatus.ContainsKey(Shop))
            {
                foreach (ShopElement element in Elements)
                    element.Remove();

                Elements = [];
                ClaimStatus.Remove(Shop);
                Shop = null;
                SendMessage("You have lost your shop. ");
                claimTimer.Reset();
            }
        }

        public void Restock()
        {
            if (!CanRestock)
                return;

            foreach (ShopElement element in Elements)
                element.Restock();
        }

        public override void Remove()
        {
            base.Remove();

            PlayerEvents.Death -= OnDeath;
            PlayerEvents.ChangedRole -= OnChangedRole;
            UnclaimRoom();
        }

        private void OnChangedRole(LabApi.Events.Arguments.PlayerEvents.PlayerChangedRoleEventArgs ev)
        {
            if (ev.Player != Player)
                return;

            UnclaimRoom();
        }

        private void OnDeath(LabApi.Events.Arguments.PlayerEvents.PlayerDeathEventArgs ev)
        {
            if (ev.Player != Player)
                return;

            UnclaimRoom();
        }
    }
}
