using GorillaNametags.Tags;
using HarmonyLib;

namespace GorillaNametags.Patches;

[HarmonyPatch(typeof(VRRig), "IUserCosmeticsCallback.OnGetUserCosmetics")]
public class CosmeticsLoadedPatch
{
    private static void Postfix(VRRig __instance)
    {
        if (__instance.isLocal)
            return;
        
        __instance.GetComponent<StatsTag>().UpdatePlatform();
    }
}