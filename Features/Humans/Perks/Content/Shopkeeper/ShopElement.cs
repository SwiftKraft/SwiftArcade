namespace SwiftArcadeMode.Features.Humans.Perks.Content.Shopkeeper
{
    using LabApi.Features.Wrappers;

    public abstract class ShopElement
    {
        public Shopkeeper Parent { get; protected set; } = null!;

        public virtual void Init(Shopkeeper parent)
        {
            Parent = parent;
        }

        public virtual void Restock()
        {
        }

        public virtual void Remove()
        {
        }
    }
}
