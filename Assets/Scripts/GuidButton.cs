using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuidButton : MonoBehaviour
{
    public int productId { get; set; }

    public bool Tracking { get; set; } = false;

    public bool Selecting { get; private set; } = false;

    public bool DocentArive { get; set; } = false;

    public ProductBus bus { get; private set; } = null;

    public void OnClick()
    {
        Selecting = !Selecting;
    }

    public void OnEnable()
    {
        DownloadSource.Instance.complete_func += GetData;
    }

    public void OnDisable()
    {
        DownloadSource.Instance.complete_func -= GetData;
    }

    public void LoadData()
    {
        if (bus != null)
            return;
        StartCoroutine(DownloadSource.Instance.GetRemoteAudio(productId));
    }

    void GetData(int id)
    {
        bus = DownloadSource.Instance.productBusLst[id];
    }
}
