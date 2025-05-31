using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class Outline : MonoBehaviour
{
    public Color outlineColor = Color.yellow;
    public float outlineWidth = 1.05f;
    private Material originalMat;
    private Material outlineMat;

    void Start()
    {
        var renderer = GetComponent<Renderer>();
        originalMat = renderer.material;

        // Clone material
        outlineMat = new Material(Shader.Find("Outlined/Silhouetted Diffuse"));
        outlineMat.color = outlineColor;

        DisableOutline();
    }

    public void EnableOutline()
    {
        GetComponent<Renderer>().material = outlineMat;
        transform.localScale *= outlineWidth;
    }

    public void DisableOutline()
    {
        GetComponent<Renderer>().material = originalMat;
        transform.localScale /= outlineWidth;
    }
}
