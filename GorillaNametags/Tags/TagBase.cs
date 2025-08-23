using System.Collections;
using TMPro;
using UnityEngine;

namespace GorillaNametags.Tags;

public class TagBase : MonoBehaviour
{
    public TextMeshPro firstPersonTag { get; private set; }
    public TextMeshPro thirdPersonTag { get; private set; }

    protected Transform firstPersonParent;
    protected Transform thirdPersonParent;

    protected Vector3 localPosition;
    
    protected virtual void Start()
    {
        firstPersonTag = Plugin.CreateTag($"FirstPerson{GetType().Name}", Plugin.FirstPersonLayerName, firstPersonParent, localPosition);
        thirdPersonTag = Plugin.CreateTag($"ThirdPerson{GetType().Name}", Plugin.ThirdPersonLayerName, thirdPersonParent, localPosition);

        Plugin.OnFontReloaded += UpdateFont;
    }

    private void UpdateFont(TMP_FontAsset fontAsset)
    {
        if (firstPersonTag != null)
            firstPersonTag.font = fontAsset;

        if (thirdPersonTag != null)
            thirdPersonTag.font = fontAsset;
    }
    
    public void AssignTagParents(Transform firstPersonParentNew, Transform thirdPersonParentNew) => (firstPersonParent, thirdPersonParent) = (firstPersonParentNew, thirdPersonParentNew);
}