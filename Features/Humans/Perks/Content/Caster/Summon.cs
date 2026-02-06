using LabApi.Events.Handlers;
using LabApi.Features.Wrappers;
using PlayerRoles;
using SwiftArcadeMode.Utils.Deployable;
using UnityEngine;

namespace SwiftArcadeMode.Features.Humans.Perks.Content.Caster
{
    public abstract class Summon(SpellBase spell, string name, string schematicName, RoleTypeId role, Vector3 colliderScale, Vector3 position, Quaternion rotation) : DeployableBase(name, schematicName, role, colliderScale, position, rotation)
    {
        public Player Owner { get; set; }
        public SpellBase Spell { get; private set; } = spell;

        public abstract float DestroyRange { get; }
        public abstract float Health { get; }

        public virtual bool DieOnOwnerDeath => true;

        public override void Initialize()
        {
            base.Initialize();
            Dummy.MaxHealth = Health;
            Dummy.Health = Health;

            if (DieOnOwnerDeath)
                PlayerEvents.Death += OnOwnerDied;
        }

        public override void Tick()
        {
            base.Tick();

            if ((Owner.Position - Position).sqrMagnitude > DestroyRange * DestroyRange)
            {
                Destroy();
                Spell.Caster.SendMessage($"{Name} has been destroyed! \nYou are too far away to support its existence.");
                return;
            }
        }

        protected virtual void OnOwnerDied(LabApi.Events.Arguments.PlayerEvents.PlayerDeathEventArgs ev)
        {
            if (ev.Player != Owner)
                return;
            PlayerEvents.Death -= OnOwnerDied;
            Destroy();
        }
    }
}
