using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SkinnedMeshRenderer))]
public class ClothFix : MonoBehaviour
{
    public Vector3 boundsCenter = Vector3.zero;
    public Vector3 boundsSize = new Vector3(10f, 10f, 10f); // Make it big

    private IEnumerator Start()
    {
        yield return null; // wait 1 frame so Unity doesn't override our bounds
        var smr = GetComponent<SkinnedMeshRenderer>();
        smr.localBounds = new Bounds(boundsCenter, boundsSize);
        Debug.Log("✅ Cloth bounds set manually");
    }
}