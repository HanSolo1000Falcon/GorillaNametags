using System.Collections.Generic;
using System.IO;
using System.Reflection;
using BepInEx;
using GorillaNametags.Patches;
using GorillaNametags.Tags;
using TMPro;
using UnityEngine;

namespace GorillaNametags;

[BepInPlugin(Constants.PluginGuid, Constants.PluginName, Constants.PluginVersion)]
public class Plugin : BaseUnityPlugin
{
    public static Transform firstPersonCamera;
    public static Transform thirdPersonCamera;
    
    public static TMP_FontAsset comicSans;
    
    public const string FirstPersonLayerName = "FirstPersonOnly";
    public const string ThirdPersonLayerName = "MirrorOnly";
    
    public static Dictionary<VRRig, Nametag> nametags = new();
    
    private void Start()
    {
        HarmonyPatches.ApplyHarmonyPatches();
        GorillaTagger.OnPlayerSpawned(OnGameInitialized);
    }

    private void OnGameInitialized()
    {
        firstPersonCamera = GorillaTagger.Instance.mainCamera.transform;
        thirdPersonCamera = GorillaTagger.Instance.thirdPersonCamera.transform.GetChild(0);
        
        Stream bundleStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("GorillaNametags.Resources.gorillanametags");
        AssetBundle bundle = AssetBundle.LoadFromStream(bundleStream);
        bundleStream.Close();
        
        comicSans = bundle.LoadAsset<TMP_FontAsset>("COMICBD SDF");
        comicSans.material.shader = Shader.Find("TextMeshPro/Distance Field");
    }

    public static TextMeshPro CreateTag(string name, string layerName, Transform parent, Vector3 localPosition)
    {
        GameObject tagObject = new GameObject(name);
        tagObject.transform.SetParent(parent);
        tagObject.transform.localPosition = localPosition;
        tagObject.layer = LayerMask.NameToLayer(layerName);
        
        TextMeshPro tagText = tagObject.AddComponent<TextMeshPro>();
        tagText.font = comicSans;
        tagText.fontSize = 2;
        tagText.alignment = TextAlignmentOptions.Center;
        return tagText;
    }
}