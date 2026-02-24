namespace SwiftArcadeMode.Features.Humans.Perks.Content
{
    using LabApi.Events.Handlers;
    using LabApi.Features.Wrappers;

    [Perk("MicroMissiles", Rarity.Secret)]
    public class MicroMissiles(PerkInventory inv) : PerkBase(inv)
    {
        private int counter;

        public override string Name => "Micro Missiles";

        public override string Description => $"Every {Amount} shots creates an explosive projectile.";

        public virtual int Amount => 10;

        public override void Init()
        {
            base.Init();
            PlayerEvents.ShootingWeapon += OnShootingWeapon;
        }

        public override void Remove()
        {
            base.Remove();
            PlayerEvents.ShootingWeapon -= OnShootingWeapon;
        }

        private void OnShootingWeapon(LabApi.Events.Arguments.PlayerEvents.PlayerShootingWeaponEventArgs ev)
        {
            if (ev.Player != Player)
                return;

            counter++;

            if (counter < Amount)
                return;

            counter = 0;

            ExplosiveGrenadeProjectile? projectile = TimedGrenadeProjectile.SpawnActive(Player.Camera.position, ItemType.GrenadeHE, Player, 3d) as ExplosiveGrenadeProjectile;
            if (projectile is null)
                return;

            projectile.Base.MaxRadius = 2f;

            Rocketeer.ConvertRocket(Player, projectile, 45f);
            projectile.Base.ScpDamageMultiplier = 1f;
        }
    }
}
