using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PointManager : MonoBehaviour
{
    public bool Tracking
    {
        get
        {
            return _tracking;
        }
        set
        {
            _tracking = value;
            _trackerImage.enabled = value & !_selecting;
        }
    }

    public bool Selecting
    {
        get
        {
            return _selecting;
        }
        set
        {
            _selecting = value;
            _selecterImage.enabled = value;
            //turn off other UI    
            _trackerImage.enabled = _tracking & !value;
            _pointerImage.enabled = !value;
        }
    }

    public string Name { get; set; } = null;
    public float UIScale { get { return _rect.localScale.x; } set { _rect.localScale = new Vector3(value, value, 1); } }

    private bool _tracking = false;
    private bool _selecting = false;

    private Image _pointerImage;
    private Image _trackerImage;
    private Image _selecterImage;
    
    private TextMeshProUGUI _text;
    private RectTransform _rect;

    private void Awake()
    {
        _pointerImage = transform.GetChild(0).GetComponent<Image>();
        _trackerImage = transform.GetChild(1).GetComponent<Image>();
        _selecterImage = transform.GetChild(2).GetComponent<Image>();

        _text = transform.GetChild(3).GetComponent<TextMeshProUGUI>();

        _rect = GetComponent<RectTransform>();
    }

    void OnEnable()
    {
        _text.text = Name;
        Selecting = false;
        Tracking = false;
    }
}
