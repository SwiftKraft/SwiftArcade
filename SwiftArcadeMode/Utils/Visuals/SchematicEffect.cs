namespace SwiftArcadeMode.Utils.Visuals
{
    using ProjectMER.Features;
    using ProjectMER.Features.Objects;
    using SwiftArcadeMode.Utils.Structures;
    using UnityEngine;

    public class SchematicEffect : MonoBehaviour
    {
        private readonly Timer lifetime = new();

        public SchematicObject Schematic { get; set; } = null!;

        public static SchematicEffect? Create(string schematicName, Vector3 position, Quaternion rotation, Vector3 scale, float lifetime)
        {
            SchematicObject obj = ObjectSpawner.SpawnSchematic(schematicName, position, rotation, scale);

            if (!obj)
                return null;

            SchematicEffect effect = obj.gameObject.AddComponent<SchematicEffect>();
            effect.Schematic = obj;
            effect.lifetime.Reset(lifetime);
            return effect;
        }

        private void FixedUpdate()
        {
            lifetime.Tick(Time.fixedDeltaTime);
            if (lifetime.Ended)
            {
                Destroy(this);
                Schematic.Destroy();
            }
        }
    }
}
