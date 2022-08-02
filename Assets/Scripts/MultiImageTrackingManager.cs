using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARTrackedImageManager))]
public class MultiImageTrackingManager : MonoBehaviour
{
    private ARTrackedImageManager m_TrackedImageManager;
    private Dictionary<string, GameObject> trackedObj;
    
    private GameObject towardObj = null;

    [SerializeField]
    private GameObject[] prefabs;
    [SerializeField]
    Debugger debugger;

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
        trackedObj = new Dictionary<string, GameObject>();

        foreach (GameObject obj in prefabs)
        {
            GameObject clone = Instantiate(obj);
            clone.name = obj.name;
            clone.SetActive(false);
            trackedObj.Add(clone.name, clone);
        }
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
        towardObj = trackedObj[trackedImage.referenceImage.name];

        if (trackedImage.trackingState == TrackingState.Tracking)
        {
            towardObj.transform.SetPositionAndRotation(trackedImage.transform.position, trackedImage.transform.rotation);
            debugger.status = Debugger.t_status.Tracking;
            towardObj.SetActive(true);
        }
        else if (trackedImage.trackingState == TrackingState.Limited)
        {
            debugger.status = Debugger.t_status.Limited;
        }
        else
        {
            debugger.status = Debugger.t_status.None;
            towardObj.SetActive(false);
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
            trackedObj[removedImage.referenceImage.name].SetActive(false);
        }
    }
}
