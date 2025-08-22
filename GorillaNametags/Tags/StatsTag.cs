using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GorillaNetworking;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Random = System.Random;

namespace GorillaNametags.Tags;

public class StatsTag : MonoBehaviour
{
    public static Dictionary<string, string> KnownMods = new();
    public static Dictionary<string, string> KnownCheats = new();

    private static readonly string[] colors = new string[]
    {
        "red",
        "#699F3B",
        "#85B2D5",
        "#DA3733",
        "#3C228B",
        "#D94E01",
        "#89B58B",
        "green",
        "yellow",
    };

    private TextMeshPro firstPersonStatsTag;
    private TextMeshPro thirdPersonStatsTag;

    private Hashtable customProperties;

    private string properties;
    private string platform = "????";

    private void Start()
    {
        if (firstPersonStatsTag == null)
            firstPersonStatsTag = Plugin.CreateTag("FirstPersonUserIDTag", Plugin.FirstPersonLayerName,
                Plugin.userIDTags[GetComponent<VRRig>()].firstPersonUserIDTag.transform, new Vector3(0f, 0.1f, 0f));

        if (thirdPersonStatsTag == null)
            thirdPersonStatsTag = Plugin.CreateTag("ThirdPersonUserIDTag", Plugin.ThirdPersonLayerName,
                Plugin.userIDTags[GetComponent<VRRig>()].thirdPersonUserIDTag.transform, new Vector3(0f, 0.1f, 0f));

        firstPersonStatsTag.transform.localRotation = Quaternion.identity;
        thirdPersonStatsTag.transform.localRotation = Quaternion.identity;

        UpdateProperties();
    }

    public void UpdateProperties()
    {
        Random random = new Random();

        customProperties = GetComponent<VRRig>().OwningNetPlayer.GetPlayerRef().CustomProperties;
        properties = "";

        foreach (string key in customProperties.Keys)
        {
            if (KnownMods.TryGetValue(key, out string modName))
                properties += $"<color={colors[random.Next(0, colors.Length)]}>[{modName}]</color>";
            
            if (KnownCheats.TryGetValue(key, out string cheatName))
                properties += $"<color={colors[random.Next(0, colors.Length)]}>[{cheatName}]</color>";
        }
    }

    public async void UpdatePlatform()
    {
        if (platform == "Steam")
            return;
        
        string concatStringOfCosmeticsAllowed = GetComponent<VRRig>().concatStringOfCosmeticsAllowed;

        if (concatStringOfCosmeticsAllowed.Contains("S. FIRST LOGIN"))
        {
            platform = "Steam";
            return;
        }

        if (platform == "PC")
            return;
        
        if (concatStringOfCosmeticsAllowed.Contains("FIRST LOGIN") || customProperties.Count > 1)
        {
            platform = "PC";
            return;
        }
        
        if (platform == "Steam" || platform == "PC" || platform == "Quest")
            return;

        if (Plugin.createdDates.TryGetValue(GetComponent<VRRig>().OwningNetPlayer.UserId, out DateTime createdDate))
        {
            if (createdDate > new DateTime(2023, 02, 06))
            {
                platform = "Quest";
                return;
            }

            platform = "????";
            return;
        }

        Plugin.createdDates[GetComponent<VRRig>().OwningNetPlayer.UserId] = new DateTime(2023, 02, 05);

        GetAccountInfoResult actualCreatedDate =
            await GetAccountCreationDateAsync(GetComponent<VRRig>().OwningNetPlayer.UserId);
        Plugin.createdDates[GetComponent<VRRig>().OwningNetPlayer.UserId] = actualCreatedDate.AccountInfo.Created;

        if (TryGetComponent<AccountCreationDateTag>(out AccountCreationDateTag accountCreationDateTag))
        {
            accountCreationDateTag.firstPersonAccountCreationDateTag.text = actualCreatedDate.AccountInfo.Created.ToShortDateString();
            accountCreationDateTag.thirdPersonAccountCreationDateTag.text = actualCreatedDate.AccountInfo.Created.ToShortDateString();
        }
        
        if (actualCreatedDate.AccountInfo.Created > new DateTime(2023, 02, 06))
        {
            platform = "Quest";
            return;
        }

        platform = "????";
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

    private void EnsureTagsCreated()
    {
        if (firstPersonStatsTag == null || thirdPersonStatsTag == null)
            Start();
    }

    private void Update()
    {
        EnsureTagsCreated();
        if (firstPersonStatsTag == null || thirdPersonStatsTag == null)
            return;

        string fullText = $"{properties}<color=#1761B4>[{platform}]</color>";

        firstPersonStatsTag.text = fullText;
        thirdPersonStatsTag.text = fullText;
    }
}