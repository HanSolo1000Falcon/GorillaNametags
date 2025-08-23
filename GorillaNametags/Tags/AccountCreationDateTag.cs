using System;
using System.Threading.Tasks;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

namespace GorillaNametags.Tags;

public class AccountCreationDateTag : TagBase
{
    protected override void Start()
    {
        localPosition = new Vector3(0f, 0.3f, 0f);
        base.Start();
        
        GetAccountCreationDate();
    }
    
    private async void GetAccountCreationDate()
    {
        if (Plugin.createdDates.TryGetValue(GetComponent<VRRig>().OwningNetPlayer.UserId, out DateTime createdDate))
        {
            string text = createdDate.ToShortDateString();
            firstPersonTag.text = text;
            thirdPersonTag.text = text;
        }
        else
        {
            Plugin.createdDates[GetComponent<VRRig>().OwningNetPlayer.UserId] = new DateTime(2023, 02, 05);
            GetAccountInfoResult actualCreatedDate = await GetAccountCreationDateAsync(GetComponent<VRRig>().OwningNetPlayer.UserId);
            Plugin.createdDates[GetComponent<VRRig>().OwningNetPlayer.UserId] = actualCreatedDate.AccountInfo.Created;
            
            string text = actualCreatedDate.AccountInfo.Created.ToShortDateString();
            firstPersonTag.text = text;
            thirdPersonTag.text = text;
            
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
    
    public void UpdateColour(Color colour)
    {
        if (firstPersonTag == null || thirdPersonTag == null)
            return;
        
        firstPersonTag.color = colour;
        thirdPersonTag.color = colour;
    }
}