using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

    public static event Action<TMP_FontAsset> OnFontReloaded;

    public const string FirstPersonLayerName = "FirstPersonOnly";
    public const string ThirdPersonLayerName = "MirrorOnly";

    public static Dictionary<string, DateTime> CreatedDates = new();
    
    private const string GorillaInfoURL = "https://raw.githubusercontent.com/HanSolo1000Falcon/GorillaInfo/main/";
    
    private static TMP_FontAsset chosenFont;

    private bool isGuiOpen = true;

    private void Start()
    {
        HarmonyPatches.ApplyHarmonyPatches();
        GorillaTagger.OnPlayerSpawned(OnGameInitialized);

        PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable()
            { { "GorillaNametags", "Made by HanSolo1000Falcon B)" } });
    }

    private void LoadCurrentFont()
    {
        string directoryPath = Path.Combine(Paths.BepInExRootPath, "GorillaNametagsFont");

        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);

            using (Stream fontStream = Assembly.GetExecutingAssembly()
                       .GetManifestResourceStream("GorillaNametags.GorillaNametagsFont.comicbd.ttf"))
            {
                using (FileStream fileStream = new FileStream(Path.Combine(directoryPath, "comicbd.ttf"), FileMode.Create, FileAccess.Write))
                    fontStream.CopyTo(fileStream);
            }
        }

        try
        {
            string firstFont = Directory.EnumerateFiles(directoryPath, "*.*", SearchOption.TopDirectoryOnly).FirstOrDefault(f => f.EndsWith(".ttf", StringComparison.OrdinalIgnoreCase) || f.EndsWith(".otf", StringComparison.OrdinalIgnoreCase));
            chosenFont = TMP_FontAsset.CreateFontAsset(new Font(firstFont));
            chosenFont.material.shader = Shader.Find("TextMeshPro/Distance Field");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error loading font: {ex.Message}");
            Application.Quit();
        }
    }

    private void OnGUI()
    {
        if (!isGuiOpen)
            return;
        
        GUI.Label(new Rect(20f, Screen.height - 40f, 400f, 20f), "Press 'L' to hot-reload font; press 'O' to open/close GUI");
    }

    private void Update()
    {
        if (UnityInput.Current.GetKeyDown(KeyCode.O))
            isGuiOpen = !isGuiOpen;

        if (UnityInput.Current.GetKeyDown(KeyCode.L))
        {
            LoadCurrentFont();
            OnFontReloaded?.Invoke(chosenFont);
        }
    }

    private void OnGameInitialized()
    {
        FirstPersonCamera = GorillaTagger.Instance.mainCamera.transform;
        ThirdPersonCamera = GorillaTagger.Instance.thirdPersonCamera.transform.GetChild(0);

        gameObject.AddComponent<PunCallbacks>();
        
        LoadCurrentFont();

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