namespace SwiftArcadeMode.Features.Patches
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection.Emit;
    using System.Runtime.CompilerServices;
    using DrawableLine;
    using HarmonyLib;
    using InventorySystem.Items.ThrowableProjectiles;
    using Mirror;
    using NorthwoodLib.Pools;
    using PlayerRoles;
    using PlayerStatsSystem;
    using RelativePositioning;
    using SwiftArcadeMode.Utils.Deployable;
    using UnityEngine;
    using static HarmonyLib.AccessTools;

    [HarmonyPatch(typeof(Scp018Projectile), nameof(Scp018Projectile.Update))]
    public class Scp018DeployableFix
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            LocalBuilder destructible = generator.DeclareLocal(typeof(IDestructible));

            int index = newInstructions.FindLastIndex(x => x.Calls(
                Method(typeof(Physics), nameof(Physics.OverlapCapsuleNonAlloc), [typeof(Vector3), typeof(Vector3), typeof(float), typeof(Collider[]), typeof(int)])));

            // we do the section ahead first so we dont have to worry about the index being changed for the previous section
            newInstructions.InsertRange(index + 1, [
                new(OpCodes.Ldc_I4_0),
                new(OpCodes.Call, Method(typeof(Hitbox), nameof(Hitbox.ToggleAll)))
            ]);

            newInstructions.InsertRange(index, [
                new(OpCodes.Ldc_I4_1),
                new(OpCodes.Call, Method(typeof(Hitbox), nameof(Hitbox.ToggleAll)))
            ]);

            // all of this is just to make balls collide with deployables AAAAAA
            // tldr; we're replacing component with a local, destructible, that is used, then the whole IsSCP check is contained within a safe cast of destructible to HitboxIdentity
            int offset = 3;
            index = newInstructions.FindLastIndex(x => x.opcode == OpCodes.Ldsfld) + offset;

            newInstructions[index].operand = destructible;
            index++;
            newInstructions[index].operand = typeof(Component).GetMethods().Single(m => m.Name == nameof(Component.TryGetComponent) && m.GetParameters().Length is 1).MakeGenericMethod(typeof(IDestructible));
            index += 4;
            newInstructions[index].operand = destructible;
            index++;
            newInstructions[index].operand = PropertyGetter(typeof(IDestructible), nameof(IDestructible.NetworkId));

            offset = -1;
            index = newInstructions.FindLastIndex(x => x.Calls(PropertyGetter(typeof(HitboxIdentity), nameof(HitboxIdentity.TargetHub)))) + offset;

            Label skipLabel = (Label)newInstructions[index + 4].operand;

            newInstructions.InsertRange(index, [
                new(OpCodes.Ldloc_S, destructible),
                new(OpCodes.Isinst, typeof(HitboxIdentity)),
                new(OpCodes.Dup),
                new(OpCodes.Stloc_S, 5),
                new(OpCodes.Brfalse_S, skipLabel),
            ]);

            index = newInstructions.FindLastIndex(x => x.Calls(Method(typeof(HitboxIdentity), nameof(HitboxIdentity.Damage))));

            newInstructions[index - 8].operand = destructible;
            newInstructions[index].operand = Method(typeof(IDestructible), nameof(IDestructible.Damage));

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}