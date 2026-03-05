namespace SwiftArcadeMode.Features.Patches
{
    using System.Collections.Generic;
    using System.Reflection.Emit;
    using HarmonyLib;
    using InventorySystem.Items.Autosync;
    using NorthwoodLib.Pools;
    using SwiftArcadeMode.Utils.Deployable;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="MeleeAutoSync.DetectDestructibles"/> so we can make all deployables hitboxes collidable during the tick a player is attacking with a melee weapon.
    /// </summary>
    [HarmonyPatch(typeof(MeleeAutoSync), nameof(MeleeAutoSync.DetectDestructibles))]
    public class MeleeDeployableFix
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            Label retLabel = generator.DefineLabel();

            newInstructions.InsertRange(0, [
              new(OpCodes.Ldc_I4_1),
              new(OpCodes.Call, Method(typeof(Hitbox), nameof(Hitbox.ToggleAll))),
            ]);

            newInstructions.InsertRange(newInstructions.Count - 1, [
              new CodeInstruction(OpCodes.Ldc_I4_0).WithLabels(retLabel),
              new(OpCodes.Call, Method(typeof(Hitbox), nameof(Hitbox.ToggleAll))),
            ]);

            // make all exit cases toggle hitboxes off
            for (int z = 0; z < newInstructions.Count; z++)
            {
              if (newInstructions[z].opcode != OpCodes.Ret || z == newInstructions.Count - 1)
              {
                yield return newInstructions[z];
                continue;
              }

              newInstructions[z].opcode = OpCodes.Br_S;
              newInstructions[z].operand = retLabel;

              yield return newInstructions[z];
            }

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}