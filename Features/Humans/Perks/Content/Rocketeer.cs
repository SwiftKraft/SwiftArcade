namespace SwiftArcadeMode.Features.Humans.Perks.Content
{
    using LabApi.Events.Arguments.PlayerEvents;
    using LabApi.Events.Handlers;
    using LabApi.Features.Wrappers;
    using Mirror;
    using UnityEngine;

    [Perk("Rocketeer", Rarity.Rare)]
    public class Rocketeer(PerkInventory inv) : PerkBase(inv)
    {
        public static readonly LayerMask CollisionLayers = LayerMask.GetMask("Default", "Door", "Glass");

        public override string Name => "Rocketeer";

        public override string Description => "Throwing a grenade will turn it into a rocket projectile. \nRockets have reduced SCP damage.";

        public override void Init()
        {
            base.Init();
            PlayerEvents.ThrewProjectile += OnThrewProjectile;
        }

        private void OnThrewProjectile(PlayerThrewProjectileEventArgs ev)
        {
            if (ev.Player != Player || ev.Projectile is not ExplosiveGrenadeProjectile proj)
                return;

            ConvertRocket(Player, proj, 10f);
        }

        public override void Remove()
        {
            base.Remove();
            PlayerEvents.ThrewProjectile -= OnThrewProjectile;
        }

        public static void ConvertRocket(Player player, ExplosiveGrenadeProjectile proj, float speed)
        {
            float time = 5f;
            if (Physics.Raycast(player.Camera.position, player.Camera.forward, out RaycastHit hit, Mathf.Infinity, CollisionLayers, QueryTriggerInteraction.Ignore))
                time = hit.distance / speed;
            proj.Rigidbody.position = player.Camera.position;
            proj.Rigidbody.rotation = player.Camera.rotation;
            proj.Rigidbody.useGravity = false;
            proj.Rigidbody.linearVelocity = player.Camera.forward * speed;
            proj.Rigidbody.angularVelocity = default;
            proj.Rigidbody.mass = 99999f;
            proj.Base.TargetTime = NetworkTime.time + time - 0.05f;
            proj.Base.ScpDamageMultiplier = 1.5f;
        }
    }
}
