using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections.Generic;

public class ProductStatusUI : MonoBehaviour
{
    public static uint screenInObj = 0;

    [SerializeField]
    private ARObjPool objPool;
    [SerializeField]
    private ARTrackedImageManager m_ARTrackedImageManager;
    [SerializeField]
    private RectTransform screenRect;
    
    private Image _pointerImage;
    private TextMeshProUGUI _text;
    
    private float _canvas_x;
    private float _canvas_y;

    private List<string> _imageName = null;

    //message
    private const string _trackedStr = "OK";
    private const string _untrackedStr = "Fit the artwork into a square";

    private void Awake()
    {
        _pointerImage = transform.GetChild(0).GetComponent<Image>();
        _text = transform.GetChild(1).GetComponent<TextMeshProUGUI>();

        _canvas_x = screenRect.rect.width;
        _canvas_y = screenRect.rect.height;

        _imageName = new List<string>();

        Debug.Log(_canvas_x);
        Debug.Log(_canvas_y);
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
        //move UI
        for (int i = 0; i < _imageName.Count; ++i)
        {
            MoveUI(_imageName[i]);
        }
    }

    void MoveUI(string name)
    {
        //Set UI prefab active.
        GameObject obj = objPool?.trackedUI[name];
        RaycastScreenStatus r_status = objPool.trackedObj[name].GetComponent<RaycastScreenStatus>();
        PointManager p_manager = objPool.trackedUI[name].GetComponent<PointManager>();
        if (r_status.ScreenIn)
        {
            RectTransform r_trans = obj.GetComponent<RectTransform>();
            r_trans.position = new Vector3(r_status.viewPort.x * _canvas_x, r_status.viewPort.y * _canvas_y, 0f);
            p_manager.UIScale= 1f / (2f * Vector3.Distance(objPool.trackedObj[name].transform.position, Camera.main.transform.position));
            obj.SetActive(true);
        }
        else
        {
            obj.SetActive(false);
        }

    }

    void Trace(ARTrackedImagesChangedEventArgs eventArg)
    {
        foreach (var newImage in eventArg.added)
        {
            _imageName.Add(newImage.referenceImage.name);
        }
        foreach (var removedImage in eventArg.removed)
        {
            _imageName.Remove(removedImage.referenceImage.name);
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
