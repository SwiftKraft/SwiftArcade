namespace SwiftArcadeMode.Utils.Effects
{
    using System.Collections.Generic;
    using System.Linq;
    using LabApi.Features.Wrappers;
    using UnityEngine;

    public class CustomEffectContainer(Player p)
    {
        public CustomEffectContainer(ReferenceHub refHub)
            : this(Player.Get(refHub))
        {
        }

        public Player Player { get; } = p;

        public List<CustomEffectBase> ActiveEffects { get; } = [];

        public void Tick()
        {
            for (int i = ActiveEffects.Count - 1; i >= 0; i--)
            {
                ActiveEffects[i].Tick();
                ActiveEffects[i].EffectTimer.Tick(Time.fixedDeltaTime);

                if (ActiveEffects[i].EffectTimer.Ended)
                    RemoveEffect(i);
            }
        }

        public void ClearEffects()
        {
            for (int i = ActiveEffects.Count - 1; i >= 0; i--)
                RemoveEffect(i);
        }

        public void RemoveEffect(int i)
        {
            ActiveEffects[i].Remove();
            ActiveEffects.RemoveAt(i);
        }

        public void RemoveEffect(CustomEffectBase eff)
        {
            if (!ActiveEffects.Contains(eff))
                return;

            eff.Remove();
            ActiveEffects.Remove(eff);
        }

        public void AddEffect(CustomEffectBase effect)
        {
            effect.Init(this);

            bool canAdd = true;

            List<CustomEffectBase> existing = [.. ActiveEffects.Where(c => c.GetType() == effect.GetType())];

            if (existing.Count > 0)
            {
                if (effect.StackCount >= existing.Count)
                    RemoveEffect(existing[0]);
                else
                    canAdd = effect.Stack(existing[0]);
            }

            if (canAdd)
            {
                ActiveEffects.Add(effect);
                effect.Add();
            }
        }
    }
}
