namespace SwiftArcadeMode.Utils.Sounds
{
    using System.Collections.Generic;
    using System.IO;
    using LabApi.Features.Wrappers;
    using UnityEngine;
    using Logger = LabApi.Features.Console.Logger;

    public static class SoundEffectManager
    {
        public static HashSet<string> PreviousIds { get; } = [];

        public static bool DebugLogs { get; set; }

        public static bool Disabled { get; private set; }

        public static string BasePath { get; internal set; } = null!;

        public static void PreloadClip(string id, params string[] folders)
        {
            if (Disabled)
            {
                Disable();
                return;
            }

            if (AudioClipStorage.AudioClips.ContainsKey(id))
                return;

            string path = Path.Combine(BasePath, Path.Combine(folders));
            if (File.Exists(path))
            {
                AudioClipStorage.LoadClip(path, id);
            }
            else if (DebugLogs && !PreviousIds.Contains(id))
            {
                Logger.Error("Failed to load sound clip at \"" + path + "\", ID: " + id);
                PreviousIds.Add(id);
            }

            if (!DebugLogs)
                return;

            Logger.Info("Successfully loaded sound clip at \"" + path + "\", ID: " + id);
        }

        public static void PlaySound(Player player, string id, string name = "speaker", float volume = 1f, bool loop = false, bool destroyOnEnd = true, float minDist = 5f, float maxDist = 15f)
        {
            if (Disabled)
            {
                Disable();
                return;
            }

            AudioPlayer audioPlayer = AudioPlayer.CreateOrGet($"Player {player.Nickname}", onIntialCreation: (p) =>
            {
                p.transform.parent = player.GameObject?.transform;
                Speaker speaker = p.AddSpeaker(name, isSpatial: true, minDistance: minDist, maxDistance: maxDist);

                speaker.transform.parent = player.GameObject?.transform;
                speaker.transform.localPosition = Vector3.zero;
            });

            audioPlayer.AddClip(id, volume, loop, destroyOnEnd);
        }

        public static void PlaySound(Vector3 position, string id, string name = "speaker", float volume = 1f, bool loop = false, float minDist = 5f, float maxDist = 15f)
        {
            if (Disabled)
            {
                Disable();
                return;
            }

            AudioPlayer audioPlayer = AudioPlayer.CreateOrGet(position.ToString(), onIntialCreation: (p) =>
            {
                Speaker speaker = p.AddSpeaker(name, isSpatial: true, minDistance: minDist, maxDistance: maxDist);
                speaker.transform.position = position;
            });

            audioPlayer.AddClip(id, volume, loop, true);
        }

        public static void Disable(string error = "Failed to find SCPSL Audio API, perks will not play custom sounds.")
        {
            if (!Disabled)
            {
                Disabled = true;
                Logger.Error(error);
            }
        }
    }
}
