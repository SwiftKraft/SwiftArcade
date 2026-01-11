using Hints;
using LabApi.Events.Arguments.Interfaces;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Arguments.ServerEvents;
using LabApi.Events.Handlers;
using LabApi.Features.Wrappers;
using MEC;
using System;
using System.Collections.Generic;
using UnityEngine;
using Logger = LabApi.Features.Console.Logger;

namespace SwiftArcadeMode.Features.Humans.Perks
{
    public static class PerkSpawner
    {
        public static bool AllowSpawn { get; set; } = true;

        public static readonly Dictionary<ushort, PerkAttribute> PerkPickups = [];
        public static readonly Dictionary<ushort, LightSourceToy> LightSources = [];

        public static void Enable()
        {
            ServerEvents.RoundStarted += OnRoundStarted;
            ServerEvents.WaveRespawned += OnWaveRespawned;
            ServerEvents.GeneratorActivated += OnGeneratorActivated;
            PlayerEvents.PickedUpItem += OnPickedUpItem;
            PlayerEvents.SearchingPickup += OnSearchingPickup;

            AllowSpawn = Core.Instance.Config.AllowPerkSpawning;
        }

        public static void Disable()
        {
            ServerEvents.RoundStarted -= OnRoundStarted;
            ServerEvents.WaveRespawned -= OnWaveRespawned;
            ServerEvents.GeneratorActivated -= OnGeneratorActivated;
            PlayerEvents.PickedUpItem -= OnPickedUpItem;
            PlayerEvents.SearchingPickup -= OnSearchingPickup;
        }

        private static void OnGeneratorActivated(GeneratorActivatedEventArgs ev) => SpawnPerks();

        private static void OnSearchingPickup(PlayerSearchingPickupEventArgs ev)
        {
            if (!PerkPickups.ContainsKey(ev.Pickup.Serial))
                return;

            try
            {
                Type type = PerkPickups[ev.Pickup.Serial].Perk;
                PerkManager.PerkProfile prof = PerkPickups[ev.Pickup.Serial].Profile;

                CheckPickupEventArgs chk = new(type, prof, ev);
                PerkEvents.OnCheckPickup(chk);

                ev.Player.SendHint(!string.IsNullOrWhiteSpace(chk.OverrideHint) ? chk.OverrideHint : $"Picking Up Perk: {prof.FancyName}\n{prof.Description}{(PerkManager.HasPerk(ev.Player, type) ? "\n\n<color=#FF0000><b>WARNING: Picking this up will remove the perk of the same type.</b></color>" : "")}", [HintEffectPresets.FadeOut()], 5f);
            }
            catch (Exception ex) { Logger.Error(ex); }
        }

        private static void OnPickedUpItem(PlayerPickedUpItemEventArgs ev)
        {
            if (!PerkPickups.ContainsKey(ev.Item.Serial))
                return;

            try
            {
                ev.Player.RemoveItem(ev.Item);

                AttemptAddEventArgs pick = new(ev, PerkPickups[ev.Item.Serial]);
                PerkEvents.OnAttemptAdd(pick);

                if (!pick.IsAllowed || !PerkManager.GivePerk(ev.Player, PerkPickups[ev.Item.Serial]))
                {
                    SpawnPerk(PerkPickups[ev.Item.Serial], ev.Player.Position);
                    return;
                }

                PerkEvents.OnPickedUpPerk(new(ev.Player, PerkPickups[ev.Item.Serial]));

                PerkPickups.Remove(ev.Item.Serial);

                if (LightSources.ContainsKey(ev.Item.Serial))
                {
                    LightSources[ev.Item.Serial].Destroy();
                    LightSources.Remove(ev.Item.Serial);
                }
            }
            catch (Exception ex) { Logger.Error(ex); }
        }

        private static void OnRoundStarted() => Timing.CallDelayed(0.1f, SpawnPerks);

        private static void OnWaveRespawned(WaveRespawnedEventArgs ev) => SpawnPerks();

        public static PerkSpawnRulesBase DefaultSpawnRules = new PerkSpawnRulesBasic();
        public static PerkSpawnRulesBase PerkSpawnRules = DefaultSpawnRules;

        public static void SpawnPerks()
        {
            if (!AllowSpawn)
                return;

            PerkSpawnRules.SpawnPerks();
        }

        public static Pickup SpawnPerk(PerkAttribute ty, Vector3 location)
        {
            Pickup p = Pickup.Create(ItemType.Coin, location, Quaternion.identity, new(4f, 4f, 4f));
            if (p != null)
            {
                PerkPickups.Add(p.Serial, ty);
                p.Weight *= 5f;
                p.Spawn();

                LightSourceToy toy = LightSourceToy.Create(p.Transform, false);
                toy.Intensity = 0.5f;
                if (ColorUtility.TryParseHtmlString(ty.Rarity.GetColor(), out Color col))
                    toy.Color = col;
                toy.Spawn();
                LightSources.Add(p.Serial, toy);
            }
            return p;
        }
    }

    public class PickedUpPerkEventArgs(Player p, PerkAttribute att) : EventArgs, IPlayerEvent
    {
        public Player Player { get; } = p;
        public PerkAttribute Perk { get; } = att;
    }

    public class AttemptAddEventArgs(PlayerPickedUpItemEventArgs ev, PerkAttribute att) : EventArgs, IPlayerEvent, IItemEvent, ICancellableEvent
    {
        public bool IsAllowed { get; set; } = true;

        public PerkAttribute Perk { get; } = att;
        public Player Player { get; } = ev.Player;
        public Item Item { get; } = ev.Item;
    }

    public class CheckPickupEventArgs(Type perk, PerkManager.PerkProfile prof, PlayerSearchingPickupEventArgs ev) : EventArgs, IPlayerEvent, IPickupEvent
    {
        public string OverrideHint { get; set; } = null;

        public Player Player { get; } = ev.Player;
        public Pickup Pickup { get; } = ev.Pickup;

        public Type Perk { get; } = perk;
        public PerkManager.PerkProfile Profile { get; } = prof;
    }
}
