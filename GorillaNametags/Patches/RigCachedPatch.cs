using GorillaNametags.Tags;
using HarmonyLib;
using UnityEngine;

namespace GorillaNametags.Patches;

[HarmonyPatch(typeof(VRRigCache), nameof(VRRigCache.RemoveRigFromGorillaParent))]
public class RigCachedPatch
{
    private static void Postfix(NetPlayer player, VRRig vrrig)
    {
        Object.Destroy(vrrig.GetComponent<UserIDTag>());
        Object.Destroy(vrrig.GetComponent<Nametag>());
    }
}