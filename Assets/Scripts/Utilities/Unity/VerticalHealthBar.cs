using Assets.Scripts.Utilities.Unity.Interface;
using UnityEngine;
using Image = UnityEngine.UI.Image;

public class VerticalHealthBar : MonoBehaviour, IVerticalHealthBar
{
    private RectTransform _rectTransform;
    private float _maxHeight;

    private Image _image;
    private Color _initialColor;

    [SerializeField]
    private float maxValue = 100f;
    [SerializeField]
    private Color flashingColor = Color.red;
    [SerializeField]
    private float flashingFrequency = .3f;
    [SerializeField]
    private float percentThresholdFlashing = .1f;

    private float _internalValue;
    private bool _flashing = false;
    private bool _flashed = false;

    private float _timeElaspedSinceFlashing;

    public float Value
    {
        get
        {
            return _internalValue;
        }
        set
        {
            if (value >= 0 && value <= 100f)
            {
                _internalValue = value;
            }
            else
            {
                Debug.LogError($"Out of bound value {value} for healthbar {gameObject}");
            }
        }
    }

    public float MaxValue { get => maxValue; set => maxValue = value; }

    private void UpdateHeight(float percentValue)
    {
        var size = _rectTransform.sizeDelta;
        size.y = _maxHeight * percentValue;
        _rectTransform.sizeDelta = size;
    }

    private void UpdateFlashing(float percentValue)
    {
        if (percentValue <= percentThresholdFlashing)
        {
            _flashing = true;
            _timeElaspedSinceFlashing = 0f;
        }
        else
        {
            _flashing = false;
        }
    }

    public void SetFlashingColor(Color color)
    {
        flashingColor = color;
    }

    public void SetFlashingFrequency(float frequency)
    {
        flashingFrequency = frequency;
    }

    public void SetPercentThresholdFlashing(float threshold)
    {
        percentThresholdFlashing = threshold;
    }
    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _image = GetComponent<Image>();

        _maxHeight = _rectTransform.rect.height;
        _initialColor = _image.color;
        _internalValue = maxValue;
        _timeElaspedSinceFlashing = 0f;
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        var percentValue = _internalValue / maxValue;
        UpdateHeight(percentValue);
        UpdateFlashing(percentValue);
        HandleFlashing();
    }

    private void HandleFlashing()
    {
        if (_flashing)
        {
            _timeElaspedSinceFlashing += Time.deltaTime;
            if (_timeElaspedSinceFlashing >= flashingFrequency)
            {
                _image.color = _flashed ? _initialColor : flashingColor;
                _flashed = !_flashed;
                _timeElaspedSinceFlashing = 0f;
            }
        }
        else
        {
            _image.color = _initialColor;
        }
    }
}
