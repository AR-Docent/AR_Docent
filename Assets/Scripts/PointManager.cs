using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PointManager : MonoBehaviour
{
    public bool Tracking { get; private set;}

    public string Name { get; set; } = null;
    public float UIScale { get { return _rect.localScale.x; } set { _rect.localScale = new Vector3(value, value, value); } }
    
    private Image _pointerImage;
    private TextMeshProUGUI _text;
    private RectTransform _rect;

    private void Awake()
    {
        _pointerImage = transform.GetChild(0).GetComponent<Image>();
        _text = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        _rect = GetComponent<RectTransform>();
    }

    void OnEnable()
    {
        _text.text = Name;
    }
}
