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

    protected IEnumerator SetText(string newText)
    {
        if (!string.IsNullOrEmpty(firstPersonTag.text))
        {
            while (firstPersonTag.text.Length > 0)
            {
                firstPersonTag.text = firstPersonTag.text.Substring(0, firstPersonTag.text.Length - 1);
                thirdPersonTag.text = thirdPersonTag.text.Substring(0, thirdPersonTag.text.Length - 1);
                yield return new WaitForSeconds(0.02f);
            }
        }
        
        char[] newTextArray = newText.ToCharArray();
        foreach (char c in newTextArray)
        {
            firstPersonTag.text += c;
            thirdPersonTag.text += c;
            yield return new WaitForSeconds(0.02f);
        }
    }
    
    public void AssignTagParents(Transform firstPersonParentNew, Transform thirdPersonParentNew) => (firstPersonParent, thirdPersonParent) = (firstPersonParentNew, thirdPersonParentNew);
}