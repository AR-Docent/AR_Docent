using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class PlaneSpawnManager : MonoBehaviour
{
    public Vector3 planePos { get; set; }
    public Quaternion planeRot { get; set; }
    
    private ARPlaneManager m_ARPlaneManager;

    private void Awake()
    {
        m_ARPlaneManager = GetComponent<ARPlaneManager>();
    }

    private void OnEnable()
    {
        m_ARPlaneManager.planesChanged += OnChanged;
    }

    private void OnDisable()
    {
        m_ARPlaneManager.planesChanged -= OnChanged;
    }

    private void OnChanged(ARPlanesChangedEventArgs eventArgs)
    {
        foreach (ARPlane newPlane in eventArgs.added)
        {
            UpdatePlane(newPlane);
        }
        foreach (ARPlane updatePlane in eventArgs.updated)
        {
            UpdatePlane(updatePlane);
        }
        foreach (ARPlane removePlane in eventArgs.removed)
        {
            //최솟값 갱신
        }
    }

    private void UpdatePlane(ARPlane plane)
    {
        if (plane.trackingState == TrackingState.Tracking)
        {
            
        }
    }
}
