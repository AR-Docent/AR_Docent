using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARSubsystems;

public class ARObjPool : MonoBehaviour
{
    public Dictionary<string, GameObject> trackedObj { get; private set; }
    public Dictionary<string, GameObject> trackedUI { get; private set; }
    [SerializeField]
    private Transform canvasTrans;
    
    [SerializeField]
    private GameObject prefab;
    [SerializeField]
    private GameObject UIprefab;
    [SerializeField]
    private XRReferenceImageLibrary imageLib;

    private RaycastClickEvent clickEvent;

    void Awake()
    {
        clickEvent = GetComponent<RaycastClickEvent>();

        trackedObj = new Dictionary<string, GameObject>();
        trackedUI = new Dictionary<string, GameObject>();
        foreach (var image in imageLib)
        {
            //initialize trackedObj
            GameObject obj = Instantiate(prefab);
            obj.GetComponent<GuidButton>().productName = image.name;
            obj.GetComponent<Debugger>().click = clickEvent;
            obj.SetActive(false);
            trackedObj.Add(image.name, obj);

            //initialize trackedUI
            GameObject UIObj = Instantiate(UIprefab, canvasTrans);
            UIObj.GetComponent<PointManager>().Name = image.name;
            UIObj.SetActive(false);
            trackedUI.Add(image.name, UIObj);
        }
    }
}
