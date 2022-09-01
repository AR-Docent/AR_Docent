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
            if (!_isSelected)
            {
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
    }

    public enum t_status { None, Limited, Tracking }

    private RaycastClickEvent _click;
    
    private t_status _status;
    private Material _mat;

    private bool _isSelected;

    void Awake()
    {
        _mat = GetComponent<MeshRenderer>().material;
        _click = GetComponent<RaycastClickEvent>();
    }

    void OnEnable()
    {
        _isSelected = false;
        _click.clickEvent += OnClick;
    }

    void OnDisable()
    {
        _click.clickEvent -= OnClick;
    }

    // Start is called before the first frame update
    void Start()
    {
        status = t_status.None;
    }

    void OnClick(Transform t, Vector3 p)
    {
        if (t.CompareTag("Product"))
        {
            _isSelected = !_isSelected;
        }
        if (_isSelected)
            _mat.color = Color.blue;
        else
            status = _status;
    }
}
