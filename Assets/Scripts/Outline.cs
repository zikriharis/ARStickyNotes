using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class Outline : MonoBehaviour
{
    public Material outlineMaterial; // Assign in Inspector
    public float outlineWidth = 1.05f;

    private Material originalMat;
    private Renderer rend;

    void Start()
    {
        rend = GetComponent<Renderer>();
        originalMat = rend.material;

        DisableOutline();
    }

    public void EnableOutline()
    {
        if (outlineMaterial != null)
        {
            rend.material = outlineMaterial;
            transform.localScale *= outlineWidth;
        }
    }

    public void DisableOutline()
    {
        rend.material = originalMat;
        transform.localScale /= outlineWidth;
    }
}