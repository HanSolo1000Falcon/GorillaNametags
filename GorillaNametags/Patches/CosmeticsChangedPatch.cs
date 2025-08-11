using GorillaNametags.Tags;
using HarmonyLib;

namespace GorillaNametags.Patches;

[HarmonyPatch(typeof(VRRig), nameof(VRRig.SetCosmeticsActive))]
public class CosmeticsChangedPatch
{
    private static void Postfix(VRRig __instance)
    {
        if (__instance.isLocal || __instance.inTryOnRoom)
            return;

        __instance.GetOrAddComponent<StatsTag>(out StatsTag statsTag);
        statsTag.UpdateCosmetx();
    }
}