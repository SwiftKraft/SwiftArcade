namespace SwiftArcadeMode.Utils.Structures
{
    using System;
    using SwiftArcadeMode.Utils.Interfaces;
    using UnityEngine;

    public class Timer : IValue<float>, ITickable
    {
        public Timer()
            : this(0F)
        {
        }

        public Timer(float time, bool startEnded = true)
        {
            if (!startEnded)
                Reset(time);
            MaxValue = time;
            Ended = startEnded;
        }

        public event Action? OnTimerEnd;

        public float MaxValue { get; set; }

        public float PrevTickTime { get; private set; }

        public bool StartEnded { get; set; }

        public virtual float CurrentValue
        {
            get
            {
                if (field < 0f)
                    field = 0f;
                return field;
            }

            set
            {
                if (value > 0f)
                {
                    PrevTickTime = field;
                    field = value;
                }
                else
                    field = 0f;
            }
        }

        public bool Ended
        {
            get => CurrentValue == 0f;
            set
            {
                if (!value || Ended)
                    return;

                CurrentValue = 0f;
                OnTimerEnd?.Invoke();
            }
        }

        public virtual float Tick(float deltaTime)
        {
            if (Ended)
                return CurrentValue;

            CurrentValue -= deltaTime;

            if (CurrentValue == 0f)
                OnTimerEnd?.Invoke();

            return CurrentValue;
        }

        public bool IsPassingTime(float time) => Mathf.Approximately(CurrentValue, time) || (PrevTickTime >= time && CurrentValue <= time);

        public virtual void Reset(float time)
        {
            MaxValue = time;
            Reset();
        }

        public virtual void Reset() => CurrentValue = MaxValue;

        public float GetPercentage() => Mathf.InverseLerp(0F, MaxValue, CurrentValue);
    }
}
