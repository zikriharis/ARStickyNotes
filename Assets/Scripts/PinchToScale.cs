using UnityEngine;

public class PinchToScaleOnTouch : MonoBehaviour
{
    public float minScale = 0.2f;
    public float maxScale = 2.0f;

    private float initialDistance;
    private Vector3 initialScale;
    private bool isScaling = false;

    private Camera mainCamera;
    private Outline outline;

    void Start()
    {
        mainCamera = Camera.main;
        outline = GetComponent<Outline>();
    }

    void Update()
    {
        if (Input.touchCount == 2)
        {
            Touch t0 = Input.GetTouch(0);
            Touch t1 = Input.GetTouch(1);

            if (t1.phase == TouchPhase.Began)
            {
                if (RaycastToThisObject(t0.position) || RaycastToThisObject(t1.position))
                {
                    isScaling = true;
                    initialDistance = Vector2.Distance(t0.position, t1.position);
                    initialScale = transform.localScale;

                    if (outline != null) outline.EnableOutline();
                }
            }

            if (isScaling && (t0.phase == TouchPhase.Moved || t1.phase == TouchPhase.Moved))
            {
                float currentDistance = Vector2.Distance(t0.position, t1.position);
                if (Mathf.Approximately(initialDistance, 0)) return;

                float scaleFactor = currentDistance / initialDistance;

                // Keep Y constant and apply uniform scale factor to X and Z
                float newX = Mathf.Clamp(initialScale.x * scaleFactor, minScale, maxScale);
                float ratio = initialScale.z / initialScale.x;
                float newZ = newX * ratio;

                transform.localScale = new Vector3(newX, initialScale.y, newZ);
            }

        }
        else if (isScaling)
        {
            isScaling = false;
            if (outline != null) outline.DisableOutline();
        }
    }

    private bool RaycastToThisObject(Vector2 screenPosition)
    {
        Ray ray = mainCamera.ScreenPointToRay(screenPosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            return hit.transform == transform;
        }
        return false;
    }
}
