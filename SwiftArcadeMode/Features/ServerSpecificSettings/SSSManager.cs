namespace SwiftArcadeMode.Features.ServerSpecificSettings
{
    using System;
    using System.Linq;
    using Hints;
    using LabApi.Events.Arguments.PlayerEvents;
    using LabApi.Events.Handlers;
    using LabApi.Features.Wrappers;
    using SwiftArcadeMode.Commands.Client;
    using UnityEngine;
    using UserSettings.ServerSpecific;
    using Logger = LabApi.Features.Console.Logger;

    public static class SSSManager
    {
        public static SSKeybindSetting KeybindSeePerks { get; } = new(20070923, "See Perks", KeyCode.LeftBracket, true, true, "Shows you your perks.");

        public static SSKeybindSetting KeybindSeeUpgrades { get; } = new(20070924, "See Upgrades", KeyCode.RightBracket, true, false, "Shows you your available upgrades.");

        public static SSKeybindSetting KeybindChooseUpgrade1 { get; } = new(20070925, "Choose Upgrade 1", KeyCode.Comma, true, false, "Chooses upgrade 1.");

        public static SSKeybindSetting KeybindChooseUpgrade2 { get; } = new(20070926, "Choose Upgrade 2", KeyCode.Period, true, false, "Chooses upgrade 2.");

        public static SSKeybindSetting KeybindChooseUpgrade3 { get; } = new(20070927, "Choose Upgrade 3", KeyCode.Slash, true, false, "Chooses upgrade 3.");

        public static void Enable()
        {
            AppendSettings();
            ServerSpecificSettingsSync.ServerOnSettingValueReceived += OnPressKeybind;
            PlayerEvents.Joined += OnJoined;
        }

        public static void Disable()
        {
            ServerSpecificSettingsSync.DefinedSettings = ServerSpecificSettingsSync.DefinedSettings.Where(setting =>
                setting.SettingId is not (20070922 or 20070923 or 20070924 or 20070925 or 20070926 or 20070927))
                .ToArray();

            ServerSpecificSettingsSync.ServerOnSettingValueReceived -= OnPressKeybind;
            PlayerEvents.Joined -= OnJoined;
        }

        private static void AppendSettings()
        {
            ServerSpecificSettingsSync.DefinedSettings = [.. ServerSpecificSettingsSync.DefinedSettings ?? [], new SSGroupHeader(20070922, "SwiftKraft's Arcade Mode"), KeybindSeePerks, KeybindSeeUpgrades, KeybindChooseUpgrade1, KeybindChooseUpgrade2, KeybindChooseUpgrade3];
            ServerSpecificSettingsSync.SendToAll();
        }

        private static void OnJoined(PlayerJoinedEventArgs ev)
        {
            ServerSpecificSettingsSync.SendToPlayer(ev.Player.ReferenceHub);
        }

        private static void OnPressKeybind(ReferenceHub hub, ServerSpecificSettingBase setting)
        {
            try
            {
                Player p = Player.Get(hub);

                PerkInventory inv = p.GetPerkInventory();

                void Select(int ind)
                {
                    if (inv.UpgradeQueue.Choose(ind, out string name))
                        p.SendHint("<align=\"left\">Chosen: " + name + "</align>", [HintEffectPresets.FadeOut()], 10f);
                    else
                        p.SendHint("<align=\"left\">Failed to choose upgrade.</align>", [HintEffectPresets.FadeOut()], 2f);
                }

                switch (setting)
                {
                    case SSKeybindSetting { SettingId: 20070923, SyncIsPressed: true }: // see perks
                        ClientSeePerksCommand.GetPerks(p, out string brief1);
                        p.SendHint(brief1, [HintEffectPresets.FadeOut()], 5f);
                        break;
                    case SSKeybindSetting { SettingId: 20070924, SyncIsPressed: true }: // see upgrades
                        inv.UpgradeQueue.Peek(out string n);
                        p.SendHint("<align=\"left\">" + n + "</align>", [HintEffectPresets.FadeOut()], 10f);
                        break;
                    case SSKeybindSetting { SettingId: 20070925, SyncIsPressed: true }: // choose 1
                        Select(0);
                        break;
                    case SSKeybindSetting { SettingId: 20070926, SyncIsPressed: true }: // choose 2
                        Select(1);
                        break;
                    case SSKeybindSetting { SettingId: 20070927, SyncIsPressed: true }: // choose 3
                        Select(2);
                        break;
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }
    }
}
