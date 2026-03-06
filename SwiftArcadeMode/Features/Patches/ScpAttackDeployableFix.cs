namespace SwiftArcadeMode.Features.Patches
{
    using System.Collections.Generic;
    using System.Reflection.Emit;
    using HarmonyLib;
    using NorthwoodLib.Pools;
    using PlayerRoles.PlayableScps.Scp3114;
    using PlayerRoles.PlayableScps.Subroutines;
    using SwiftArcadeMode.Utils.Deployable;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="ScpAttackAbilityBase{T}.ServerPerformAttack"/> so we can make all deployables hitboxes collidable during the tick a scp is attacking.
    /// </summary>
    /// <remarks>Also excludes duplicate attacks on the same IDestructible.</remarks>
    // the target generic here should be irrelevant because all the generics are going to share the same IL for this method because the generics are not value types--ask harmony discord for more info--so I just use Scp3114Role for funsies
    [HarmonyPatch(typeof(ScpAttackAbilityBase<Scp3114Role>), nameof(ScpAttackAbilityBase<>.ServerPerformAttack))]
    public class ScpAttackDeployableFix
    {
        public static HashSet<uint> NetIds { get; } = [];

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            Label continueLabel = (Label)newInstructions.Find(x => x.opcode == OpCodes.Br_S).operand;

            newInstructions.InsertRange(0, [
                new(OpCodes.Ldc_I4_1),
                new(OpCodes.Call, Method(typeof(Hitbox), nameof(Hitbox.ToggleAll))),
            ]);

            newInstructions.InsertRange(newInstructions.Count - 1, [
                new(OpCodes.Call, PropertyGetter(typeof(ScpAttackDeployableFix), nameof(NetIds))),
                new(OpCodes.Callvirt, Method(typeof(HashSet<uint>), nameof(HashSet<>.Clear))),
                new(OpCodes.Ldc_I4_0),
                new(OpCodes.Call, Method(typeof(Hitbox), nameof(Hitbox.ToggleAll))),
            ]);

            int offset = -1;
            int index = newInstructions.FindLastIndex(x => x.opcode == OpCodes.Ldloc_2) + offset;

            newInstructions.InsertRange(index, [
                new(OpCodes.Call, PropertyGetter(typeof(ScpAttackDeployableFix), nameof(NetIds))),
                new(OpCodes.Ldloc_2),
                new(OpCodes.Callvirt, PropertyGetter(typeof(IDestructible), nameof(IDestructible.NetworkId))),
                new(OpCodes.Callvirt, Method(typeof(HashSet<uint>), nameof(HashSet<>.Add))),
                new(OpCodes.Brfalse_S, continueLabel),
            ]);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}