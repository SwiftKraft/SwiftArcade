namespace SwiftArcadeMode.Utils.Extensions
{
    using System.IO;
    using ProjectMER.Features;
    using ProjectMER.Features.Objects;
    using ProjectMER.Features.Serializable.Schematics;
    using SwiftArcadeMode.Features.Patches;
    using UnityEngine;
    using Utf8Json;
    using Logger = LabApi.Features.Console.Logger;

    public static class SchematicExtensions
    {
        public static SchematicObject? SpawnSchematic(string name, Vector3 position, Quaternion rotation) => SpawnSchematic(name, position, rotation, Vector3.one);

        public static SchematicObject? SpawnSchematic(string name, Vector3 position, Quaternion rotation, Vector3 scale)
        {
#pragma warning disable CS0618 // Type or member is obsolete
            if (Directory.Exists(Path.Combine(ProjectMER.ProjectMER.SchematicsDir, name.ApplySchematicPrefix())))
            {
                return ObjectSpawner.SpawnSchematic(name.ApplySchematicPrefix(), position, rotation, scale);
            }
#pragma warning restore CS0618 // Type or member is obsolete

            SerializableSchematic schematic = new()
            {
                SchematicName = name,
                Position = position,
                Rotation = rotation.eulerAngles,
                Scale = scale,
            };

            SchematicObjectDataList? data = GetSchematicDataByNameAndDirectory(Core.CoreConfig.SchematicsDirectory, name);
            if (data == null)
                return null;

            SerializableSchematicPatch.ExistingData.Add(schematic, data);

            return ObjectSpawner.SpawnSchematic(schematic);
        }

        public static SchematicObjectDataList? GetSchematicDataByNameAndDirectory(string dir, string schematicName)
        {
            string directory = Path.Combine(ProjectMER.ProjectMER.SchematicsDir, dir, schematicName);
            string file = Path.Combine(directory, schematicName + ".json");

            if (!Directory.Exists(directory))
            {
                Logger.Error($"Failed to load schematic data: Directory {schematicName} does not exist!");
                return null;
            }

            if (!File.Exists(file))
            {
                Logger.Error($"Failed to load schematic data: File {schematicName} does not exist!");
                return null;
            }

            SchematicObjectDataList data;
            try
            {
                data = JsonSerializer.Deserialize<SchematicObjectDataList>(File.ReadAllText(file));
                data.Path = directory;
            }
            catch (JsonParsingException ex)
            {
                string message = $"Failed to load schematic data: File {schematicName}.json has JSON errors!\n{ex.ToString().Split('\n')[0]}";
                Logger.Error(message);
                return null;
            }

            return data;
        }
    }
}