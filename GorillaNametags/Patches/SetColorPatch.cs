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
        
        Color realColor = color;
        
        if (__instance.bodyRenderer.cosmeticBodyType == GorillaBodyType.Skeleton)
            realColor = Color.green;

        switch (__instance.setMatIndex)
        {
            case 1:
                realColor = Color.red;
                break;

            case 2:
            case 11:
                realColor = new Color(1f, 0.3288f, 0f, 1f);
                break;

            case 3:
            case 7:
                realColor = Color.blue;
                break;

            case 12:
                realColor = Color.green;
                break;
        }

        __instance.GetOrAddComponent<UserIDTag>(out UserIDTag userIDTag);
        userIDTag.UpdateColor(realColor);
        
        __instance.GetOrAddComponent<StatsTag>(out _);
        
        __instance.GetOrAddComponent<FPSNametag>(out FPSNametag nametag);
        nametag.UpdateColor(realColor);
        
        __instance.GetOrAddComponent<AccountCreationDateTag>(out AccountCreationDateTag accountCreationDateTag);
        accountCreationDateTag.UpdateColor(realColor);
    }
}