using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GuidButton : MonoBehaviour
{
    public string productName { get; set; } = null;

    public bool Tracking { get; set; } = false;

    public bool Selecting { get; private set; } = false;

    private RaycastClickEvent _click;

    void Awake()
    {
        _click = GetComponent<RaycastClickEvent>();
    }

    private void OnEnable()
    {
        _click.clickEvent += OnClick;
    }

    private void OnDisable()
    {
        _click.clickEvent -= OnClick;
    }

    void OnClick(Transform t, Vector3 p)
    {
        if (t == this.transform)
        {
            Selecting = !Selecting;
        }
    }
}
