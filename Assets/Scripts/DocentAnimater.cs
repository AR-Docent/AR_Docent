using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DocentAnimater : MonoBehaviour
{
    public bool walk
    {
        get { return _walk; }
        set
        {
            _walk = value;
            _animator?.SetBool("walk", value);
        }
    }

    Animator _animator;

    bool _walk = false;
    
    void Awake()
    {
        _animator = GetComponent<Animator>();
    }
}
