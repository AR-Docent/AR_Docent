using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARTrackedImageManager))]
public class MultiImageTrackingManager : MonoBehaviour
{
    [SerializeField]
    private ARObjPool objPool;
    [SerializeField]
    [Range(0f, 1f)]
    private float aboveDistance;
    
    private ARTrackedImageManager m_TrackedImageManager;

    void OnEnable()
    {
        m_TrackedImageManager.trackedImagesChanged += OnChanged;
    }

    void OnDisable()
    {
        m_TrackedImageManager.trackedImagesChanged -= OnChanged;
    }

    void Awake()
    {
        m_TrackedImageManager = GetComponent<ARTrackedImageManager>();
    }
    
    void ListAllImage()
    {
        Debug.Log($"There are {m_TrackedImageManager.trackables.count} images being tracked. ");
        foreach (ARTrackedImage trackedImage in m_TrackedImageManager.trackables)
        {
            Debug.Log($"Image: {trackedImage.referenceImage.name} is at " + $"{trackedImage.transform.position}");
        }
    }


    void UpdateImage(ARTrackedImage trackedImage)
    {
        GameObject obj = objPool?.trackedObj[trackedImage.referenceImage.name];
        Debugger debugger = obj.GetComponent<Debugger>();

        if (trackedImage.trackingState == TrackingState.Tracking)
        {
            obj.transform.SetPositionAndRotation(trackedImage.transform.position + Vector3.up * aboveDistance, Quaternion.identity);
            debugger.status = Debugger.t_status.Tracking;
            obj.SetActive(true);
        }
        else if (trackedImage.trackingState == TrackingState.Limited)
        {
            debugger.status = Debugger.t_status.Limited;
        }
        else
        {
            debugger.status = Debugger.t_status.None;
            obj.SetActive(false);
        }
    }

    private void OnChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach (ARTrackedImage newImage in eventArgs.added)
        {
            //Handle added event
            UpdateImage(newImage);
        }
        foreach (ARTrackedImage updateImage in eventArgs.updated)
        {
            //Handle updated event
            UpdateImage(updateImage);
        }
        foreach (ARTrackedImage removedImage in eventArgs.removed)
        {
            //Handle removed event
            objPool?.trackedObj[removedImage.referenceImage.name].SetActive(false);
        }
    }
}
