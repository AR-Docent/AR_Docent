using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARSubsystems;

public class ARObjManager : MonoBehaviour
{
    public Dictionary<string, GameObject> trackedObj { get; private set; }
    public Dictionary<string, GameObject> trackedUI { get; private set; }
    
    [SerializeField]
    private GameObject prefab;
    [SerializeField]
    private GameObject UIprefab;
    [SerializeField]
    private XRReferenceImageLibrary imageLib;

    void Awake()
    {
        trackedObj = new Dictionary<string, GameObject>();
        trackedUI = new Dictionary<string, GameObject>();
        foreach (var image in imageLib)
        {
            //initialize trackedObj
            GameObject obj = Instantiate(prefab);
            obj.GetComponent<GuidButton>().productName = image.name;
            obj.SetActive(false);
            trackedObj.Add(image.name, obj);

            //initialize trackedUI
            GameObject UIObj = Instantiate(UIprefab);
            UIObj.GetComponent<PointManager>().Name = image.name;
            UIObj.SetActive(false);
            trackedUI.Add(image.name, UIObj);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
