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

        toward = new List<(Vector3, Quaternion)>();

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

    private List<(Vector3, Quaternion)> toward;

    void UpdateImage(ARTrackedImage trackedImage)
    {
        GameObject obj = trackedObj[trackedImage.referenceImage.name];

        //ListAllImage();
        if (trackedImage.trackingState != TrackingState.None)
        {
            //obj.transform.SetPositionAndRotation(trackedImage.transform.position, trackedImage.transform.rotation);
            if (trackedImage.trackingState == TrackingState.Tracking)
            {
                toward.Add((trackedImage.transform.position, trackedImage.transform.rotation));
                obj.SetActive(true);
            }
            if (toward.Count > 0)
            {
                bool r, p;

                r = false;
                p = false;
                if (Vector3.Distance(obj.transform.position, toward[0].Item1) < 0.01f)
                {
                    obj.transform.position = Vector3.Lerp(obj.transform.position, toward[0].Item1, 1f);
                    p = true;
                }
                else
                    obj.transform.position = Vector3.Lerp(obj.transform.position, toward[0].Item1, 0.1f);
                if (Quaternion.Angle(obj.transform.rotation, toward[0].Item2) < 0.01f)
                {
                    obj.transform.rotation = Quaternion.Lerp(obj.transform.rotation, toward[0].Item2, 1f);
                    r = true;
                }
                else
                    obj.transform.rotation = Quaternion.Lerp(obj.transform.rotation, toward[0].Item2, 0.1f);
                if (r && p)
                    toward.RemoveAt(0);
            }
        }
        else
        {
            toward.Clear();
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
