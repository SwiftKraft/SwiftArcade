namespace SwiftArcadeMode.Features.Patches
{
    using System.Collections.Generic;
    using System.Reflection.Emit;
    using HarmonyLib;
    using InventorySystem.Items.ThrowableProjectiles;
    using SwiftArcadeMode.Utils.Deployable;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="ExplosionGrenade.SetHostHitboxes"/> so we can make all deployables hitboxes collidable during the tick an explosion is happening.
    /// </summary>
    [HarmonyPatch(typeof(ExplosionGrenade), nameof(ExplosionGrenade.SetHostHitboxes))]
    public class ExplosionDeployableFix
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            yield return new CodeInstruction(OpCodes.Ldarg_0);
            yield return new CodeInstruction(OpCodes.Call, Method(typeof(Hitbox), nameof(Hitbox.ToggleAll)));

            foreach (CodeInstruction instruction in instructions)
                yield return instruction;
        }
    }
}