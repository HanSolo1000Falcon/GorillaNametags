using System.Collections;
using TMPro;
using UnityEngine;

namespace GorillaNametags.Tags;

public class UserIDTag : MonoBehaviour
{
    private TextMeshPro firstPersonUserIDTag;
    private TextMeshPro thirdPersonUserIDTag;

    private void Start()
    {
        if (firstPersonUserIDTag == null)
            firstPersonUserIDTag = Plugin.CreateTag("FirstPersonUserIDTag", Plugin.FirstPersonLayerName,
                Plugin.nametags[GetComponent<VRRig>()].firstPersonNametag.transform, new Vector3(0f, 0.2f, 0f));

        if (thirdPersonUserIDTag == null)
            thirdPersonUserIDTag = Plugin.CreateTag("ThirdPersonUserIDTag", Plugin.ThirdPersonLayerName,
                Plugin.nametags[GetComponent<VRRig>()].thirdPersonNametag.transform, new Vector3(0f, 0.2f, 0f));
        
        firstPersonUserIDTag.text = GetComponent<VRRig>().OwningNetPlayer.UserId;
        thirdPersonUserIDTag.text = GetComponent<VRRig>().OwningNetPlayer.UserId;
    }

    private IEnumerator UpdateColorCoroutine(Color color)
    {
        while (firstPersonUserIDTag == null || thirdPersonUserIDTag == null)
            yield return null;
        
        firstPersonUserIDTag.color = color;
        thirdPersonUserIDTag.color = color;
    }
    
    private void OnDestroy()
    {
        Destroy(firstPersonUserIDTag);
        Destroy(thirdPersonUserIDTag);
    }

    public void UpdateColor(Color color) => StartCoroutine(UpdateColorCoroutine(color));
}