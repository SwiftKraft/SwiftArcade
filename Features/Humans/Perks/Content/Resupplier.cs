namespace SwiftArcadeMode.Features.Humans.Perks.Content
{
    using InventorySystem.Items.Firearms.Modules;
    using LabApi.Events.Arguments.PlayerEvents;
    using LabApi.Events.Handlers;
    using LabApi.Features.Wrappers;

    [Perk("Resupplier", Rarity.Uncommon)]
    public class Resupplier(PerkInventory inv) : PerkBase(inv)
    {
        public override string Name => "Resupplier";

        public override string Description => "Reloading no longer requires ammo.";

        public override void Init()
        {
            base.Init();

            Player.SetAmmo(ItemType.Ammo12gauge, 999);
            Player.SetAmmo(ItemType.Ammo9x19, 999);
            Player.SetAmmo(ItemType.Ammo44cal, 999);
            Player.SetAmmo(ItemType.Ammo556x45, 999);
            Player.SetAmmo(ItemType.Ammo762x39, 999);

            PlayerEvents.ReloadingWeapon += OnReloadingWeapon;
            PlayerEvents.AimedWeapon += OnAimedWeapon;
            PlayerEvents.DroppingAmmo += OnDroppingAmmo;
            PlayerEvents.Cuffing += OnCuffing;
            PlayerEvents.Uncuffing += OnUncuffing;
            PlayerEvents.Dying += OnDying;
        }

        private void OnDying(PlayerDyingEventArgs ev)
        {
            if (!ev.IsAllowed || ev.Player != Player)
                return;

            Player.SetAmmo(ItemType.Ammo12gauge, 0);
            Player.SetAmmo(ItemType.Ammo9x19, 0);
            Player.SetAmmo(ItemType.Ammo44cal, 0);
            Player.SetAmmo(ItemType.Ammo556x45, 0);
            Player.SetAmmo(ItemType.Ammo762x39, 0);
        }

        private void OnUncuffing(PlayerUncuffingEventArgs ev)
        {
            if (!ev.IsAllowed || ev.Target != Player)
                return;

            Player.SetAmmo(ItemType.Ammo12gauge, 999);
            Player.SetAmmo(ItemType.Ammo9x19, 999);
            Player.SetAmmo(ItemType.Ammo44cal, 999);
            Player.SetAmmo(ItemType.Ammo556x45, 999);
            Player.SetAmmo(ItemType.Ammo762x39, 999);
        }

        private void OnCuffing(PlayerCuffingEventArgs ev)
        {
            if (!ev.IsAllowed || ev.Target != Player)
                return;

            Player.SetAmmo(ItemType.Ammo12gauge, 0);
            Player.SetAmmo(ItemType.Ammo9x19, 0);
            Player.SetAmmo(ItemType.Ammo44cal, 0);
            Player.SetAmmo(ItemType.Ammo556x45, 0);
            Player.SetAmmo(ItemType.Ammo762x39, 0);
        }

        public override void Remove()
        {
            base.Remove();

            PlayerEvents.ReloadingWeapon -= OnReloadingWeapon;
            PlayerEvents.AimedWeapon -= OnAimedWeapon;
            PlayerEvents.DroppingAmmo -= OnDroppingAmmo;
            PlayerEvents.Cuffing -= OnCuffing;
            PlayerEvents.Uncuffing -= OnUncuffing;
            PlayerEvents.Dying -= OnDying;
        }

        private void OnAimedWeapon(PlayerAimedWeaponEventArgs ev)
        {
            if (ev.Player != Player)
                return;

            Player.SetAmmo(ItemType.Ammo12gauge, 999);
            Player.SetAmmo(ItemType.Ammo9x19, 999);
            Player.SetAmmo(ItemType.Ammo44cal, 999);
            Player.SetAmmo(ItemType.Ammo556x45, 999);
            Player.SetAmmo(ItemType.Ammo762x39, 999);
        }

        private void OnDroppingAmmo(PlayerDroppingAmmoEventArgs ev)
        {
            if (ev.Player == Player)
                ev.IsAllowed = false;
        }

        protected void OnReloadingWeapon(PlayerReloadingWeaponEventArgs ev)
        {
            if (ev.Player != Player || Player.CurrentItem == null || Player.CurrentItem.Type == ItemType.GunSCP127 || Player.CurrentItem.Type == ItemType.ParticleDisruptor || Player.CurrentItem is not FirearmItem item || !item.Base.TryGetModule(out IPrimaryAmmoContainerModule mod) || mod.AmmoType == ItemType.None)
                return;

            Player.SetAmmo(mod.AmmoType, 999);
        }
    }
}
