using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastScreenStatus : MonoBehaviour
{
    public bool ScreenIn
    {
        get
        {
            return _status;
        }
        private set
        {
            if (value != _status)
            {
                _status = value;
                if (value)
                {
                    ProductStatusUI.screenInObj += 1;
                }
                else
                {
                    ProductStatusUI.screenInObj = ProductStatusUI.screenInObj > 0 ? ProductStatusUI.screenInObj - 1 : ProductStatusUI.screenInObj;
                }
            }
        }
    }

    private bool _status = false;
    private Vector3 screenPos;

    void OnDisable()
    {
        ProductStatusUI.screenInObj = ProductStatusUI.screenInObj > 0 ? ProductStatusUI.screenInObj - 1 : ProductStatusUI.screenInObj;
    }

    // Update is called once per frame
    void Update()
    {
        screenPos = Camera.main.WorldToViewportPoint(transform.position);
        if (screenPos.x > 0f && screenPos.x < 1f)
        {
            ScreenIn = true;
        }
        else
        {
            ScreenIn = false;
        }
    }
}
