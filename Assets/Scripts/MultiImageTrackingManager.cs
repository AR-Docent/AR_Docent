using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARTrackedImageManager))]
public class MultiImageTrackingManager : MonoBehaviour
{
    ARTrackedImageManager m_TrackedImageManager;
    Dictionary<string, GameObject> trackedObj;

    [SerializeField]
    GameObject[] prefabs;

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
        string name = trackedImage.referenceImage.name;
        GameObject obj = trackedObj[name];

        //ListAllImage();
        if (trackedImage.trackingState == TrackingState.Tracking)
        {
            obj.transform.SetPositionAndRotation(trackedImage.transform.position, trackedImage.transform.rotation);
            obj.SetActive(true);
        }
        else
        {
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
            trackedObj[removedImage.referenceImage.name].SetActive(false);
        }
    }
}
