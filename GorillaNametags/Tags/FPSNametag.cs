using System.Collections;
using TMPro;
using UnityEngine;

namespace GorillaNametags.Tags;

public class FPSNametag : MonoBehaviour
{
    private TextMeshPro firstPersonNametag;
    private TextMeshPro thirdPersonNametag;

    private VRRig rig;
    private NetPlayer netPlayer;

    private void Update()
    {
        int fps = rig.fps;

        string tagColour = "green";
        
        switch (fps)
        {
            case < 60:
                tagColour = "red";
                break;
            case < 90:
                tagColour = "yellow";
                break;
            default:
                tagColour = "green";
                break;
        }

        string text = $"{netPlayer.NickName} | <color={tagColour}>{fps} FPS</color>";
        
        firstPersonNametag.text = text;
        thirdPersonNametag.text = text;
    }

    private void OnDestroy()
    {
        Destroy(firstPersonNametag);
        Destroy(thirdPersonNametag);
    }

    private void Start()
    {
        rig = GetComponent<VRRig>();
        netPlayer = rig.OwningNetPlayer;
        
        if (firstPersonNametag == null)
            firstPersonNametag = Plugin.CreateTag("FirstPersonNametag", Plugin.FirstPersonLayerName,
                Plugin.userIDTags[GetComponent<VRRig>()].firstPersonUserIDTag.transform, new Vector3(0f, 0.2f, 0f));

        if (thirdPersonNametag == null)
            thirdPersonNametag = Plugin.CreateTag("ThirdPersonNametag", Plugin.ThirdPersonLayerName,
                Plugin.userIDTags[GetComponent<VRRig>()].thirdPersonUserIDTag.transform, new Vector3(0f, 0.2f, 0f));
    }
    
    private IEnumerator UpdateColorCoroutine(Color color)
    {
        while (firstPersonNametag == null || thirdPersonNametag == null)
            yield return null;

        firstPersonNametag.color = color;
        thirdPersonNametag.color = color;
    }

    public void UpdateColor(Color color) => StartCoroutine(UpdateColorCoroutine(color));
}