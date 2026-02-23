namespace SwiftArcadeMode.Features.Humans.Perks.Content.Caster
{
    using System.Collections.Generic;
    using SwiftArcadeMode.Utils.Deployable;
    using UnityEngine;

    public abstract class SummonSpell : SpellBase
    {
        public virtual int Limit => 1;

        protected List<DeployableBase> SpawnedDeployables { get; } = [];

        public override void Cast()
        {
            if (Physics.Raycast(Caster.Player.Camera.position, Vector3.down, out RaycastHit hit, 4f, LayerMask.GetMask("Default"), QueryTriggerInteraction.Ignore))
                Spawn(hit.point);
        }

        public virtual void Spawn(Vector3 point)
        {
            if (Limit <= 0)
                return;

            Vector3 loc = point + Vector3.up;

            SpawnedDeployables.RemoveAll(d => d.Destroyed);

            if (SpawnedDeployables.Count < Limit)
                SpawnedDeployables.Add(Create(loc));
            else
            {
                DeployableBase dep = SpawnedDeployables[0];
                dep.Position = loc;
                if (Limit > 1)
                {
                    SpawnedDeployables.Remove(dep);
                    SpawnedDeployables.Add(dep);
                }
            }
        }

        public abstract DeployableBase Create(Vector3 loc);
    }
}
