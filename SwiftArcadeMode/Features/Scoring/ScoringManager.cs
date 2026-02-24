namespace SwiftArcadeMode.Features.Scoring
{
    using System;
    using System.Collections.Generic;
    using LabApi.Features.Wrappers;
    using SwiftArcadeMode.Utils.Extensions;

    public static class ScoringManager
    {
        /// <summary>
        /// Gets a dictionary containing all player scores. The key represents User ID (Steam ID) and the value represents score number.
        /// </summary>
        public static Dictionary<string, int> Scores { get; } = [];

        public static Dictionary<string, string> IDToName { get; } = [];

        public static HashSet<ScoreEventBase> Events { get; } = [];

        public static HashSet<Type> ScoreEventsCache => field ??= ReflectionExtensions.GetAllNonAbstractSubclasses<ScoreEventBase>();

        public static void Enable()
        {
            foreach (Type t in ScoreEventsCache)
            {
                ScoreEventBase b = (ScoreEventBase)Activator.CreateInstance(t);
                Events.Add(b);
                b.Enable();
            }
        }

        public static void Tick()
        {
            foreach (ScoreEventBase b in Events)
                b.Tick();
        }

        public static void Disable()
        {
            foreach (ScoreEventBase b in Events)
                b.Disable();
        }

        public static void AddScore(this Player p, int amount)
        {
            if (p.IsDummy || string.IsNullOrWhiteSpace(p.UserId) || p.DoNotTrack)
                return;

            if (!IDToName.ContainsKey(p.UserId))
                IDToName.Add(p.UserId, p.Nickname);
            else if (p.Nickname != IDToName[p.UserId])
                IDToName[p.UserId] = p.Nickname;

            if (!Scores.TryAdd(p.UserId, amount))
                Scores[p.UserId] += amount;
        }

        public static int GetScore(this Player p) => Scores.ContainsKey(p.UserId) ? Scores[p.UserId] : 0;
    }
}
