using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static bool m_shuttingDown = false;
    private static object m_lock = new object();
    private static T m_Instance = null;

    public static T Instance
    {
        get
        {
            if (m_shuttingDown)
            {
                Debug.LogWarning($"Singleton {typeof(T).ToString()} Instance already destroyed. Returning null.");
                return null;
            }
            lock (m_lock)
            {
                if (m_Instance == null)
                {
                    //search existing instance
                    m_Instance = (T)FindObjectOfType(typeof(T));
                    //create new instance if not exist
                    if (m_Instance == null)
                    {
                        var singletonObj = new GameObject();
                        m_Instance = singletonObj.AddComponent<T>();
                        singletonObj.name = typeof(T).ToString() + "(Singleton)";

                        DontDestroyOnLoad(singletonObj);
                    }
                }
                return m_Instance;
            }
        }
    }

    private void OnApplicationQuit()
    {
        m_shuttingDown = true;
    }


    private void OnDestroy()
    {
        m_shuttingDown = true;
    }
}
