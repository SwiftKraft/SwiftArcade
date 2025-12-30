using LabApi.Events.Handlers;
using LabApi.Features.Wrappers;
using MEC;
using SwiftArcadeMode.Utils.Projectiles;
using SwiftArcadeMode.Utils.Structures;
using System;
using System.Linq;
using UnityEngine;

namespace SwiftArcadeMode.Features.Humans.Perks.Content.Caster
{
    public abstract class CasterBase(PerkInventory inv) : PerkItemReceiveBase(inv)
    {
        public abstract Type[] ListSpells();

        public override ItemType ItemType => ItemType.KeycardCustomTaskForce;
        public override string PerkDescription => $"Allows you to cast {Name} spells.\nDrop the keycard to change spell, inspect to cast.";

        public virtual float KillCooldownReduction => 4f;
        public virtual float LessItemsCooldown => 3f;
        public abstract float RegularCooldown { get; }
        public override float Cooldown => Player == null || Player.Items.Count() < 3 ? LessItemsCooldown : RegularCooldown;
        public override int Limit => int.MaxValue;

        public override string ReadyMessage => Player.IsInventoryFull ? "Failed to refresh, no space in inventory." : "Spells refreshed!";

        public SpellBase CurrentSpell { get; private set; }
        public SpellBase[] Spells { get; private set; }
        public Item CurrentSpellItem { get; private set; }
        public ushort CurrentSpellItemSerial { get; private set; }
        public int CurrentSpellIndex
        {
            get => currentSpellIndex;
            set
            {
                if (value < 0 || Spells.Length <= 0)
                    return;

                currentSpellIndex = value % Spells.Length;
                CurrentSpell = Spells[currentSpellIndex];
                CurrentSpell?.Init(this);
            }
        }
        int currentSpellIndex;

        bool casting;
        readonly Timer castDuration = new();

        public override void Init()
        {
            base.Init();

            PlayerEvents.DroppingItem += OnDroppingItem;
            PlayerEvents.DroppedItem += OnDroppedItem;
            PlayerEvents.InspectingKeycard += OnInspectingKeycard;
            PlayerEvents.Dying += OnDying;
            PlayerEvents.ChangingItem += OnChangingItem;

            castDuration.OnTimerEnd += OnCastTimerEnded;

            Spells = [.. ListSpells().Select(t => (SpellBase)Activator.CreateInstance(t)).Where(s => s != null)];

            if (Player != null && Spells.Length <= 0)
                Player.GetPerkInventory().RemovePerk(this);

            CurrentSpellIndex = 0;
        }

        public override void Remove()
        {
            base.Remove();
            PlayerEvents.DroppingItem -= OnDroppingItem;
            PlayerEvents.DroppedItem -= OnDroppedItem;
            PlayerEvents.InspectingKeycard -= OnInspectingKeycard;
            PlayerEvents.Dying -= OnDying;
            PlayerEvents.ChangingItem -= OnChangingItem;

            castDuration.OnTimerEnd -= OnCastTimerEnded;

            RemoveCurrentSpellItem();
        }

        public override void Tick()
        {
            if (CurrentSpellItem == null || !Player.Items.Any(i => i.Serial == CurrentSpellItemSerial))
                base.Tick();

            if (!castDuration.Ended)
            {
                castDuration.Tick(Time.fixedDeltaTime);
                CurrentSpell?.Tick();
            }
        }

        private void OnCastTimerEnded() => CurrentSpell?.End();

        private void OnDroppedItem(LabApi.Events.Arguments.PlayerEvents.PlayerDroppedItemEventArgs ev)
        {
            if (ev.Pickup.Serial != CurrentSpellItemSerial)
                return;

            ev.Pickup.Destroy();
        }

        private void OnChangingItem(LabApi.Events.Arguments.PlayerEvents.PlayerChangingItemEventArgs ev)
        {
            if (ev.Player == Player && ev.NewItem == CurrentSpellItem)
                SendMessage("Equipped " + CurrentSpell.Name);

            if (casting && ev.Player == Player && ev.OldItem == CurrentSpellItem)
                ev.IsAllowed = false;
        }

        private void OnInspectingKeycard(LabApi.Events.Arguments.PlayerEvents.PlayerInspectingKeycardEventArgs ev)
        {
            if (casting || !castDuration.Ended || ev.Player != Player || ev.KeycardItem != CurrentSpellItem)
                return;

            SendMessage("Casting " + CurrentSpell.Name, CurrentSpell.CastTime);
            casting = true;
            Timing.CallDelayed(CurrentSpell.CastTime, () =>
            {
                casting = false;
                if (!Player.IsAlive)
                    return;

                CurrentSpell.Cast();

                if (CurrentSpell.CastDuration > 0f)
                    castDuration.Reset(CurrentSpell.CastDuration);

                RemoveCurrentSpellItem();
                SendMessage("Casted " + CurrentSpell.Name, 1f);
            });
        }

        private void OnDying(LabApi.Events.Arguments.PlayerEvents.PlayerDyingEventArgs ev)
        {
            if (ev.Player != Player)
            {
                if (ev.Attacker == Player)
                    CooldownTimer.Tick(KillCooldownReduction);

                return;
            }

            casting = false;
            RemoveCurrentSpellItem();
        }

        private void RemoveCurrentSpellItem()
        {
            if (CurrentSpellItem != null)
            {
                Player.RemoveItem(CurrentSpellItem);
                CurrentSpellItem = null;
                CurrentSpellItemSerial = 0;
            }
        }

        private void OnDroppingItem(LabApi.Events.Arguments.PlayerEvents.PlayerDroppingItemEventArgs ev)
        {
            if (ev.Player != Player || ev.Item != CurrentSpellItem)
                return;

            ev.IsAllowed = false;

            if (castDuration.Ended && !casting)
            {
                CurrentSpellIndex++;
                bool held = ev.Player.CurrentItem == CurrentSpellItem;

                Item it = GiveItem();

                if (held)
                    ev.Player.CurrentItem = it;
            }
        }

        public override Item GiveItem()
        {
            RemoveCurrentSpellItem();

            CurrentSpellItem = KeycardItem.CreateCustomKeycardTaskForce(Player, CurrentSpell.Name, CurrentSpell.Name, default, CurrentSpell.BaseColor, CurrentSpell.BaseColor, default, CurrentSpell.RankIndex);
            if (CurrentSpellItem != null)
                CurrentSpellItemSerial = CurrentSpellItem.Serial;

            return CurrentSpellItem;
        }

        public abstract class MagicProjectileBase(SpellBase spell, Vector3 initialPosition, Quaternion initialRotation, Vector3 initialVelocity, float lifetime = 10, Player owner = null) : ProjectileBase(initialPosition, initialRotation, initialVelocity, lifetime, owner)
        {
            public readonly SpellBase Spell = spell;

            public abstract bool UseGravity { get; }

            public override void Init()
            {
                base.Init();
                Rigidbody.useGravity = UseGravity;
            }
        }
    }
}
