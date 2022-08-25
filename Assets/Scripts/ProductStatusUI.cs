using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ProductStatusUI : MonoBehaviour
{
    public static uint screenInObj = 0;

    [SerializeField]
    private ARTrackedImageManager m_ARTrackedImageManager;
    
    private Image _pointerImage;
    private TextMeshProUGUI _text;

    //message
    private const string _trackedStr = "OK";
    private const string _untrackedStr = "Fit the artwork into a square";

    private void Awake()
    {
        _pointerImage = transform.GetChild(0).GetComponent<Image>();
        _text = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        //get tracked image event
    }

    void OnEnable()
    {
        m_ARTrackedImageManager.trackedImagesChanged += Trace;
    }

    void OnDisable()
    {
        m_ARTrackedImageManager.trackedImagesChanged -= Trace;
    }

    void Start()
    {
        Untracked();
    }

    void Update()
    {
        if (screenInObj == 0)
            Untracked();
        else
            Tracked();
    }

    void TraceTarget(ARTrackedImage trackedImage)
    {
        if (trackedImage.trackingState == TrackingState.Tracking
            || trackedImage.trackingState == TrackingState.Limited)
        {
        }
        else
        {
        }
    }

    void Trace(ARTrackedImagesChangedEventArgs eventArg)
    {
        foreach (ARTrackedImage newImage in eventArg.added)
        {
        }
        foreach (ARTrackedImage updateImage in eventArg.added)
        {
        }
        foreach (ARTrackedImage removedImage in eventArg.added)
        {
        }
    }

    void Untracked()
    {
        //image and text visible;
        if (!_pointerImage.enabled)
            _pointerImage.enabled = true;
        if (!_text.enabled)
            _text.enabled = true;
        //change image and message
        _pointerImage.color = Color.gray;
        _text.text = _untrackedStr;
    }

    void Tracked()
    {
        //change image and message
        _pointerImage.color = Color.green;
        _text.text = _trackedStr;
        StartCoroutine(DisablePointer());
    }

    IEnumerator DisablePointer()
    {
        yield return new WaitForSecondsRealtime(1.0f);
        if (_pointerImage.enabled)
            _pointerImage.enabled = false;
        if (_text.enabled)
            _text.enabled = false;
    }
}
