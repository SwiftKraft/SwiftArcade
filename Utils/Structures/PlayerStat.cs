namespace SwiftArcadeMode.Utils.Structures
{
    using System;

    public class PlayerStat(Action<float>? setValue = null) : ModifiableStatistic
    {
        public Action<float>? SetValue { get; } = setValue;

        public override void UpdateValue()
        {
            base.UpdateValue();
            SetValue?.Invoke(GetValue());
        }
    }
}
