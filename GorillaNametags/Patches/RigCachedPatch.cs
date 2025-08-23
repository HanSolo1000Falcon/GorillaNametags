using GorillaNametags.Tags;
using HarmonyLib;
using UnityEngine;

namespace GorillaNametags.Patches;

[HarmonyPatch(typeof(VRRigCache), nameof(VRRigCache.RemoveRigFromGorillaParent))]
public class RigCachedPatch
{
    private static void Postfix(NetPlayer player, VRRig vrrig)
    {
        if (vrrig.isLocal)
            return;
        
        Object.Destroy(vrrig.GetComponent<UserIDTag>());
    }
}