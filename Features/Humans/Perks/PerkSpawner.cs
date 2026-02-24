namespace SwiftArcadeMode.Features.Humans.Perks
{
    using System;
    using System.Collections.Generic;
    using Hints;
    using LabApi.Events.Arguments.PlayerEvents;
    using LabApi.Events.Arguments.ServerEvents;
    using LabApi.Events.Handlers;
    using LabApi.Features.Wrappers;
    using MEC;
    using SwiftArcadeMode.Features.Events;
    using UnityEngine;
    using Logger = LabApi.Features.Console.Logger;

    public static class PerkSpawner
    {
        public static bool AllowSpawn { get; set; } = true;

        public static Dictionary<ushort, PerkAttribute> PerkPickups { get; } = [];

        public static Dictionary<ushort, LightSourceToy> LightSources { get; } = [];

        public static PerkSpawnRulesBase DefaultSpawnRules { get; } = new PerkSpawnRulesBasic();

        public static PerkSpawnRulesBase PerkSpawnRules { get; set; } = DefaultSpawnRules;

        public static void Enable()
        {
            ServerEvents.RoundStarted += OnRoundStarted;
            ServerEvents.WaveRespawned += OnWaveRespawned;
            ServerEvents.GeneratorActivated += OnGeneratorActivated;
            PlayerEvents.PickedUpItem += OnPickedUpItem;
            PlayerEvents.SearchingPickup += OnSearchingPickup;

            AllowSpawn = Core.CoreConfig.AllowPerkSpawning;
        }

        public static void Disable()
        {
            ServerEvents.RoundStarted -= OnRoundStarted;
            ServerEvents.WaveRespawned -= OnWaveRespawned;
            ServerEvents.GeneratorActivated -= OnGeneratorActivated;
            PlayerEvents.PickedUpItem -= OnPickedUpItem;
            PlayerEvents.SearchingPickup -= OnSearchingPickup;
        }

        public static void SpawnPerks()
        {
            if (!AllowSpawn)
                return;

            PerkSpawnRules.SpawnPerks();
        }

        public static Pickup? SpawnPerk(PerkAttribute attribute, Vector3 location)
        {
            Pickup? p = Pickup.Create(ItemType.Coin, location, Quaternion.identity, new Vector3(4f, 4f, 4f));

            if (p == null)
                return p;

            PerkPickups.Add(p.Serial, attribute);
            p.Weight *= 5f;
            p.Spawn();

            LightSourceToy toy = LightSourceToy.Create(p.Transform, false);
            toy.Intensity = 0.5f;
            if (ColorUtility.TryParseHtmlString(attribute.Rarity.GetColor(), out Color col))
                toy.Color = col;
            toy.Spawn();
            LightSources.Add(p.Serial, toy);

            return p;
        }

        private static void OnGeneratorActivated(GeneratorActivatedEventArgs ev) => SpawnPerks();

        private static void OnSearchingPickup(PlayerSearchingPickupEventArgs ev)
        {
            if (!PerkPickups.ContainsKey(ev.Pickup.Serial))
                return;

            try
            {
                PerkAttribute attribute = PerkPickups[ev.Pickup.Serial];
                Type type = attribute.Perk;

#pragma warning disable CS0618 // Type or member is obsolete
                PerkProfile prof = PerkPickups[ev.Pickup.Serial].Profile;
                CheckPickupEventArgs chk = new(type, attribute, prof, ev);
#pragma warning restore CS0618 // Type or member is obsolete

                PerkEvents.OnCheckPickup(chk);

                ev.Player.SendHint(!string.IsNullOrWhiteSpace(chk.OverrideHint) ? chk.OverrideHint : $"Picking Up Perk: {attribute.HollowInstance.GetFancyName(ev.Player)}\n{prof.Description}{(ev.Player.HasPerk(type) ? "\n\n<color=#FF0000><b>WARNING: Picking this up will remove the perk of the same type.</b></color>" : string.Empty)}", [HintEffectPresets.FadeOut()], 5f);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
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

                if (!pick.IsAllowed || !ev.Player.GivePerk(PerkPickups[ev.Item.Serial]))
                {
                    SpawnPerk(PerkPickups[ev.Item.Serial], ev.Player.Position);
                    return;
                }

                PerkEvents.OnPickedUpPerk(new PickedUpPerkEventArgs(ev.Player, PerkPickups[ev.Item.Serial]));

                PerkPickups.Remove(ev.Item.Serial);

                if (LightSources.ContainsKey(ev.Item.Serial))
                {
                    LightSources[ev.Item.Serial].Destroy();
                    LightSources.Remove(ev.Item.Serial);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        private static void OnRoundStarted() => Timing.CallDelayed(0.1f, SpawnPerks);

        private static void OnWaveRespawned(WaveRespawnedEventArgs ev) => SpawnPerks();
    }
}
