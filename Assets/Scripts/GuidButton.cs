using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GuidButton : MonoBehaviour
{
    public string productName { get; set; } = null;

    private TextMeshPro _text;

    void Awake()
    {
        _text = GetComponentInChildren<TextMeshPro>();
    }

    private void OnEnable()
    {
        if (productName != null)
        {
            _text.text = productName;
        }
    }

    private void OnDisable()
    {
        
    }
}
