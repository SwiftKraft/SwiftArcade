namespace SwiftArcadeMode.Utils.Deployable
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using AdminToys;
    using LabApi.Features.Wrappers;
    using PlayerStatsSystem;
    using ProjectMER.Features;
    using ProjectMER.Features.Objects;
    using SwiftArcadeMode.Utils.Extensions;
    using UnityEngine;

    using Logger = LabApi.Features.Console.Logger;

    public abstract class DeployableBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeployableBase"/> class.
        /// </summary>
        /// <param name="name">The name of the deployable.</param>
        /// <param name="schematicName">The name of the schematic.</param>
        /// <param name="position">The position of the deployable.</param>
        /// <param name="rotation">The rotation of the deployable.</param>
        /// <exception cref="InvalidOperationException">Thrown if the target schematic either doesnt exist, or has no head.</exception>
        /// <remarks>Call <see cref="Initialize"/> on the new instance after this constructor.</remarks>
        public DeployableBase(string name, string schematicName, Vector3 position, Quaternion rotation)
        {
            Name = name;

            Schematic = SchematicExtensions.SpawnSchematic(schematicName, position, rotation) ?? throw new InvalidOperationException($"Failed to spawn deployable {schematicName}!");

            RegisterSchematicProperties(schematicName);

            if (Head is null)
                throw new InvalidOperationException($"Deployable {schematicName} has no head!");

            DeployableManager.AllDeployables.Add(this);
        }

        public static Dictionary<string, int[]> SchematicPropertyCache { get; } = new();

        public static FileSystemWatcher? Watcher { get; } = TryInitializeWatcher();

        public string Name { get; }

        public virtual string TypeName => GetType().Name;

        public Vector3 Position
        {
            get => Schematic.Position;
            set => Schematic.Position = value;
        }

        public Quaternion Rotation
        {
            get => Schematic.Rotation;
            set => Schematic.Rotation = value;
        }

        public abstract float MaxHealth { get; }

        public float Health { get; set; }

        public bool Destroyed { get; private set; }

        public SchematicObject Schematic { get; set; }

        public AdminToy Head { get; set; }

        public AnimationController Animator => Schematic.AnimationController;

        public List<Hitbox> Hitboxes { get; } = [];

        public virtual void Initialize()
        {
            Health = MaxHealth;
        }

        public virtual void Tick()
        {
            if (!Schematic)
                Destroy();
        }

        public virtual void OnHit(float damage, DamageHandlerBase handler, Vector3 hitPosition)
        {
            Health -= damage;

            if (Health <= 0)
                Destroy();
        }

        public virtual void Destroy()
        {
            DeployableManager.AllDeployables.Remove(this);
            if (Schematic && Schematic.gameObject)
                Schematic.Destroy();
            Destroyed = true;
        }

        private static FileSystemWatcher? TryInitializeWatcher()
        {
            if (!Directory.Exists(ProjectMER.ProjectMER.SchematicsDir))
                return null;

            string dir = Path.Combine(ProjectMER.ProjectMER.SchematicsDir, Core.CoreConfig.SchematicsDirectory);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            FileSystemWatcher watcher = new(dir);

            watcher.Changed += OnChanged;
            watcher.Renamed += OnChanged;
            watcher.Deleted += OnChanged;
            watcher.Created += OnChanged;

            watcher.Error += (_, ev) => { Logger.Error(ev.GetException()); };

            return watcher;
        }

        private static void OnChanged(object source, FileSystemEventArgs e)
        {
            if (Path.GetExtension(e.FullPath) != ".json")
                return;

            SchematicPropertyCache.Remove(Path.ChangeExtension(e.Name, null));
        }

        private void RegisterSchematicProperties(string schematicName)
        {
            if (SchematicPropertyCache.TryGetValue(schematicName, out int[] indices))
            {
                for (int i = 0; i < indices.Length; i++)
                    CheckToy(Schematic.AdminToyBases[indices[i]]);
            }

            HashSet<int> keyIndices = [];

            for (int i = 0; i < Schematic.AdminToyBases.Count; i++)
            {
                if (CheckToy(Schematic.AdminToyBases[i]))
                    keyIndices.Add(i);
            }

            if (Watcher != null)
                SchematicPropertyCache[schematicName] = keyIndices.ToArray();

            return;

            bool CheckToy(AdminToyBase toy)
            {
                bool result = false;
                if (toy.name.Contains("SAM-DeployableHead") && Head is null)
                {
                    Head = AdminToy.Get(toy);
                    result = true;
                }

                if (toy is not AdminToys.PrimitiveObjectToy prim)
                    return result;

                const string HitboxTag = "SAM-Hitbox(";
                int index = toy.name.IndexOf(HitboxTag, StringComparison.Ordinal);
                if (index is not -1)
                {
                    index += HitboxTag.Length;

                    int nextIndex = toy.name.IndexOf(')', index);
                    string tag = toy.name.Substring(index, nextIndex - index);

                    if (!float.TryParse(tag, out float multiplier))
                    {
                        Logger.Warn($"Hitbox {toy} for deployable {Name} has an invalid damage multiplier and will be ignored. (Processed value: {tag})");
                        return false;
                    }

                    Hitboxes.Add(Hitbox.Create(this, prim, multiplier));
                    result = true;
                }

                return result;
            }
        }
    }
}
