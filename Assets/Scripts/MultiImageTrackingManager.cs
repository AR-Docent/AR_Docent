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
    private List<(Vector3, Quaternion)> toward;

    [SerializeField]
    private GameObject[] prefabs;
    [SerializeField]
    [Range(0.0f, 1.0f)]
    private float movindSpeed = 0.1f;
    [SerializeField]
    [Range(0.0f, 1.0f)]
    private float rotateSpeed = 0.1f;

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


    void UpdateImage(ARTrackedImage trackedImage)
    {
        towardObj = trackedObj[trackedImage.referenceImage.name];

        //ListAllImage();
        if (trackedImage.trackingState != TrackingState.None)
        {
            //obj.transform.SetPositionAndRotation(trackedImage.transform.position, trackedImage.transform.rotation);
            if (trackedImage.trackingState == TrackingState.Tracking)
            {
                toward.Add((trackedImage.transform.position, trackedImage.transform.rotation));
                towardObj.SetActive(true);
            }
        }
        else
        {
            Debug.Log("img lost.");
            if (toward.Count == 0)
            {
                towardObj.SetActive(false);
                Debug.Log("toward clean");
            }
        }
    }

    void FixedUpdate()
    {
        if (toward.Count > 0 && towardObj != null)
        {
            //move toward image
            bool r, p;

            r = false;
            p = false;
            //position
            if (Vector3.Distance(towardObj.transform.position, toward[0].Item1) < 0.01f)
            {
                towardObj.transform.position = Vector3.Lerp(towardObj.transform.position, toward[0].Item1, 1f);
                p = true;
            }
            else
                towardObj.transform.position = Vector3.Lerp(towardObj.transform.position, toward[0].Item1, movindSpeed);
            //rotation
            if (Quaternion.Angle(towardObj.transform.rotation, toward[0].Item2) < 0.01f)
            {
                towardObj.transform.rotation = Quaternion.Lerp(towardObj.transform.rotation, toward[0].Item2, 1f);
                r = true;
            }
            else
                towardObj.transform.rotation = Quaternion.Lerp(towardObj.transform.rotation, toward[0].Item2, rotateSpeed);
            if (r && p)
                toward.RemoveAt(0);
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
