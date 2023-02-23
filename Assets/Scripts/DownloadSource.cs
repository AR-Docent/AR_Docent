using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Networking;

using Debug = UnityEngine.Debug;


public class DownloadSource : Singleton<DownloadSource>
{
    public Dictionary<int, ProductBus> productBusLst { get; private set; }

    public bool complete { get; private set; }

    protected DownloadSource() { }

    Dictionary<string, Texture2D> imageDict;
    Dictionary<string, AudioClip> audioDict;

    Product[] products;
    int completedFile;

    Stopwatch stopwatch;

    void Awake()
    {
        productBusLst = new Dictionary<int, ProductBus>();
        imageDict = new Dictionary<string, Texture2D>();
        audioDict = new Dictionary<string, AudioClip>();
        stopwatch = new Stopwatch();

        complete = false;
        completedFile = 0;
    }

    // Start is called before the first frame update
    void Start()
    {
        stopwatch.Start();
#if true
        DownloadFromWebsite("https://ardocent.azurewebsites.net/api/Unity");
#else
        StartCoroutine(DownloadProducts());
#endif
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

    void MakeProductBus(Product[] products)
    {
        foreach (Product prod in products)
        {
            ProductBus bus = new()
            {
                id = prod.Id,
                artist = prod.Name,
                productName = prod.Title,
                content = prod.Content,
                image = imageDict[prod.Image_name],
                audio = audioDict[prod.Audio_name]
            };

            productBusLst.Add(prod.Id, bus);
        }
    }

#if true

    void DownloadFromWebsite(string url)
    {
        try
        {
            UnityWebRequest request = UnityWebRequest.Get(url);
            UnityWebRequestAsyncOperation oper = request.SendWebRequest();

            oper.completed += (asyncOperation) =>
            {
                if (request.result != UnityWebRequest.Result.Success)
                {
                    Debug.Log($"{request.error} : {request.url}");
                    throw new WebException(request.error);
                }

                string jsonStr = "{ \"Items\":" + request.downloadHandler.text + "}";
                request.Dispose();

                Debug.Log(jsonStr);
                products = MyFromJson<Product>(jsonStr);

                //next step
                StartCoroutine(DownloadFiles(products));
            };
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }

    IEnumerator DownloadFiles(Product[] products)
    {
        foreach (Product product in products)
        {
            DownloadImage(product.Image_name, product.Image_url);
            DownloadAudio(product.Audio_name, product.Audio_url);
        }

        while (completedFile < products.Length * 2)
        {
            yield return new WaitForSeconds(0.016f);
        }
        MakeProductBus(products);

        complete = true;
        stopwatch.Stop();
        Debug.LogWarning($"download complete in {stopwatch.ElapsedMilliseconds}ms");
    }

    void DownloadImage(string name, string url)
    {
        try
        {
            UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
            UnityWebRequestAsyncOperation oper = request.SendWebRequest();

            oper.completed += (asyncOperation) =>
            {
                if (request.result != UnityWebRequest.Result.Success)
                {
                    Debug.Log($"{request.error} : {request.url}");
                    throw new WebException(request.error);
                }

                Texture2D texture = DownloadHandlerTexture.GetContent(request);

                request.Dispose();

                Monitor.Enter(imageDict);
                imageDict.Add(name, texture);
                Monitor.Exit(imageDict);

                completedFile += 1;
            };
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }

    void DownloadAudio(string name, string url)
    {
        try
        {
            UnityWebRequest request = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.WAV);
            UnityWebRequestAsyncOperation oper = request.SendWebRequest();

            oper.completed += (asyncOperation) =>
            {
                if (request.result != UnityWebRequest.Result.Success)
                {
                    Debug.Log($"{request.error} : {request.url}");
                    throw new WebException(request.error);
                }
                //add
                AudioClip audio = DownloadHandlerAudioClip.GetContent(request);

                request.Dispose();

                Monitor.Enter(audioDict);
                audioDict.Add(name, audio);
                Monitor.Exit(audioDict);

                completedFile += 1;
            };
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }

#else
    //----------------------------past------------------------------------

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

    IEnumerator DownloadProducts()
    {
        Debug.Log("download start");
        using UnityWebRequest request = UnityWebRequest.Get("https://ardocent.azurewebsites.net/api/Unity");
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
        //serialize: tojson
        string jsonStr = "{ \"Items\":" + request.downloadHandler.text + "}";
        Debug.Log(jsonStr);

        //save in products
        Product[] products = MyFromJson<Product>(jsonStr);

        List<IEnumerator> task_lst = new List<IEnumerator>();

        foreach (Product prod in products)
        {
            IEnumerator img_task = GetRemoteTexture(prod.Image_name, prod.Image_url);
            IEnumerator audio_task = GetRemoteAudio(prod.Audio_name, prod.Audio_url);

            task_lst.Add(img_task);
            task_lst.Add(audio_task);

            //run task
            StartCoroutine(img_task);
            StartCoroutine(audio_task);
        }
        //wait
        while (TaskRunning(task_lst))
        {
            yield return new WaitForSeconds(0.001f);
        }

        task_lst.Clear();
        task_lst = null;

        Debug.Log("download end");
        //make product bus
        MakeProductBus(products);

        complete = true;

        stopwatch.Stop();
        Debug.LogWarning($"download complete in {stopwatch.ElapsedMilliseconds}ms");
    }

    //순차적
    IEnumerator GetRemoteTexture(string name, string url)
    {
        using UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        UnityWebRequestAsyncOperation oper = request.SendWebRequest();

        while (oper.isDone == false)
        {
            yield return new WaitForSeconds(0.016f);
        }
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.Log($"{request.error} : {request.url}");
            yield break;
        }
        //add
        imageDict.Add(name, DownloadHandlerTexture.GetContent(request));
    }

    //필수적이지 않.
    IEnumerator GetRemoteAudio(string name, string url)
    {
        AudioClip audio = null;

        using UnityWebRequest request = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.WAV);
        UnityWebRequestAsyncOperation oper = request.SendWebRequest();

        while (oper.isDone == false)
        {
            yield return new WaitForSeconds(0.016f);
        }
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.Log($"{request.error} : {request.url}");
            yield break;
        }
        //add
        audio = DownloadHandlerAudioClip.GetContent(request);
        audioDict.Add(name, audio);
    }
#endif
}
