namespace SwiftArcadeMode.Features.Humans.Perks.Content.Shopkeeper
{
    using UnityEngine;

    public abstract class ShopElementOffset(Vector3 offset) : ShopElement
    {
        public readonly Vector3 Offset = offset;

        public virtual Vector3 Position => Parent.Shop.Position + Parent.Shop.Rotation * Offset;

        public virtual Quaternion Rotation => Parent.Shop.Rotation;
    }
}
