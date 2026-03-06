namespace SwiftArcadeMode.Features.Humans.Perks.Content.Caster
{
    using Footprinting;
    using LabApi.Events.Handlers;
    using LabApi.Features.Wrappers;
    using SwiftArcadeMode.Utils.Deployable;
    using UnityEngine;

    public abstract class Summon(SpellBase spell, string name, string schematicName, Vector3 position, Quaternion rotation)
        : DeployableBase(name, schematicName, position, rotation)
    {
        public Player Owner { get; } = spell.Caster.Player;

        public Footprint Footprint { get; } = new(spell.Caster.Player.ReferenceHub);

        public SpellBase Spell { get; } = spell;

        public abstract float DestroyRange { get; }

        public virtual bool DieOnOwnerDeath => true;

        public override void Initialize()
        {
            base.Initialize();

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
