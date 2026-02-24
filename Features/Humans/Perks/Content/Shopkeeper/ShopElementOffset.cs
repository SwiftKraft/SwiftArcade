namespace SwiftArcadeMode.Features.Humans.Perks.Content.Shopkeeper
{
    using LabApi.Features.Wrappers;
    using UnityEngine;

    public abstract class ShopElementOffset(Vector3 offset) : ShopElement
    {
        public Vector3 Offset { get; } = offset;

        public virtual Vector3 Position { get; private set; }

        public virtual Quaternion Rotation { get; private set; }

        public void UpdatePosition(Room shop)
        {
            Position = shop.Position + (shop.Rotation * Offset);
            Rotation = shop.Rotation;
        }
    }
}
