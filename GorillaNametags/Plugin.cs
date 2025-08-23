using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Reflection;
using BepInEx;
using ExitGames.Client.Photon;
using GorillaNametags.Components;
using GorillaNametags.Patches;
using GorillaNametags.Tags;
using Newtonsoft.Json;
using Photon.Pun;
using TMPro;
using UnityEngine;

namespace GorillaNametags;

[BepInPlugin(Constants.PluginGuid, Constants.PluginName, Constants.PluginVersion)]
public class Plugin : BaseUnityPlugin
{
    public static Transform FirstPersonCamera;
    public static Transform ThirdPersonCamera;

    public const string FirstPersonLayerName = "FirstPersonOnly";
    public const string ThirdPersonLayerName = "MirrorOnly";

    public static Dictionary<VRRig, UserIDTag> userIDTags = new();
    public static Dictionary<string, DateTime> createdDates = new();
    
    private const string GorillaInfoURL = "https://raw.githubusercontent.com/HanSolo1000Falcon/GorillaInfo/main/";
    
    private static TMP_FontAsset chosenFont;

    private void Start()
    {
        HarmonyPatches.ApplyHarmonyPatches();
        GorillaTagger.OnPlayerSpawned(OnGameInitialized);

        PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable()
            { { "GorillaNametags", "Made by HanSolo1000Falcon B)" } });
    }

    private void OnGameInitialized()
    {
        FirstPersonCamera = GorillaTagger.Instance.mainCamera.transform;
        ThirdPersonCamera = GorillaTagger.Instance.thirdPersonCamera.transform.GetChild(0);

        Stream bundleStream = Assembly.GetExecutingAssembly()
            .GetManifestResourceStream("GorillaNametags.Resources.gorillanametags");
        AssetBundle bundle = AssetBundle.LoadFromStream(bundleStream);
        bundleStream.Close();

        chosenFont = bundle.LoadAsset<TMP_FontAsset>("COMICBD SDF");
        chosenFont.material.shader = Shader.Find("TextMeshPro/Distance Field");

        gameObject.AddComponent<PunCallbacks>();

        using (HttpClient httpClient = new())
        {
            HttpResponseMessage knownModsResponse = httpClient.GetAsync(GorillaInfoURL + "KnownMods.txt").Result;
            HttpResponseMessage knownCheatsResponse = httpClient.GetAsync(GorillaInfoURL + "KnownCheats.txt").Result;

            knownModsResponse.EnsureSuccessStatusCode();
            knownCheatsResponse.EnsureSuccessStatusCode();

            using (Stream stream = knownModsResponse.Content.ReadAsStreamAsync().Result)
            using (StreamReader reader = new(stream))
                StatsTag.KnownMods = JsonConvert.DeserializeObject<Dictionary<string, string>>(reader.ReadToEnd());
            
            using (Stream stream = knownCheatsResponse.Content.ReadAsStreamAsync().Result)
            using (StreamReader reader = new(stream))
                StatsTag.KnownCheats = JsonConvert.DeserializeObject<Dictionary<string, string>>(reader.ReadToEnd());
        }
    }

    public static TextMeshPro CreateTag(string name, string layerName, Transform parent, Vector3 localPosition)
    {
        GameObject tagObject = new GameObject(name);
        tagObject.layer = LayerMask.NameToLayer(layerName);
        
        tagObject.transform.SetParent(parent);
        tagObject.transform.localPosition = localPosition;
        tagObject.transform.localRotation = Quaternion.identity;

        TextMeshPro tagText = tagObject.AddComponent<TextMeshPro>();
        tagText.richText = true;
        
        tagText.font = chosenFont;
        tagText.fontSize = 1;
        
        tagText.alignment = TextAlignmentOptions.Center;
        
        return tagText;
    }
}