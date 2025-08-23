using GorillaNametags.Tags;
using UnityEngine;
using HarmonyLib;

namespace GorillaNametags.Patches;

[HarmonyPatch(typeof(VRRig), nameof(VRRig.SetColor))]
public class SetColorPatch
{
    private static void Postfix(VRRig __instance, Color color)
    {
        if (__instance.isLocal)
            return;
        
        __instance.GetOrAddComponent<UserIDTag>(out UserIDTag userIDTag);
    }
}