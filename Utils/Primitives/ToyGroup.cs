namespace SwiftArcadeMode.Utils.Primitives
{
    using System.Collections.Generic;
    using System.Linq;

    public class ToyGroup(params ToyBase[] primitives)
    {
        public List<ToyBase> Toys { get; } = primitives.ToList();

        public virtual void Spawn()
        {
            for (int i = 0; i < Toys.Count; i++)
                Toys[i].Spawn();
        }

        public virtual void Tick()
        {
            for (int i = 0; i < Toys.Count; i++)
                Toys[i].Tick();
        }

        public virtual void Destroy()
        {
            for (int i = 0; i < Toys.Count; i++)
                Toys[i].Destroy();
        }
    }
}
