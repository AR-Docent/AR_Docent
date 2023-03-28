using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using UnityEditor.PackageManager.Requests;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkManager : MonoBehaviour
{
    [Serializable]
    private class Wrapper<T>
    {
        public T[] Items;
    }

    private T[] ArrayFromJson<T>(string json)
    {
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
        return wrapper.Items;
    }

    private T FromJson<T>(string jsonStr)
    {
        T ret = default;

        if (jsonStr == null)
        {
            return ret;
        }
        return JsonUtility.FromJson<T>(jsonStr);
    }

    private IEnumerable<T> IENumerableFromJson<T>(string jsonStr)
    {
        T[] retLst = default;

        if (jsonStr == null)
        {
            return retLst;
        }
        string str = "{ \"Items\":" + jsonStr + "}";
        return ArrayFromJson<T>(str);
    }

    public string GetRequestStr(string url)
    {
        string ret = default;
        try
        {
            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
                UnityWebRequestAsyncOperation oper = request.SendWebRequest();
                while (!oper.isDone) { }
                if (request.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError($"{request.error} : {request.url}");
                    throw new Exception($"{request.error} : {request.url}");
                }
                ret = request.downloadHandler.text;
            }
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
        return ret;
    }

    public Task<string> GetRequestStrAsync(string url)
    {
        try
        {
            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
                UnityWebRequestAsyncOperation oper = request.SendWebRequest();
                return Task<string>.Run(async () =>
                {
                    while (!oper.isDone)
                    {
                        await Task.Yield();
                    }
                    if (request.result != UnityWebRequest.Result.Success)
                    {
                        Debug.LogError($"{request.error} : {request.url}");
                        throw new Exception($"{request.error} : {request.url}");
                    }
                    return request.downloadHandler.text;
                });
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            return null;
        }
    }

    public IEnumerator GetRequestStrIEnumerator(string url)
    {
        yield return null;
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            UnityWebRequestAsyncOperation oper = request.SendWebRequest();
            while (!oper.isDone)
            {
                yield return null;
            }
            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"{request.error} : {request.url}");
                throw new Exception($"{request.error} : {request.url}");
            }
            yield return request.downloadHandler.text;
        }
    }

    public T GetDataFromJson<T>(string url)
    {
        return FromJson<T>(GetRequestStr(url));
    }

    public IEnumerable<T> GetDataListFromJson<T>(string url)
    {
        return IENumerableFromJson<T>(GetRequestStr(url));
    }

    public Task<T> GetDataFromJsonAsync<T>(string url)
    {
        //return FromJson<T>(GetRequestStrAsync(url).Result);
        IEnumerator task = GetRequestStrIEnumerator(url);
        StartCoroutine(task);
        return Task.Run(() => {
            while (task.MoveNext())
            {
                Task.Yield();
            }
            return FromJson<T>((string)task.Current);
        });
    }

    public Task<IEnumerable<T>> GetDataListFromJsonAsync<T>(string url)
    {
        IEnumerator task = GetRequestStrIEnumerator(url);
        try
        {
            //return IENumerableFromJson<T>(GetRequestStrAsync(url).Result);
            StartCoroutine(task);
            return Task.Run<IEnumerable<T>>(() =>
            {
                while (task.MoveNext())
                {
                    Task.Yield();
                }
                return IENumerableFromJson<T>((string)task.Current);
            });
        }
        catch (Exception e)
        {
            StopCoroutine(task);
            Debug.LogError(e);
            return null;
        }
    }

    public T GetDataFromJsonAsync_v2<T>(string url)
    {
        return FromJson<T>(GetRequestStrAsync(url).Result);
    }

    public IEnumerable<T> GetDataListFromJsonAsync_v2<T>(string url)
    {
        return IENumerableFromJson<T>(GetRequestStrAsync(url).Result);
    }

    public Texture2D GetTexture2D(string url)
    {
        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(url))
        {
            UnityWebRequestAsyncOperation oper = request.SendWebRequest();

            while (oper.isDone == false) { }
            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.Log($"{request.error} : {request.url}");
                throw new Exception($"{request.error} : {request.url}");
            }
            return DownloadHandlerTexture.GetContent(request);
        }
    }

    public async Task<Texture2D> GetTexture2DAsync(string url)
    {
        return await Task.Run<Texture2D>(() =>
        {
            using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(url))
            {
                UnityWebRequestAsyncOperation oper = request.SendWebRequest();

                while (oper.isDone == false)
                {
                    Task.Yield();
                }
                if (request.result != UnityWebRequest.Result.Success)
                {
                    Debug.Log($"{request.error} : {request.url}");
                    throw new Exception($"{request.error} : {request.url}");
                }
                return DownloadHandlerTexture.GetContent(request);
            }
        });
    }
}
