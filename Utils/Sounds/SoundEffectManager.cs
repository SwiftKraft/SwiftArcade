using LabApi.Features.Wrappers;
using System.IO;
using UnityEngine;
using Logger = LabApi.Features.Console.Logger;

namespace SwiftArcadeMode.Utils.Sounds
{
    public static class SoundEffectManager
    {
        public static bool DebugLogs { get; set; } = false;
        public static bool Disabled { get; private set; } = false;
        public static string BasePath;

        public static void PreloadClip(string id, params string[] folders)
        {
            if (Disabled || typeof(AudioClipStorage) == null)
            {
                Disable();
                return;
            }

            if (AudioClipStorage.AudioClips.ContainsKey(id))
                return;

            string path = Path.Combine(BasePath, Path.Combine(folders));
            bool success = AudioClipStorage.LoadClip(path, id);

            if (!DebugLogs)
                return;

            if (success)
                Logger.Info("Successfully loaded sound clip at \"" + path + "\", ID: " + id);
            else
                Logger.Error("Failed to load sound clip at \"" + path + "\", ID: " + id);
        }

        public static void PlaySound(Player player, string id, string name = "speaker", float volume = 1f, bool loop = false, bool destroyOnEnd = true, float minDist = 5f, float maxDist = 15f)
        {
            if (Disabled || typeof(AudioPlayer) == null)
            {
                Disable();
                return;
            }

            AudioPlayer audioPlayer = AudioPlayer.CreateOrGet($"Player {player.Nickname}", onIntialCreation: (p) =>
            {
                p.transform.parent = player.GameObject.transform;
                Speaker speaker = p.AddSpeaker(name, isSpatial: true, minDistance: minDist, maxDistance: maxDist);

                speaker.transform.parent = player.GameObject.transform;
                speaker.transform.localPosition = Vector3.zero;
            });

            audioPlayer.AddClip(id, volume, loop, destroyOnEnd);
        }

        public static void PlaySound(Vector3 position, string id, string name = "speaker", float volume = 1f, bool loop = false, float minDist = 5f, float maxDist = 15f)
        {
            if (Disabled || typeof(AudioPlayer) == null)
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
