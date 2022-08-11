using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
                    _mat.color = Color.gray;
                    break;
                case t_status.Limited:
                    _mat.color = Color.red;
                    break;
                case t_status.Tracking:
                    _mat.color = Color.green;
                    break;
            }
        }
    }

    public enum t_status { None, Limited, Tracking }

    private t_status _status;

    private Material _mat;

    void Awake()
    {
        _mat = GetComponent<MeshRenderer>().material;
    }
    // Start is called before the first frame update
    void Start()
    {
        status = t_status.None;
    }
}
