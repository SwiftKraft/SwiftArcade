namespace SwiftArcadeMode.Utils.Structures
{
    using System;
    using System.Collections.Generic;
    using SwiftArcadeMode.Utils.Interfaces;
    using UnityEngine;

    [Serializable]
    public class ModifiableStatistic : IOverrideParent
    {
        private float cache;

        public ModifiableStatistic()
        {
        }

        public ModifiableStatistic(float baseValue)
        {
            BaseValue = baseValue;
        }

        public event Action<float>? OnUpdate;

        public enum ModifierType
        {
            Addition,
            Subtraction,
            Multiplication,
            Division,
            Mutation,
        }

        [field: SerializeField]
        public float BaseValue { get; private set; } = 1f;

        [field: SerializeField]
        public List<Modifier> Values { get; private set; } = [];

        public List<ModifiableStatistic> Additives { get; } = [];

        public bool IsDirty { get; set; } = true;

        public static implicit operator float(ModifiableStatistic stat) => stat.GetValue();

        public virtual void UpdateValue()
        {
            IsDirty = true;
            OnUpdate?.Invoke(GetValue());
        }

        public float GetValue()
        {
            if (!IsDirty)
                return cache;

            float value = BaseValue;

            if (Values.Count > 0)
            {
                for (int i = 0; i < Values.Count; i++)
                    value = Values[i].Modify(value);
            }

            if (Additives.Count > 0)
            {
                for (int i = 0; i < Additives.Count; i++)
                {
                    for (int j = 0; j < Additives[i].Values.Count; j++)
                        value = Additives[i].Values[j].Modify(value);
                }
            }

            IsDirty = false;
            cache = value;
            return value;
        }

        public Modifier AddModifier()
        {
            Modifier modifier = new(this);
            Values.Add(modifier);
            IsDirty = true;
            return modifier;
        }

        public void RemoveOverride(object target)
        {
            Values.Remove((Modifier)target);
            IsDirty = true;
        }

        public class Modifier(ModifiableStatistic parent) : OverrideBase<ModifiableStatistic>(parent)
        {
            public float Value
            {
                get;
                set
                {
                    field = value;
                    Parent.UpdateValue();
                }
            }

            public ModifierType Type { get; set; }

            public float Modify(float value) => Type switch
            {
                ModifierType.Addition => value + Value,
                ModifierType.Subtraction => value - Value,
                ModifierType.Division => value / Value,
                ModifierType.Multiplication => value * Value,
                ModifierType.Mutation => Value,
                _ => value,
            };
        }
    }
}
