using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Networking;

public class Awaiter : INotifyCompletion
{
    private UnityWebRequestAsyncOperation asyncOp;
    private Action continuation;

    private void OnRequestCompleted(AsyncOperation obj)
    {
        continuation();
    }

    public Awaiter(UnityWebRequestAsyncOperation asyncOp)
    {
        this.asyncOp = asyncOp;
        asyncOp.completed += OnRequestCompleted;
    }

    public bool IsCompleted { get { return asyncOp.isDone; } }

    public void GetResult() { }

    public void OnCompleted(Action continuation)
    {
        this.continuation = continuation;
    }
}

public static class ExtensionMethods
{
    public static Awaiter GetAwaiter(this UnityWebRequestAsyncOperation asyncOp)
    {
        return new Awaiter(asyncOp);
    }
}
