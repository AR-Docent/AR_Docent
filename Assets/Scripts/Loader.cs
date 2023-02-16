using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class Loader : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(LoadScenes());
    }

    IEnumerator LoadScenes()
    {
        float timer = 0f;

        yield return new WaitUntil(() => DownloadSource.Instance.complete);

        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync("SampleScene");

        asyncOperation.allowSceneActivation = false;
        timer += Time.deltaTime;

        while (!asyncOperation.isDone)
        {
            yield return null;
            if (asyncOperation.progress < 0.9f)
            {
            }
            else
            {
                asyncOperation.allowSceneActivation = true;
                yield break;
            }
        }
    }
}
