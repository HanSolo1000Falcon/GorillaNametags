using TMPro;
using UnityEngine;

namespace GorillaNametags.Tags;

public class Nametag : MonoBehaviour
{
    public TextMeshPro firstPersonNametag;
    public TextMeshPro thirdPersonNametag;

    private NetPlayer netPlayer;
    
    public void UpdateColor(Color color)
    {
        if (firstPersonNametag == null)
            firstPersonNametag = Plugin.CreateTag("FirstPersonNametag", Plugin.FirstPersonLayerName, transform, new Vector3(0f, 0.7f, 0f));
        
        if (thirdPersonNametag == null)
            thirdPersonNametag = Plugin.CreateTag("ThirdPersonNametag", Plugin.ThirdPersonLayerName, transform, new Vector3(0f, 0.7f, 0f));
        
        firstPersonNametag.color = color;
        thirdPersonNametag.color = color;
    }

    private void Update()
    {
        firstPersonNametag.transform.LookAt(Plugin.firstPersonCamera);
        thirdPersonNametag.transform.LookAt(Plugin.thirdPersonCamera);
        
        firstPersonNametag.transform.Rotate(0f, 180f, 0f);
        thirdPersonNametag.transform.Rotate(0f, 180f, 0f);

        firstPersonNametag.text = netPlayer.NickName;
        thirdPersonNametag.text = netPlayer.NickName;
    }

    private void OnDestroy()
    {
        Plugin.nametags.Remove(GetComponent<VRRig>());
        Destroy(firstPersonNametag);
        Destroy(thirdPersonNametag);
    }

    private void Start()
    {
        VRRig rig = GetComponent<VRRig>();
        netPlayer = rig.OwningNetPlayer;
        Plugin.nametags[rig] = this;
    }
}