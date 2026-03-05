namespace SwiftArcadeMode.Features.Patches
{
    using System.Collections.Generic;
    using System.Reflection.Emit;
    using HarmonyLib;
    using InventorySystem.Items.Firearms.Modules;
    using SwiftArcadeMode.Utils.Deployable;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="HitscanHitregModuleBase.SetHitboxes"/> so we can make all deployables hitboxes collidable during the tick a player is firing.
    /// </summary>
    [HarmonyPatch(typeof(HitscanHitregModuleBase), nameof(HitscanHitregModuleBase.SetHitboxes))]
    public class HitregDeployableFix
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            yield return new CodeInstruction(OpCodes.Ldarg_1);
            yield return new CodeInstruction(OpCodes.Ldc_I4_0);
            yield return new CodeInstruction(OpCodes.Ceq);
            yield return new CodeInstruction(OpCodes.Call, Method(typeof(Hitbox), nameof(Hitbox.ToggleAll)));
            foreach (CodeInstruction instruction in instructions)
                yield return instruction;
        }
    }
}