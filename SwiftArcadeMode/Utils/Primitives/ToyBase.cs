namespace SwiftArcadeMode.Utils.Primitives
{
    using System;
    using LabApi.Features.Wrappers;

    public class ToyBase(AdminToy toy)
    {
        public AdminToy? Toy { get; private set; } = toy;

        public Action? TickAction { get; set; }

        public static implicit operator ToyBase(AdminToy toy) => new(toy);

        public virtual void Spawn()
        {
            if (Toy != null)
                return;

            Init();
            Toy?.Spawn();
        }

        /// <summary>
        /// Initializes this custom toy.
        /// </summary>
        /// <remarks>MUST set <see cref="Toy"/> to a non-null value.</remarks>
        public virtual void Init()
        {
        }

        public virtual void Tick() => TickAction?.Invoke();

        public virtual void Destroy() => Toy?.Destroy();
    }
}
