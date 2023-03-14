using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkManager
{

    private static T FromByteArray<T>(byte[] data)
    {
        T ret = default;

        if (data == null)
        {
            return ret;
        }
        BinaryFormatter binary = new ();
        using MemoryStream ms = new(data);

        object obj = binary.Deserialize(ms);
        ret = (T)obj;

        return ret;
    }
    public static T GetData<T>(string url)
    {
        T ret = default;
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            UnityWebRequestAsyncOperation oper = request.SendWebRequest();
            while (!oper.isDone)
            {
            }
            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"{request.error} : {request.url}");
                throw new Exception($"{request.error} : {request.url}");
            }
            ret = FromByteArray<T>(request.downloadHandler.data);
        }
        return ret;
    }

    public static async Task<T> GetDataAsync<T>(string url)
    {
        return await Task.Run<T>(async () =>
        {
            T ret = default;

            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
                UnityWebRequestAsyncOperation oper = request.SendWebRequest();
                while (!oper.isDone)
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(10));
                }
                if (request.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError($"{request.error} : {request.url}");
                    throw new Exception($"{request.error} : {request.url}");
                }
                ret = FromByteArray<T>(request.downloadHandler.data);
            }
            return ret;
        });
    }
}
