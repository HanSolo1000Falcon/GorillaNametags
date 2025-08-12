using System;
using System.Collections;
using System.Threading.Tasks;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using UnityEngine;

namespace GorillaNametags.Tags;

public class AccountCreationDateTag : MonoBehaviour
{
    private TextMeshPro firstPersonAccountCreationDateTag;
    private TextMeshPro thirdPersonAccountCreationDateTag;
    
    private IEnumerator UpdateColorCoroutine(Color color)
    {
        while (firstPersonAccountCreationDateTag == null || thirdPersonAccountCreationDateTag == null)
            yield return null;

        firstPersonAccountCreationDateTag.color = color;
        thirdPersonAccountCreationDateTag.color = color;
    }
    
    private async void GetAccountCreationDate()
    {
        if (Plugin.createdDates.TryGetValue(GetComponent<VRRig>().OwningNetPlayer.UserId, out DateTime createdDate))
        {
            string text = createdDate.ToShortDateString();
            firstPersonAccountCreationDateTag.text = text;
            thirdPersonAccountCreationDateTag.text = text;
        }
        else
        {
            Plugin.createdDates[GetComponent<VRRig>().OwningNetPlayer.UserId] = new DateTime(2023, 02, 07);
            GetAccountInfoResult actualCreatedDate = await GetAccountCreationDateAsync(GetComponent<VRRig>().OwningNetPlayer.UserId);
            Plugin.createdDates[GetComponent<VRRig>().OwningNetPlayer.UserId] = actualCreatedDate.AccountInfo.Created;
            
            string text = actualCreatedDate.AccountInfo.Created.ToShortDateString();
            firstPersonAccountCreationDateTag.text = text;
            thirdPersonAccountCreationDateTag.text = text;
            
            GetComponent<StatsTag>().UpdatePlatform();
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
        Destroy(firstPersonAccountCreationDateTag);
        Destroy(thirdPersonAccountCreationDateTag);
    }

    private void Start()
    {
        if (firstPersonAccountCreationDateTag == null)
            firstPersonAccountCreationDateTag = Plugin.CreateTag("FirstPersonAccountCreationDateTag", Plugin.FirstPersonLayerName,
                Plugin.userIDTags[GetComponent<VRRig>()].firstPersonUserIDTag.transform, new Vector3(0f, 0.3f, 0f));

        if (thirdPersonAccountCreationDateTag == null)
            thirdPersonAccountCreationDateTag = Plugin.CreateTag("ThirdPersonAccountCreationDateTag", Plugin.ThirdPersonLayerName,
                Plugin.userIDTags[GetComponent<VRRig>()].thirdPersonUserIDTag.transform, new Vector3(0f, 0.3f, 0f));
        
        GetAccountCreationDate();
    }
    
    public void UpdateColor(Color color) => StartCoroutine(UpdateColorCoroutine(color));
}