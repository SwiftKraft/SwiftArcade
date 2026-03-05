namespace SwiftArcadeMode.Utils.Deployable
{
    using System.Collections.Generic;
    using AdminToys;
    using PlayerStatsSystem;
    using UnityEngine;

    /*
     * MISSING DAMAGE SOURCES
     * 173 / 106 / 049
     */
    public class Hitbox : MonoBehaviour, IDestructible
    {
        public static HashSet<Hitbox> Hitboxes { get; } = [];

        public DeployableBase Parent { get; private set; } = null!;

        public PrimitiveObjectToy Toy { get; private set; } = null!;

        public float DamageMultiplier { get; private set; }

        public uint NetworkId { get; private set; }

        public Vector3 CenterOfMass => Toy.transform.position;

        public static Hitbox Create(DeployableBase parent, PrimitiveObjectToy toy, float damageMultiplier)
        {
            // good for debugging damage from new sources
            // toy.NetworkPrimitiveFlags |= PrimitiveFlags.Collidable;
            toy.gameObject.layer = LayerMask.NameToLayer("Hitbox");

            Hitbox hitbox = toy.gameObject.AddComponent<Hitbox>();
            hitbox.Parent = parent;
            hitbox.Toy = toy;
            hitbox.DamageMultiplier = damageMultiplier;

            // all hitboxes share the same parent net id
            hitbox.NetworkId = parent.Schematic.GetComponent<AdminToyBase>().netId;

            return hitbox;
        }

        public static void ToggleAll(bool state)
        {
            foreach (Hitbox hitbox in Hitboxes)
            {
                hitbox.Toy._collider?.enabled = state;
            }
        }

        public bool Damage(float damage, DamageHandlerBase handler, Vector3 exactHitPos)
        {
            Parent.OnHit(damage * DamageMultiplier, handler, exactHitPos);

            return true;
        }

        public void Awake() => Hitboxes.Add(this);

        public void OnDestroy() => Hitboxes.Remove(this);
    }
}