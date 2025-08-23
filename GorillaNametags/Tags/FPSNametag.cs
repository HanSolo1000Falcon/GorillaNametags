using System.Collections;
using TMPro;
using UnityEngine;

namespace GorillaNametags.Tags;

public class FPSNametag : TagBase
{
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
            
            case < 72:
                tagColour = "yellow";
                break;
        }

        string text = $"{netPlayer.NickName} | <color={tagColour}>{fps} FPS</color>";
        
        firstPersonTag.text = text;
        thirdPersonTag.text = text;
    }

    protected override void Start()
    {
        localPosition = new Vector3(0f, 0.2f, 0f);
        base.Start();
        
        rig = GetComponent<VRRig>();
        netPlayer = rig.OwningNetPlayer;
    }

    public void UpdateColour(Color colour)
    {
        if (firstPersonTag == null || thirdPersonTag == null)
            return;
        
        firstPersonTag.color = colour;
        thirdPersonTag.color = colour;
    }
}