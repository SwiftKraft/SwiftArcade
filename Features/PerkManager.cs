namespace SwiftArcadeMode.Features
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using LabApi.Events.Arguments.PlayerEvents;
    using LabApi.Events.Handlers;
    using LabApi.Features.Wrappers;
    using SwiftArcadeMode.Features.Humans.Perks.Content.Gambler;
    using SwiftArcadeMode.Features.Humans.Perks.Content.SixthSense;
    using SwiftArcadeMode.Utils.Extensions;
    using Logger = LabApi.Features.Console.Logger;

    public static class PerkManager
    {
        public const string PerkNameSpace = "base";

        public static Dictionary<Player, PerkInventory> Inventories { get; } = new();

        public static Dictionary<string, PerkAttribute> RegisteredPerks { get; } = new();

        public static void Enable()
        {
            if (Core.CoreConfig.AllowBaseContent)
            {
                RegisterPerks(PerkNameSpace);
                Gambler.RegisterEffects();
                SixthSense.RegisterSenses();
            }

            PlayerEvents.Death += OnPlayerDeath;
            PlayerEvents.ChangedSpectator += OnChangedSpectator;
            PlayerEvents.ChangedRole += OnChangedRole;
            ServerEvents.RoundRestarted += OnRoundRestarted;
        }

        public static void Disable()
        {
            PlayerEvents.Death -= OnPlayerDeath;
            PlayerEvents.ChangedSpectator -= OnChangedSpectator;
            PlayerEvents.ChangedRole -= OnChangedRole;
            ServerEvents.RoundRestarted -= OnRoundRestarted;
        }

        public static string FancifyPerkName(this string perkName, Rarity rarity) => $"<color={rarity.GetColor()}><b>{perkName}</b></color>";

        public static void RegisterPerks(string nameSpace)
        {
            Assembly callingAssembly = Assembly.GetCallingAssembly();

            Dictionary<Type, PerkAttribute> atts = callingAssembly
                .GetTypes()
                .Select(t => (type: t, attr: t.GetCustomAttribute<PerkAttribute>()))
                .Where(pair => pair.attr != null)
                .ToDictionary(pair => pair.type, pair => pair.attr);

            foreach (KeyValuePair<Type, PerkAttribute> attr in atts)
            {
                try
                {
                    attr.Value.Perk = attr.Key;
                    RegisteredPerks.Add((RegisteredPerks.ContainsKey(attr.Value.ID) ? nameSpace.ToLower() + "." : string.Empty) + attr.Value.ID.ToLower(), attr.Value);

                    PerkBase p;

                    try
                    {
                        p = (PerkBase)Activator.CreateInstance(attr.Key, null);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error($"Exception created when registering perk \"{attr.Value.ID}\"! Please ensure you do not define a custom constructor for your perks as an instance of all perks with a null Inventory and Owner are required.");
                        Logger.Error(ex);
                        continue;
                    }

                    attr.Value.HollowInstance = p;

                    // obsolete PerkProfile support
#pragma warning disable CS0618 // Type or member is obsolete
                    if (attr.Value.Profile.Name == string.Empty && attr.Value.Profile.Description == string.Empty)
                        attr.Value.Profile = new PerkProfile(attr.Value.Profile.Rarity, p.Name, p.Description);
#pragma warning restore CS0618 // Type or member is obsolete
                }
                catch (Exception ex)
                {
                    Logger.Error("Failed to load perk \"" + attr.Key + "\": \n" + ex);
                }
            }
        }

        public static PerkAttribute? GetRandomPerk(Func<PerkAttribute, bool> f) => RegisteredPerks.Values.Where(f).ToArray().GetWeightedRandom();

        public static PerkAttribute? GetPerk(string id) => RegisteredPerks.ContainsKey(id) ? RegisteredPerks[id] : null;

        public static PerkAttribute? GetPerk(Type type) => RegisteredPerks.Values.FirstOrDefault(att => att.Perk == type);

        public static bool TryGetPerk(string id, [NotNullWhen(true)] out PerkAttribute? t)
        {
            t = GetPerk(id);
            return t != null;
        }

        public static bool TryGetPerk(Type type, [NotNullWhen(true)] out PerkAttribute? t)
        {
            t = GetPerk(type);
            return t != null;
        }

        extension(Player player)
        {
            public bool HasRestrictions(IPerkInfo perk) => player.IsAlive && ((player.IsHuman && perk.Restriction == PerkRestriction.SCP) || (player.IsSCP && perk.Restriction == PerkRestriction.Human));

            public void UpdateSpectatorDisplay(Player target)
            {
                StringBuilder builder = new($"<align=\"left\">\n\n\n{target.DisplayName}'s Perks\n");

                if (Inventories.TryGetValue(target, out PerkInventory? inventory))
                {
                    foreach (PerkBase perk in inventory.Perks)
                        builder.AppendLine($"- {perk.GetFancyName(target)}");
                }

                builder.Append("</align>");

                player.SendHint(builder.ToString(), 120f);
            }

            // Note: The previous version of this was literally just GetPerkInventory with extra steps so expect stuff breaking (will obsolete if necessary)
            public bool TryGetPerkInventory(out PerkInventory inventory) => Inventories.TryGetValue(player, out inventory);

            public PerkInventory GetPerkInventory() => Inventories.ContainsKey(player) ? Inventories[player] : Register(player);

            public bool GivePerk(Type t) => TryGetPerk(t, out PerkAttribute? att) && player.GivePerk(att);

            public bool GivePerk(PerkAttribute t)
            {
                if (!Inventories.ContainsKey(player))
                    Register(player);

                return Inventories[player].AddPerk(t);
            }

            public void RemovePerk(Type t)
            {
                if (!Inventories.TryGetValue(player, out PerkInventory? inventory))
                {
                    Register(player);
                    return;
                }

                inventory.RemovePerk(t);
            }

            public bool HasPerk(Type perk) => Inventories.ContainsKey(player) && Inventories[player].HasPerk(perk);
        }

        public static PerkInventory Register(Player p)
        {
            if (Inventories.TryGetValue(p, out PerkInventory? inventory))
                return inventory;

            PerkInventory inv = new(p);
            Inventories.Add(p, inv);
            return inv;
        }

        public static void Remove(Player p) => Inventories.Remove(p);

        public static void Tick()
        {
            foreach (PerkInventory inv in Inventories.Values)
                inv.Tick();
        }

        private static void OnChangedRole(PlayerChangedRoleEventArgs ev)
        {
            ev.Player.SendHint(string.Empty, 1f);

            if (ev.Player.TryGetPerkInventory(out PerkInventory inv))
            {
                inv.BaseLimit = ev.Player.IsSCP ? 2 : 5;

                if (inv.Perks.Count > 0)
                {
                    List<PerkBase> perksToRemove = [];
                    foreach (PerkBase p in inv.Perks)
                    {
                        if (ev.Player.HasRestrictions(p))
                            perksToRemove.Add(p);
                    }

                    if (perksToRemove.Count > 0)
                    {
                        foreach (PerkBase p in perksToRemove)
                            inv.RemovePerk(p);

                        ev.Player.SendHint("Removed " + perksToRemove.Count + " perks, because of role incompatibility.", 5f);
                    }
                }
            }
        }

        private static void OnChangedSpectator(PlayerChangedSpectatorEventArgs ev) => ev.Player.UpdateSpectatorDisplay(ev.NewTarget);

        private static void OnRoundRestarted() => Inventories.Clear();

        private static void OnPlayerDeath(PlayerDeathEventArgs ev)
        {
            if (ev.Player.TryGetPerkInventory(out PerkInventory inv) && inv.Perks.Count > 0)
            {
                inv.RemoveRandom();
                inv.UpgradeQueue.Upgrades.Clear();
            }
        }
    }
}
