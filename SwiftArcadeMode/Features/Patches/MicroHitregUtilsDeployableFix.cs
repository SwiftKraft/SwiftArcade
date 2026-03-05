namespace SwiftArcadeMode.Features.Patches
{
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;
    using HarmonyLib;
    using InventorySystem.Items.MicroHID.Modules;
    using NorthwoodLib.Pools;
    using SwiftArcadeMode.Utils.Deployable;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="HitregUtils.Raycast"/> and <see cref="HitregUtils.OverlapSphere"/> so we can make all deployables hitboxes collidable during the tick a micro is checking for hits.
    /// </summary>
    [HarmonyPatch]
    public class MicroHitregUtilsDeployableFix
    {
        public static IEnumerable<MethodBase> TargetMethods()
        {
            yield return Method(typeof(HitregUtils), nameof(HitregUtils.Raycast));
            yield return Method(typeof(HitregUtils), nameof(HitregUtils.OverlapSphere));
        }

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            newInstructions.InsertRange(0, [
                new(OpCodes.Ldc_I4_1),
                new(OpCodes.Call, Method(typeof(Hitbox), nameof(Hitbox.ToggleAll))),
            ]);

            newInstructions.InsertRange(newInstructions.Count - 1, [
                new CodeInstruction(OpCodes.Ldc_I4_0),
                new(OpCodes.Call, Method(typeof(Hitbox), nameof(Hitbox.ToggleAll))),
            ]);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}