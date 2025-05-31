using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

[RequireComponent(typeof(ARRaycastManager))]
public class PlacementWithManyController : MonoBehaviour
{

    [SerializeField]
    private GameObject placedPrefabs;

    [SerializeField]
    private int maxNumberOfBoards = 1;

    private List<GameObject> addedInstances = new List<GameObject>();

    private Vector2 touchPosition = default;

    private ARRaycastManager arRaycastManager;

    void Awake() 
    {
        arRaycastManager = GetComponent<ARRaycastManager>();
    }
 
    void Update()
    {        
        if(Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if(touch.phase == TouchPhase.Began)
            {
                touchPosition = touch.position;

                if(arRaycastManager.Raycast(touchPosition, hits, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinPolygon))
                {
                    var hitPose = hits[0].pose;
                    if(addedInstances.Count < maxNumberOfBoards)
                    {
                        GameObject addedPrefab = Instantiate(placedPrefabs, hitPose.position, hitPose.rotation);
                        addedInstances.Add(addedPrefab);
                    }
                }
            }

            if(touch.phase == TouchPhase.Moved)
            {
                touchPosition = touch.position;
                
                if(arRaycastManager.Raycast(touchPosition, hits, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinPolygon))
                {
                    Pose hitPose = hits[0].pose;
                    if(addedInstances.Count > 0)
                    {
                       GameObject lastAdded = addedInstances[addedInstances.Count - 1];
                       lastAdded.transform.position = hitPose.position;
                       lastAdded.transform.rotation = hitPose.rotation;
                    }
                }
            }
        }

    }


    static List<ARRaycastHit> hits = new List<ARRaycastHit>();
}
