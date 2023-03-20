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

public class NetworkManager
{
    [Serializable]
    private class Wrapper<T>
    {
        public T[] Items;
    }

    private static T[] ArrayFromJson<T>(string json)
    {
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
        return wrapper.Items;
    }

    private static T FromJson<T>(string jsonStr)
    {
        T ret = default;

        if (jsonStr == null)
        {
            return ret;
        }
        return JsonUtility.FromJson<T>(jsonStr);
    }

    private static IEnumerable<T> IENumerableFromJson<T>(string jsonStr)
    {
        T[] retLst = default;

        if (jsonStr == null)
        {
            return retLst;
        }
        string str = "{ \"Items\":" + jsonStr + "}";
        return ArrayFromJson<T>(str);
    }

    public static string GetRequestStr(string url)
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

    public static Task<string> GetRequestStrAsync(string url)
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

    public static T GetDataFromJson<T>(string url)
    {
        return FromJson<T>(GetRequestStr(url));
    }

    public static IEnumerable<T> GetDataListFromJson<T>(string url)
    {
        return IENumerableFromJson<T>(GetRequestStr(url));
    }

    public static async Task<T> GetDataFromJsonAsync<T>(string url)
    {
        return FromJson<T>(await GetRequestStrAsync(url));
    }

    public static async Task<IEnumerable<T>> GetDataListFromJsonAsync<T>(string url)
    {
        return IENumerableFromJson<T>(await GetRequestStrAsync(url));
    }

    public static Texture2D GetTexture2D(string url)
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

    public static async Task<Texture2D> GetTexture2DAsync(string url)
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
