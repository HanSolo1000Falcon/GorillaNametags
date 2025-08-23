using UnityEngine;

namespace GorillaNametags.Tags;

public class UserIDTag : TagBase
{
    private VRRig rig;

    private StatsTag statsTag;
    private FPSNametag fpsNametag;
    private AccountCreationDateTag accountCreationDateTag;

    protected override void Start()
    {
        AssignTagParents(transform, transform);
        localPosition = new Vector3(0f, 0.7f, 0f);
        base.Start();
        
        rig = GetComponent<VRRig>();

        statsTag = gameObject.AddComponent<StatsTag>();
        fpsNametag = gameObject.AddComponent<FPSNametag>();
        accountCreationDateTag = gameObject.AddComponent<AccountCreationDateTag>();
        
        statsTag.AssignTagParents(firstPersonTag.transform, thirdPersonTag.transform);
        fpsNametag.AssignTagParents(firstPersonTag.transform, thirdPersonTag.transform);
        accountCreationDateTag.AssignTagParents(firstPersonTag.transform, thirdPersonTag.transform);

        StartCoroutine(SetText(rig.OwningNetPlayer.UserId));
    }

    private void OnDestroy()
    {
        Destroy(firstPersonTag);
        Destroy(thirdPersonTag);
        
        Destroy(statsTag);
        Destroy(fpsNametag);
        Destroy(accountCreationDateTag);
    }

    private void Update()
    {
        Color targetColour = GetTargetColour();
        Color lerpedColour = Color.Lerp(firstPersonTag.color, targetColour, Time.deltaTime * 3f);
        UpdateColour(lerpedColour);

        firstPersonTag.transform.LookAt(Plugin.FirstPersonCamera);
        thirdPersonTag.transform.LookAt(Plugin.ThirdPersonCamera);

        firstPersonTag.transform.Rotate(0f, 180f, 0f);
        thirdPersonTag.transform.Rotate(0f, 180f, 0f);
    }
    
    private Color GetTargetColour()
    {
        if (rig.bodyRenderer.cosmeticBodyType == GorillaBodyType.Skeleton)
            return Color.green;

        switch (rig.setMatIndex)
        {
            case 1:
                return Color.red;

            case 2:
            case 11:
                return new Color(1f, 0.3288f, 0f, 1f);

            case 3:
            case 7:
                return Color.blue;

            case 12:
                return Color.green;

            default:
                return rig.playerColor;
        }
    }

    public void UpdateColour(Color colour)
    {
        if (firstPersonTag == null || thirdPersonTag == null)
            return;
        
        firstPersonTag.color = colour;
        thirdPersonTag.color = colour;
        
        fpsNametag.UpdateColour(colour);
        accountCreationDateTag.UpdateColour(colour);
    }
}