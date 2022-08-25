using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PointManager : MonoBehaviour
{
    public bool Tracking { get; private set;}

    public string Name { get; set; } = null;
    
    private Image _pointerImage;
    private TextMeshProUGUI _text;

    private void Awake()
    {
        _pointerImage = transform.GetChild(0).GetComponent<Image>();
        _text = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
    }

    void OnEnable()
    {
        _text.text = Name;
    }
}
