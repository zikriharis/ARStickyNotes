using UnityEngine;
using UnityEngine.EventSystems;

public class PinchToScaleAndTapToSpawn : MonoBehaviour
{
    public float minScale = 0.2f;
    public float maxScale = 2.0f;

    private float initialDistance;
    private Vector3 initialScale;
    private bool isScaling = false;

    private Camera mainCamera;
    private Outline outline;

    // Sticky Note Prefab to spawn
    public GameObject stickyNotePrefab;

    // Optional offset for sticky note above the board
    public float spawnOffset = 0.01f;

    // Tap cooldown variables
    private float lastHideTime = -10f;
    public float hideCooldown = 1.0f; // Increased cooldown

    // Tap-and-hold variables
    private bool isTouching = false;
    private float touchStartTime = 0f;
    public float spawnHoldDuration = 0.5f; // Hold at least 0.5s to spawn

    void Start()
    {
        mainCamera = Camera.main;
        outline = GetComponent<Outline>();
    }

    void Update()
    {
        // Pinch to scale logic (preserve inspector ratio of X and Z)
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

                // Compute the new X scale, clamp it
                float newX = Mathf.Clamp(initialScale.x * scaleFactor, minScale, maxScale);

                // Preserve the X/Z ratio from the inspector
                float xzRatio = initialScale.z / initialScale.x;
                float newZ = newX * xzRatio;

                // Y remains as set in inspector
                transform.localScale = new Vector3(newX, initialScale.y, newZ);
            }
        }
        else if (isScaling)
        {
            isScaling = false;
            if (outline != null) outline.DisableOutline();
        }

        // Tap-and-hold to spawn sticky note logic (single touch)
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

            // Block tap if just hid a sticky note or confirmed/cancelled popup
            if (Time.time - lastHideTime < hideCooldown)
                return;

            // Block if tap is on any UI (sticky note or otherwise)
            if (IsPointerOverUI(touch))
                return; // Tap is on UI, don't spawn on board

            if (touch.phase == TouchPhase.Began)
            {
                isTouching = true;
                touchStartTime = Time.time;
            }
            else if (touch.phase == TouchPhase.Ended && isTouching)
            {
                float heldTime = Time.time - touchStartTime;
                isTouching = false;

                if (heldTime >= spawnHoldDuration)
                {
                    Ray ray = mainCamera.ScreenPointToRay(touch.position);
                    if (Physics.Raycast(ray, out RaycastHit hit))
                    {
                        if (hit.transform == transform)
                        {
                            SpawnStickyNoteAtWorldPosition(hit.point, hit.normal);
                        }
                    }
                }
            }
        }
        else
        {
            isTouching = false;
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

    // UI filter to prevent interaction when pointer is over UI
    private bool IsPointerOverUI(Touch touch)
    {
        if (EventSystem.current != null)
        {
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
            return EventSystem.current.IsPointerOverGameObject(touch.fingerId);
#else
            return EventSystem.current.IsPointerOverGameObject();
#endif
        }
        return false;
    }

    // Spawns sticky note at the world position with a small offset above the board
    private void SpawnStickyNoteAtWorldPosition(Vector3 worldPosition, Vector3 surfaceNormal)
    {
        if (stickyNotePrefab == null)
        {
            Debug.LogWarning("stickyNotePrefab is not assigned!");
            return;
        }

        // Offset the spawn position slightly above the surface to avoid z-fighting
        Vector3 spawnPos = worldPosition + surfaceNormal.normalized * spawnOffset;

        // Instantiate and orient the sticky note facing away from the surface
        // Add 180 deg Y rotation if note spawns backward
        Quaternion rotation = Quaternion.LookRotation(surfaceNormal) * Quaternion.Euler(0, 180, 0);
        GameObject stickyNote = Instantiate(stickyNotePrefab, spawnPos, rotation);

        // Assign the boardScript reference to the new sticky note
        StickyNoteHideShow stickyScript = stickyNote.GetComponent<StickyNoteHideShow>();
        if (stickyScript != null)
        {
            stickyScript.boardScript = this;
        }

        // Optionally, parent the sticky note to the board for organization
        //stickyNote.transform.SetParent(transform);
    }

    // Call this method when hiding a sticky note or confirming/cancelling a popup
    public void OnStickyNoteHideOrPopup()
    {
        lastHideTime = Time.time;
    }
}