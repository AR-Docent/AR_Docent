using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using Unity.Jobs;

public class RefImageRuntime : MonoBehaviour
{
    ARTrackedImageManager manager;
    XRReferenceImageLibrary referenceImages;

    public bool completed { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        manager = GetComponent<ARTrackedImageManager>();
        StartCoroutine(DownloadProducts()); //after ar session init
    }

    bool TaskRunning(List<IEnumerator> lst)
    {
        bool result = true;

        foreach (IEnumerator iter in lst)
        {
            bool i = !(iter.MoveNext());
            result &= i;
        }
        return !result;
    }

    bool StateRunning(List<AddReferenceImageJobState> lst)
    {
        bool state = true;

        foreach (AddReferenceImageJobState i in lst)
        {
            state &= i.jobHandle.IsCompleted;
        }
        return !state;
    }

    IEnumerator RefInitialized()
    {
        Debug.Log("make runtime library start");
        
        MutableRuntimeReferenceImageLibrary mutableLibrary = manager.referenceLibrary as MutableRuntimeReferenceImageLibrary;
        List<AddReferenceImageJobState> states = new();

        //add image to mutableLibrary
        foreach (ProductBus bus in DownloadSource.Instance.productBusLst.Values)
        {
            Debug.Log($"add start");
            AddReferenceImageJobState imgJobState =
                mutableLibrary.ScheduleAddImageWithValidationJob(bus.image, bus.id.ToString(), 1f);

            states.Add(imgJobState);
        }

        while (StateRunning(states))
        {
            yield return new WaitForSeconds(0.3f);
        }

        Debug.Log("make runtime library end");
    }

    IEnumerator DownloadProducts()
    {
        if (ARSession.state == ARSessionState.None
            || ARSession.state == ARSessionState.CheckingAvailability)
            yield return ARSession.CheckAvailability();

        if (ARSession.state == ARSessionState.Unsupported)
        {
            // unsupported devices
            Debug.Log("unsupported device");
            yield break;
        }

        if (!manager.descriptor.supportsMutableLibrary)
        {
            Debug.Log("unsupport mutable library");
            yield break;
        }
        //start

        List<IEnumerator> task_lst = new List<IEnumerator>();

        manager.referenceLibrary = manager.CreateRuntimeLibrary(referenceImages);
        //make xr reference
        IEnumerator ref_task = RefInitialized();

        task_lst.Add(ref_task);
        StartCoroutine(ref_task);
        //wait
        while (TaskRunning(task_lst))
        {
            yield return new WaitForSeconds(0.3f);
        }


        manager.enabled = true;

        Debug.Log("ref upload end");
    }
}
