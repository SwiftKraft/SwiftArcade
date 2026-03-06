namespace SwiftArcadeMode.Utils.Projectiles
{
    using LabApi.Features.Wrappers;
    using UnityEngine;

    public class LightExplosion : MonoBehaviour
    {
        public LightSourceToy Toy { get; private set; } = null!;

        public float FadeSpeed { get; set; } = 20f;

        public static LightExplosion Create(LightSourceToy unspawned, float fadeSpeed = 20f)
        {
            unspawned.SyncInterval = 0;
            LightExplosion exp = unspawned.GameObject.AddComponent<LightExplosion>();
            exp.Toy = unspawned;
            exp.FadeSpeed = fadeSpeed;
            unspawned.Spawn();
            return exp;
        }

        public void FixedUpdate()
        {
            Toy.Intensity = Mathf.MoveTowards(Toy.Intensity, 0f, Time.fixedDeltaTime * FadeSpeed);
            if (Toy.Intensity <= 0f)
            {
                Destroy(this);
                Toy.Destroy();
            }
        }
    }
}
