using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GorillaNetworking;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace GorillaNametags.Tags;

public class StatsTag : MonoBehaviour
{
    private static readonly Dictionary<string, string> mods = new()
    {
        { "GFaces", "Gorilla Faces" },
        { "github.com/maroon-shadow/SimpleBoards", "Simple Boards" },
        { "ObsidianMC", "Obsidian.lol" },
        { "github.com/ZlothY29IQ/GorillaMediaDisplay", "Gorilla Media Display" },
        { "hgrehngio889584739_hugb", "Resurgence" },
        { "GTrials", "Gorilla Trials" },
        { "github.com/ZlothY29IQ/TooMuchInfo", "Too Much Info Zlothy" },
        { "github.com/ZlothY29IQ/RoomUtils-IW", "Room Utils" },
        { "github.com/ZlothY29IQ/MonkeClick", "Monke Click" },
        { "github.com/ZlothY29IQ/MonkeClick-CI", "Monke Click CI" },
        { "github.com/ZlothY29IQ/MonkeRealism", "Monke Realism" },
        { "MediaPad", "Media Pad" },
        { "GorillaCinema", "Gorilla Cinema" },
        { "FPS-Nametags for Zlothy", "FPS Nametags" },
        { "ChainedTogetherActive", "Chained Together" },
        { "GPronouns", "Gorilla Pronouns" },
        { "CSVersion", "Custom Skin" },
        { "github.com/ZlothY29IQ/Zloth-RecRoomRig", "Zlothy Body Estimation" },
        { "ShirtProperties", "Gorilla Shirts Old" },
        { "GorillaShirts", "Gorilla Shirts" },
        { "GS", "Old Gorilla Shirts" },
        { "genesis", "ShibaGT Genesis" },
        { "elux", "Elux" },
        { "VioletFreeUser", "Violet Free" },
        { "Hidden Menu", "Hidden Menu" },
        { "HP_Left", "Holdable Pad" },
        { "GrateVersion", "Grate" },
        { "void", "Void Menu" },
        { "BananaOS", "BananaOS" },
        { "GC", "Gorilla Craft" },
        { "CarName", "Vehicles" },
        { "6XpyykmrCthKhFeUfkYGxv7xnXpoe2", "CCMV2" },
        { "cronos", "Cronos Menu" },
        { "ORBIT", "Orbit Menu (Weeb)" },
        { "Violet On Top", "Violet Menu" }, // I do NOT condone cheating, 'Violet On Top' is simply the custom property.
        { "MonkePhone", "Monke Phone" },
        { "Body Tracking", "Body Tracking" },
        { "Body Estimation", "HanSolo Body Estimation" },
        { "Gorilla Track", "Gorilla Track" },
        { "GorillaWatch", "Gorilla Watch" },
        { "InfoWatch", "Gorilla Info Watch" },
        { "BananaPhone", "Banana Phone" },
        { "Vivid", "Vivid Menu" },
        { "CustomMaterial", "Wrysers Custom Cosmetics" },
        { "cheese is gouda", "WhoIsThatMonke" },
        { "I like cheese", "awawe" },
    };
    
    private TextMeshPro firstPersonStatsTag;
    private TextMeshPro thirdPersonStatsTag;

    private Hashtable customProperties;
    
    private string properties;
    private bool hasCosmetx;
    private string platform = "????";

    private void Start()
    {
        if (firstPersonStatsTag == null)
            firstPersonStatsTag = Plugin.CreateTag("FirstPersonUserIDTag", Plugin.FirstPersonLayerName,
                Plugin.nametags[GetComponent<VRRig>()].firstPersonNametag.transform, new Vector3(0f, 0.2f, 0f));

        if (thirdPersonStatsTag == null)
            thirdPersonStatsTag = Plugin.CreateTag("ThirdPersonUserIDTag", Plugin.ThirdPersonLayerName,
                Plugin.nametags[GetComponent<VRRig>()].thirdPersonNametag.transform, new Vector3(0f, 0.2f, 0f));
        
        UpdateProperties(GetComponent<VRRig>().OwningNetPlayer.GetPlayerRef().CustomProperties);
    }

    public void UpdateProperties(Hashtable properties)
    {
        customProperties = properties;
        this.properties = "";
        
        foreach (string key in properties.Keys)
        {
            if (mods.TryGetValue(key, out string value))
                this.properties += $"[{value}]";
        }
    }

    public async void UpdatePlatform()
    {
        string concatStringOfCosmeticsAllowed = GetComponent<VRRig>().concatStringOfCosmeticsAllowed;

        if (concatStringOfCosmeticsAllowed.Contains("S. FIRST LOGIN"))
        {
            platform = "Steam";
            return;
        }

        if (concatStringOfCosmeticsAllowed.Contains("FIRST LOGIN") || customProperties.Count > 1)
        {
            platform = "PC";
            return;
        }

        if (Plugin.createdDates.TryGetValue(GetComponent<VRRig>(), out DateTime createdDate))
        {
            if (createdDate > new DateTime(2023, 02, 08))
            {
                platform = "Quest";
                return;
            }
            
            platform = "????";
            return;
        }
        
        Plugin.createdDates[GetComponent<VRRig>()] = new DateTime(2023, 02, 07);
            
        GetAccountInfoResult actualCreatedDate = await GetAccountCreationDateAsync(GetComponent<VRRig>().OwningNetPlayer.UserId);
        Plugin.createdDates[GetComponent<VRRig>()] = actualCreatedDate.AccountInfo.Created;
            
        if (actualCreatedDate.AccountInfo.Created > new DateTime(2023, 02, 08))
        {
            platform = "Quest";
            return;
        }
            
        platform = "????";
    }

    public void UpdateCosmetx()
    {
        CosmeticsController.CosmeticSet cosmeticSet = GetComponent<VRRig>().cosmeticSet;

        foreach (CosmeticsController.CosmeticItem cosmetic in cosmeticSet.items)
        {
            if (!cosmetic.isNullItem &&
                !GetComponent<VRRig>().concatStringOfCosmeticsAllowed.Contains(cosmetic.itemName))
            {
                hasCosmetx = true;
                return;
            }
        }
    }
    
    private async Task<GetAccountInfoResult> GetAccountCreationDateAsync(string userID)
    {
        var tcs = new TaskCompletionSource<GetAccountInfoResult>();

        PlayFabClientAPI.GetAccountInfo(new GetAccountInfoRequest { PlayFabId = userID },
            result => tcs.SetResult(result),
            error =>
            {
                Debug.LogError("Failed to get account info: " + error.ErrorMessage);
                tcs.SetException(new Exception(error.ErrorMessage));
            });

        return await tcs.Task;
    }

    private void OnDestroy()
    {
        Destroy(firstPersonStatsTag);
        Destroy(thirdPersonStatsTag);
    }

    private void Update()
    {
        string cosmetxText = hasCosmetx ? "[CosmetX]" : "";
        string fullText = $"{properties}{cosmetxText} | [{platform}]";
        firstPersonStatsTag.text = fullText;
        thirdPersonStatsTag.text = fullText;
    }
}