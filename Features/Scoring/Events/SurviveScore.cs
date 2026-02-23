namespace SwiftArcadeMode.Features.Scoring.Events
{
    using System.Collections.Generic;
    using LabApi.Features.Wrappers;
    using UnityEngine;

    public class SurviveScore : ScoreEventBase
    {
        public readonly Dictionary<Player, float> AliveTime = [];

        public static float Timer = 60f;
        public static int Score = 15;

        public override void Disable()
        {
        }

        public override void Enable()
        {
        }

        public override void Tick()
        {
            foreach (Player p in Player.List)
            {
                if (!p.IsAlive)
                {
                    if (AliveTime.ContainsKey(p))
                        AliveTime[p] = 0f;
                    continue;
                }

                float prev = 0f;

                if (AliveTime.ContainsKey(p))
                {
                    prev = AliveTime[p];
                    AliveTime[p] += Time.fixedDeltaTime;
                }
                else
                    AliveTime.Add(p, Time.fixedDeltaTime);

                if (prev < Timer && Timer <= AliveTime[p])
                {
                    AliveTime[p] = 0f;
                    p.AddScore(Score);
                }
            }
        }
    }
}
