using System.Collections;
using TMPro;
using UnityEngine;

namespace GorillaNametags.Tags;

public class UserIDTag : MonoBehaviour
{
    public TextMeshPro firstPersonUserIDTag;
    public TextMeshPro thirdPersonUserIDTag;

    private void Start()
    {
        if (firstPersonUserIDTag == null)
            firstPersonUserIDTag = Plugin.CreateTag("FirstPersonUserIDTag", Plugin.FirstPersonLayerName, transform,
                new Vector3(0f, 0.7f, 0f));

        if (thirdPersonUserIDTag == null)
            thirdPersonUserIDTag = Plugin.CreateTag("ThirdPersonUserIDTag", Plugin.ThirdPersonLayerName, transform,
                new Vector3(0f, 0.7f, 0f));

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
        Plugin.userIDTags.Remove(GetComponent<VRRig>());
        Destroy(firstPersonUserIDTag);
        Destroy(thirdPersonUserIDTag);
    }

    private void Update()
    {
        firstPersonUserIDTag.transform.LookAt(Plugin.firstPersonCamera);
        thirdPersonUserIDTag.transform.LookAt(Plugin.thirdPersonCamera);

        firstPersonUserIDTag.transform.Rotate(0f, 180f, 0f);
        thirdPersonUserIDTag.transform.Rotate(0f, 180f, 0f);
    }
    
    public void UpdateColor(Color color)
    {
        StartCoroutine(UpdateColorCoroutine(color));
        Plugin.userIDTags[GetComponent<VRRig>()] = this;
    }
}