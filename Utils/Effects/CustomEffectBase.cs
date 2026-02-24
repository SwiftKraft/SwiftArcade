namespace SwiftArcadeMode.Utils.Effects
{
    using SwiftArcadeMode.Utils.Structures;

    public abstract class CustomEffectBase(float duration)
    {
        public CustomEffectContainer Parent { get; private set; } = null!;

        public virtual int StackCount => int.MaxValue;

        public Timer EffectTimer { get; } = new(duration, false);

        public virtual void Init(CustomEffectContainer cont) => Parent = cont;

        public virtual bool Stack(CustomEffectBase existingEffect) => true;

        public abstract void Add();

        public abstract void Tick();

        public abstract void Remove();
    }
}
