using UnityEngine;

public class GhostFader : MonoBehaviour
{
    [Range(0f, 1f)]
    public float fade = 0f;

    private Material ghostMaterial;

    void Start()
    {
        var renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            // Use instantiated material to avoid changing the original
            ghostMaterial = renderer.material;
        }
    }

    void Update()
    {
        if (ghostMaterial != null)
        {
            ghostMaterial.SetFloat("_Fade", fade);
        }
    }

    // Optional: Call this from Timeline using signals or a control script
    public void SetFade(float target)
    {
        fade = target;
    }
}