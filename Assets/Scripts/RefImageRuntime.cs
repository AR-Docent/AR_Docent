using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using UnityEditor.ShaderGraph.Serialization;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class RefImageRuntime : MonoBehaviour
{
    ARTrackedImageManager manager;
    RuntimeReferenceImageLibrary runtimeLibrary;

    public List<string> image_names;
    
    public Dictionary<string, Texture2D> imageDict; 
    public Dictionary<string, AudioClip> audioDict;

    private void Awake()
    {
        image_names = new List<string>();
        imageDict = new Dictionary<string, Texture2D>();
    }

    // Start is called before the first frame update
    [Obsolete]
    void Start()
    {
        //manager = GetComponent<ARTrackedImageManager>();
        //runtimeLibrary = manager.CreateRuntimeLibrary();
        //StartCoroutine(DownloadProducts());
        StartCoroutine(DownloadProducts());
    }

    public void Initialized()
    {
        if (runtimeLibrary is MutableRuntimeReferenceImageLibrary mutableLibrary)
        {
            //add image to mutableLibrary
            //mutableLibrary.ScheduleAddImageJob();
        }
    }

    IEnumerator DownloadProducts()
    {
        Debug.Log("download start");
        using UnityWebRequest request = UnityWebRequest.Get("https://ardocent.azurewebsites.net/api/Unity");
        UnityWebRequestAsyncOperation oper = request.SendWebRequest();
        while (oper.isDone == false)
        {
            Debug.Log("loading");
            yield return null;
        }
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.Log($"{request.error} : {request.url}");
            yield break;
        }
        //serialize: tojson
        //string jsonStr = "{ \"Items\":" + request.downloadHandler.text + "}";
        string jsonStr = "{ \"Items\":" + UnityWebRequest.UnEscapeURL(request.downloadHandler.text) + "}";
        Debug.Log(jsonStr);
        Product[] products = MyFromJson<Product>(jsonStr);

        Debug.Log(DateTime.UtcNow);

        foreach (Product prod in products)
        {
            StartCoroutine(GetRemoteTexture(prod.image_name, prod.image_url));
            //StartCoroutine(GetRemoteAudio(prod.audio_name, prod.audio_url));
        }

        Debug.Log("download end");
    }

    [Serializable]
    private class Wrapper<T>
    {
        public T[] Items;
    }

    private T[] MyFromJson<T>(string json)
    {
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
        return wrapper.Items;
    }

    IEnumerator GetRemoteTexture(string name, string url)
    {
        using UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        UnityWebRequestAsyncOperation oper = request.SendWebRequest();

        while (oper.isDone == false)
        {
            yield return null;
        }
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.Log($"{request.error} : {request.url}");
            yield break;
        }
        //add
        image_names.Add(name);
        imageDict.Add(name, DownloadHandlerTexture.GetContent(request));
    }

    IEnumerator GetRemoteAudio(string name, string url)
    {
        using UnityWebRequest request = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.WAV);
        UnityWebRequestAsyncOperation oper = request.SendWebRequest();

        while (oper.isDone == false)
        {
            yield return null;
        }
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.Log($"{request.error} : {request.url}");
            yield break;
        }
        //add
        audioDict.Add(name, DownloadHandlerAudioClip.GetContent(request));
    }
}
