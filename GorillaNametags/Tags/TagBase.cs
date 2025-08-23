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
    }
    
    public void AssignTagParents(Transform firstPersonParentNew, Transform thirdPersonParentNew) => (firstPersonParent, thirdPersonParent) = (firstPersonParentNew, thirdPersonParentNew);
}