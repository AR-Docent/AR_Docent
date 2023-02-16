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

    void Awake()
    {
        trackedObj = new Dictionary<string, GameObject>();
        trackedUI = new Dictionary<string, GameObject>();
    }

    void Start()
    {
        foreach (var prod in DownloadSource.Instance.productBusLst.Values)
        {
            //initialize trackedObj
            GameObject obj = Instantiate(prefab);
            obj.GetComponent<GuidButton>().productName = prod.id.ToString();
            obj.SetActive(false);
            trackedObj.Add(prod.id.ToString(), obj);

            //initialize trackedUI
            GameObject UIObj = Instantiate(UIprefab, canvasTrans);
            UIObj.GetComponent<PointManager>().Name = prod.id.ToString();
            UIObj.SetActive(false);
            trackedUI.Add(prod.id.ToString(), UIObj);
        }
    }
}
