using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
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

    private IEnumerator GetRequestStrIEnumerator(string url)
    {
        Debug.Log("coroutine thread:" + Thread.CurrentThread.ManagedThreadId);
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

    public Task<T> GetDataFromJsonTask<T>(string url)
    {
        IEnumerator task = GetRequestStrIEnumerator(url);
        try
        {
            StartCoroutine(task);
            return Task.Run(() =>
            {
                while (task.Current == null)
                {
                    Task.Yield();
                }
                if (typeof(T) == typeof(string))
                    return ((T)task.Current);
                return FromJson<T>((string)task.Current);
            });
        }
        catch (Exception e)
        {
            StopCoroutine(task);
            Debug.LogError(e);
            return Task.FromException<T>(e);
        }
    }

    public Task<IEnumerable<T>> GetDataListFromJsonTask<T>(string url)
    {
        IEnumerator task = GetRequestStrIEnumerator(url);
        try
        {
            StartCoroutine(task);
            return Task.Run<IEnumerable<T>>(() =>
            {
                Debug.Log("wait task start:" + Thread.CurrentThread.ManagedThreadId);
                while (task.Current == null)
                {
                    Task.Yield();
                }
                Debug.Log("wait task end:" + Thread.CurrentThread.ManagedThreadId);
                return IENumerableFromJson<T>((string)task.Current);
            });
        }
        catch (Exception e)
        {
            StopCoroutine(task);
            Debug.LogError(e);
            return Task.FromException<IEnumerable<T>>(e);
        }
    }

    private IEnumerator<Texture2D> GetTexture2DIEnumerator(Task<string> t)
    {
        while (!t.IsCompleted)
        {
            yield return null;
        }
        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(t.Result))
        {
            UnityWebRequestAsyncOperation oper = request.SendWebRequest();

            while (oper.isDone == false)
            {
                yield return null;
            }
            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.Log($"{request.error} : {request.url}");
                throw new Exception($"{request.error} : {request.url}");
            }
            yield return DownloadHandlerTexture.GetContent(request);
        }
    }

    public Task<Texture2D> GetTextureTask(string url)
    {
        Task<string> t = GetDataFromJsonTask<string>(url);
        IEnumerator t_task = GetTexture2DIEnumerator(t);
        try
        {
            Debug.Log("start task");
            StartCoroutine(t_task);

            return Task.Run<Texture2D>(() =>
            {
                Debug.Log("wait imgtask start:" + Thread.CurrentThread.ManagedThreadId);
                Task.WaitAll(t);
                while (t_task.Current == null)
                {
                    Task.Yield();
                }
                Debug.Log("wait imgtask end:" + Thread.CurrentThread.ManagedThreadId);
                return (Texture2D)t_task.Current;
            });
        }
        catch (Exception e)
        {
            StopCoroutine(t_task);
            Debug.LogError(e);
            return Task.FromException<Texture2D>(e);
        }
    }

    private IEnumerator<AudioClip> GetAudioIEnumerator(Task<string> t)
    {
        while (!t.IsCompleted)
        {
            yield return null;
        }
        using (UnityWebRequest request = UnityWebRequestMultimedia.GetAudioClip(t.Result, AudioType.WAV))
        {
            UnityWebRequestAsyncOperation oper = request.SendWebRequest();

            while (oper.isDone == false)
            {
                yield return null;
            }
            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.Log($"{request.error} : {request.url}");
                throw new Exception($"{request.error} : {request.url}");
            }
            yield return DownloadHandlerAudioClip.GetContent(request);
        }
    }

    public Task<AudioClip> GetAudioTask(string url)
    {
        Task<string> t = GetDataFromJsonTask<string>(url);
        IEnumerator a_task = GetAudioIEnumerator(t);
        try
        {
            Debug.Log("start task");
            StartCoroutine(a_task);

            return Task.Run<AudioClip>(() =>
            {
                Debug.Log("wait audiotask start:" + Thread.CurrentThread.ManagedThreadId);
                Task.WaitAll(t);
                while (a_task.Current == null)
                {
                    Task.Yield();
                }
                Debug.Log("wait audiotask end:" + Thread.CurrentThread.ManagedThreadId);
                return (AudioClip)a_task.Current;
            });
        }
        catch (Exception e)
        {
            StopCoroutine(a_task);
            Debug.LogError(e);
            return Task.FromException<AudioClip>(e);
        }
    }
}
