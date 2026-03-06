namespace SwiftArcadeMode.Features.Scoring.Events
{
    using System.Collections.Generic;
    using LabApi.Features.Wrappers;
    using UnityEngine;

    public class SurviveScore : ScoreEventBase
    {
        public static float Timer { get; set; } = 60f;

        public static int Score { get; set; } = 15;

        public Dictionary<Player, float> AliveTime { get; } = [];

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
