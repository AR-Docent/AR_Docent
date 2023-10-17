using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Networking;

using Debug = UnityEngine.Debug;
using System.Threading.Tasks;
using UnityEditor.PackageManager.Requests;
//using UnityEditor.PackageManager.Requests;


public class DownloadSource : Singleton<DownloadSource>
{
    public Dictionary<int, ProductBus> productBusLst { get; private set; }

    public bool complete { get; private set; }

    public Action<int> complete_func;

    protected DownloadSource() { }

    Dictionary<int, Texture2D> imageDict;

    NetworkManager nm;
    //Dictionary<int, AudioClip> audioDict;

    int completedFile;

    Stopwatch stopwatch;

    void Awake()
    {
        productBusLst = new Dictionary<int, ProductBus>();
        imageDict = new Dictionary<int, Texture2D>();
        //audioDict = new Dictionary<int, AudioClip>();
        nm = gameObject.AddComponent<NetworkManager>();

        stopwatch = new Stopwatch();

        complete = false;
        completedFile = 0;
    }

    // Start is called before the first frame update
    void Start()
    {
        stopwatch.Start();
        StartCoroutine(DownloadProductsAsync());
        //StartCoroutine(DownloadProducts());
    }

    private void OnEnable()
    {
        complete_func += CompleteAudio;
    }

    private void OnDisable()
    {
        complete_func -= CompleteAudio;
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

    void MakeProductBus(IEnumerable<Product> products)
    {
        foreach (Product prod in products)
        {
            ProductBus bus = new()
            {
                id = prod.Id,
                artist = prod.Name,
                productName = prod.Title,
                content = prod.Content,
                image = imageDict[prod.Id],
                img_width = prod.Image_width
            };

            productBusLst.Add(prod.Id, bus);
        }
    }

#if false
//---------------------------asyncOperation-------------------------------------
    void DownloadFromWebsite(string url)
    {

        try
        {
            UnityWebRequest request = UnityWebRequest.Get(url);
            UnityWebRequestAsyncOperation oper = request.SendWebRequest();

            oper.completed += (asyncOper) =>
            {
                if (request.result != UnityWebRequest.Result.Success)
                {
                    Debug.Log($"{request.error} : {request.url}");
                    request?.Dispose();
                    throw new WebException(request.error);
                }

                string jsonStr = "{ \"Items\":" + request.downloadHandler.text + "}";

                Debug.Log(jsonStr);
                products = MyFromJson<Product>(jsonStr);

                request?.Dispose();

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
            DownloadImage(product.Id);
        }

        while (completedFile < products.Length)
        {
            yield return new WaitForSeconds(0.016f);
        }
        MakeProductBus(products);

        complete = true;
        stopwatch.Stop();
        Debug.LogWarning($"download complete in {stopwatch.ElapsedMilliseconds}ms");
    }

    void DownloadImage(int id)
    {
        UnityWebRequest url_request = null;
        UnityWebRequest file_request = null;
        try
        {
            url_request = UnityWebRequest.Get($"https://ardocent.azurewebsites.net/api/Unity/GetImageById/{id}");
            UnityWebRequestAsyncOperation url_oper = url_request.SendWebRequest();

            url_oper.completed += (asyncOper) =>
            {
                if (url_request.result != UnityWebRequest.Result.Success)
                {
                    Debug.Log($"{url_request.error} : {url_request.url}");
                    throw new WebException(url_request.error);
                }

                string url = url_request.downloadHandler.text;

                file_request = UnityWebRequestTexture.GetTexture(url);
                UnityWebRequestAsyncOperation oper = file_request.SendWebRequest();

                oper.completed += (asyncOperation) =>
                {
                    if (file_request.result != UnityWebRequest.Result.Success)
                    {
                        Debug.Log($"{file_request.error} : {file_request.url}");
                        throw new WebException(file_request.error);
                    }

                    Texture2D texture = DownloadHandlerTexture.GetContent(file_request);

                    Monitor.Enter(imageDict);
                    imageDict.Add(id, texture);
                    Monitor.Exit(imageDict);

                    completedFile += 1;
                };
            };
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
        finally
        {
            url_request?.Dispose();
            file_request?.Dispose();
        }
    }

    void DownloadAudio(int id)
    {
        UnityWebRequest url_request = null;
        UnityWebRequest file_request = null;
        try
        {
            url_request = UnityWebRequest.Get($"https://ardocent.azurewebsites.net/api/Unity/GetAudioById/{id}");
            UnityWebRequestAsyncOperation url_oper = url_request.SendWebRequest();

            string url = url_request.downloadHandler.text;

            url_oper.completed += (asyncOper) =>
            {
                file_request = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.WAV);
                UnityWebRequestAsyncOperation oper = file_request.SendWebRequest();

                oper.completed += (asyncOperation) =>
                {
                    if (file_request.result != UnityWebRequest.Result.Success)
                    {
                        Debug.Log($"{file_request.error} : {file_request.url}");
                        throw new WebException(file_request.error);
                    }
                    //add
                    AudioClip audio = DownloadHandlerAudioClip.GetContent(file_request);

                    Monitor.Enter(audioDict);
                    audioDict.Add(id, audio);
                    Monitor.Exit(audioDict);
                };
            };
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
        finally
        {
            url_request?.Dispose();
            file_request?.Dispose();
        }
    }

#else
    //----------------------------coroutine-------------------------------------
    /*
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
        using (UnityWebRequest request = UnityWebRequest.Get("https://ardocent.azurewebsites.net/api/Unity"))
        {
            UnityWebRequestAsyncOperation oper = request.SendWebRequest();
            while (oper.isDone == false)
            {
                yield return new WaitForSeconds(0.1f);
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
                IEnumerator img_task = GetRemoteTexture(prod.Id);
                task_lst.Add(img_task);

                //run task
                StartCoroutine(img_task);
            }
            //wait
            while (TaskRunning(task_lst))
            {
                yield return new WaitForSeconds(0.1f);
            }

            //free task_lst
            task_lst.Clear();
            task_lst = null;

            Debug.Log("download end");
            //make product bus
            MakeProductBus(products);
            complete = true;

            stopwatch.Stop();
            Debug.LogWarning($"download complete in {stopwatch.ElapsedMilliseconds}ms");
        }
    }

    //순차적
    IEnumerator GetRemoteTexture(int id)
    {
        string url;

        using (UnityWebRequest request = UnityWebRequest.Get($"https://ardocent.azurewebsites.net/api/Unity/GetImageById/{id}"))
        {
            UnityWebRequestAsyncOperation oper = request.SendWebRequest();
            while (!oper.isDone)
            {
                yield return new WaitForSeconds(0.1f);
            }
            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.Log($"{request.error} : {request.url}");
                yield break;
            }
            url = request.downloadHandler.text;
        }

        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(url))
        {
            UnityWebRequestAsyncOperation oper = request.SendWebRequest();

            while (oper.isDone == false)
            {
                yield return new WaitForSeconds(0.5f);
            }
            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.Log($"{request.error} : {request.url}");
                yield break;
            }
            //add
            imageDict.Add(id, DownloadHandlerTexture.GetContent(request));
        }
    }
    */

    //필수적이지 않.
    public IEnumerator GetRemoteAudio(int id)
    {
        string url;
        AudioClip audio = null;

        using (UnityWebRequest request = UnityWebRequest.Get($"https://ardocent.azurewebsites.net/api/Unity/GetAudioById/{id}"))
        {
            UnityWebRequestAsyncOperation oper = request.SendWebRequest();
            while (!oper.isDone)
            {
                yield return new WaitForSeconds(0.1f);
            }
            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.Log($"{request.error} : {request.url}");
                yield break;
            }
            url = request.downloadHandler.text;
        }

        using (UnityWebRequest request = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.WAV))
        {
            UnityWebRequestAsyncOperation oper = request.SendWebRequest();

            while (oper.isDone == false)
            {
                yield return new WaitForSeconds(1f);
            }
            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.Log($"{request.error} : {request.url}");
                yield break;
            }
            //add
            audio = DownloadHandlerAudioClip.GetContent(request);
            //audioDict.Add(id, audio);
            productBusLst[id].audio = audio;

            complete_func(id);
        }
    }

    void CompleteAudio(int id)
    {
        Debug.Log("audio download task complete");
    }

    IEnumerator DownloadProductsAsync()
    {
        Debug.Log("download start");

        Task<IEnumerable<Product>> t = nm.GetDataListFromJsonTask<Product>("http://ardocent.azurewebsites.net/api/Unity");

        //wait
        while (!t.IsCompleted)
            yield return null;

        if (!t.IsCompletedSuccessfully)
        {
            Debug.LogError("completed badly" + t.Result);
            yield break;
        }

        IEnumerable<Product> products = t.Result;

        List<Task<Texture2D>> t_lst = new();
        List<int> t_idx = new();
        foreach (Product prod in products)
        {
            t_lst.Add(nm.GetTextureTask($"https://ardocent.azurewebsites.net/api/Unity/GetImageById/{prod.Id}"));
            t_idx.Add(prod.Id);
        }

        Task adder = Task.Run(() =>
        {
            lock (imageDict)
            {
                Task.WaitAll(t_lst.ToArray());
                for (int i = 0; i < t_lst.Count; ++i)
                    imageDict.Add(t_idx[i], t_lst[i].Result);
            }
            return Task.CompletedTask;
        });

        while (!adder.IsCompleted)
            yield return new WaitForSeconds(0.5f);

        Debug.Log("download end");
        //make product bus
        MakeProductBus(products);
        complete = true;

        stopwatch.Stop();
        Debug.LogWarning($"download complete in {stopwatch.ElapsedMilliseconds}ms");
    }

#endif
}
