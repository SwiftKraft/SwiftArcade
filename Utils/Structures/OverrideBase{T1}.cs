namespace SwiftArcadeMode.Utils.Structures
{
    using SwiftArcadeMode.Utils.Interfaces;

    public abstract class OverrideBase<T1>(T1 parent) : OverrideBase
        where T1 : IOverrideParent
    {
        public T1 Parent { get; } = parent;

        public override void Dispose() => Parent.RemoveOverride(this);
    }
}
