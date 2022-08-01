using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Debugger : MonoBehaviour
{
    public t_status status
    {
        get => _status;
        set
        {
            _status = value;
            switch (_status)
            {
                case t_status.None:
                    image.color = Color.gray;
                    statusText.text = "None";
                    break;
                case t_status.Limited:
                    image.color = Color.red;
                    statusText.text = "Limited";
                    break;
                case t_status.Tracking:
                    image.color = Color.green;
                    statusText.text = "Tracking";
                    break;
            }
        }
    }

    public enum t_status { None, Limited, Tracking }

    private t_status _status; 
    Image image;
    Text statusText;

    void Awake()
    {
        image = GetComponent<Image>();
        statusText = GetComponentInChildren<Text>();
    }
    // Start is called before the first frame update
    void Start()
    {
        status = t_status.None;
    }
}
