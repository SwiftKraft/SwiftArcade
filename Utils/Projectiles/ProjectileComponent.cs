namespace SwiftArcadeMode.Utils.Projectiles
{
    using UnityEngine;

    public class ProjectileComponent : MonoBehaviour
    {
        public ProjectileBase? Projectile { get; set; }

        private void OnCollisionEnter(Collision cols) => Projectile?.OnCollide(cols);
    }
}
