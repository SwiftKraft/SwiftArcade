namespace SwiftArcadeMode.Features.Patches
{
    using System.Collections.Generic;
    using System.Reflection.Emit;
    using HarmonyLib;
    using NorthwoodLib.Pools;
    using ProjectMER.Features;
    using ProjectMER.Features.Serializable.Schematics;
    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="SerializableSchematic.SpawnOrUpdateObject"/> to make it check this class for an existing <see cref="SchematicObjectDataList"/> cuz PMER is stoopid.
    /// </summary>
    [HarmonyPatch(typeof(SerializableSchematic), nameof(SerializableSchematic.SpawnOrUpdateObject))]
    public class SerializableSchematicPatch
    {
        public static Dictionary<SerializableSchematic, SchematicObjectDataList> ExistingData { get; } = new();

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            int offset = -3;
            int index = newInstructions.FindIndex(x => x.Calls(Method(typeof(MapUtils), nameof(MapUtils.TryGetSchematicDataByName))));

            // subject to change if the method is changed
            const int DataLocalIndex = 3;

            newInstructions.InsertRange(index + offset, [
                new(OpCodes.Call, PropertyGetter(typeof(SerializableSchematicPatch), nameof(ExistingData))),
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldloca_S, DataLocalIndex),
                new(OpCodes.Callvirt, Method(typeof(Dictionary<SerializableSchematic, SchematicObjectDataList>), nameof(Dictionary<,>.TryGetValue))),
                new(OpCodes.Brtrue_S, newInstructions[index + 3].operand),
            ]);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}