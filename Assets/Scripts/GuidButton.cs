using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuidButton : MonoBehaviour
{
    public string productName { get; set; } = null;

    public bool Tracking { get; set; } = false;

    public bool Selecting { get; private set; } = false;

    public void OnClick()
    {
        Selecting = !Selecting;
    }
}
