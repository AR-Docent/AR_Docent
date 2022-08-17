using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

public class ProductStatusUI : MonoBehaviour
{
    public bool Tracking { get; set; }

    [SerializeField]
    private Transform UIObj;

    private ARTrackedImageManager m_TrackedImageManager;

    private Image _pointerImage;
    private TextMeshProUGUI _text;
    
    //message
    private const string _trackedStr = "OK";
    private const string _untrackedStr = "Fit the artwork into a square";
    //color
    private Color _trackedColor = Color.green;
    private Color _untrackedColor = Color.grey;

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

        _pointerImage = UIObj.GetChild(0).GetComponent<Image>();
        _text = UIObj.GetChild(1).GetComponent<TextMeshProUGUI>();
    }

    void Start()
    {
        untracked();
    }

    private void OnChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        if (eventArgs.added.Count > 0)
        {
            tracked();
        }
        else
        {
            untracked();
        }
    }

    public void untracked()
    {
        //image and text visible;
        if (!_pointerImage.enabled)
            _pointerImage.enabled = true;
        if (!_text.enabled)
            _text.enabled = true;
        //change image and message
        _pointerImage.color = _untrackedColor;
        _text.text = _untrackedStr;
        Tracking = false;
    }

    public void tracked()
    {
        //change image and message
        _pointerImage.color = _trackedColor;
        _text.text = _trackedStr;
        Tracking = true;
        StartCoroutine(disablePointer());
    }

    IEnumerator disablePointer()
    {
        yield return new WaitForSecondsRealtime(1.0f);
        if (_pointerImage.enabled)
            _pointerImage.enabled = false;
        if (_text.enabled)
            _text.enabled = false;
    }
}
