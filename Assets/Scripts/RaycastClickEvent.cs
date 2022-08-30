using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastClickEvent : MonoBehaviour
{
    private Camera _mainCam;

    private Ray _ray;
    private RaycastHit _hit;
    private int _layer = 1 << 6;

    public bool clicked { get; private set; } = false;
    public Action<bool> clickEvent;

    void Awake()
    {
        _mainCam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _ray = _mainCam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(_ray, out _hit, 100f, _layer))
            {
                if (_hit.transform == this.transform)
                {
                    clicked = !clicked;
                    clickEvent(clicked);
                }
            }
        }
    }
}
