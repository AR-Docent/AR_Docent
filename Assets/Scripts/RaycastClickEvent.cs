using System;
using UnityEngine;

public class RaycastClickEvent : MonoBehaviour
{
    private Camera _mainCam;

    private Ray _ray;
    private RaycastHit _hit;
    private int _layer = 1 << 6;

    public Action<Transform, Vector3> clickEvent;

    void Awake()
    {
        _mainCam = Camera.main;
    }

    void OnEnable()
    {
        clickEvent += GuidObjectEvent;
    }

    void OnDisable()
    {
        clickEvent -= GuidObjectEvent;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _ray = _mainCam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(_ray, out _hit, 100f, _layer))
            {
                clickEvent(_hit.transform, _hit.point);
            }
        }
    }

    void GuidObjectEvent(Transform t, Vector3 vec)
    {
        if (t.CompareTag("Product"))
        {
            t.GetComponent<GuidButton>().OnClick();
        }
    }
}
