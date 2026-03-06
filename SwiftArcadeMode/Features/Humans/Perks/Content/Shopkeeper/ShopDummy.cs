namespace SwiftArcadeMode.Features.Humans.Perks.Content.Shopkeeper
{
    using LabApi.Features.Wrappers;
    using NetworkManagerUtils.Dummies;
    using UnityEngine;

    public class ShopDummy(Vector3 offset) : ShopElementOffset(offset)
    {
        public Player? Dummy { get; private set; }

        public override void Init(Shopkeeper parent)
        {
            base.Init(parent);
            Restock();
        }

        public override void Restock()
        {
            base.Restock();

            Dummy ??= Player.Get(DummyUtils.SpawnDummy());

            if (Dummy.IsAlive)
                return;

            Dummy.SetRole(Parent.Player.Role, PlayerRoles.RoleChangeReason.RemoteAdmin, PlayerRoles.RoleSpawnFlags.None);
            Dummy.Position = Position;
            Dummy.Rotation = Rotation;
        }
    }
}
