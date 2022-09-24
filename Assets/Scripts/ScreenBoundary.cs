using UnityEngine;

public class ScreenBoundary : MonoBehaviour
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

    public Vector3 viewPort { get; private set; }
    
    private bool _status = false;

    void OnDisable()
    {
        ScreenIn = false;
    }

    // Update is called once per frame
    void Update()
    {
        viewPort = Camera.main.WorldToViewportPoint(transform.position);
        if (viewPort.x > 0f && viewPort.x < 1f && viewPort.y > 0f && viewPort.y < 1f)
        {
            ScreenIn = true;
        }
        else
        {
            ScreenIn = false;
        }
    }
}
