using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    // ********************************************************************************
    // Properties
    public Image EmptyBar;
    public Image FullBar;

    public Color FullColor = Color.green;
    public Color EmptyColor = Color.red;

    public bool Flip;

    // ********************************************************************************
    // Unity messages
    public void Start()
    {
        UpdateColors();
    }

    public void Update()
    {

    }

    public void OnValidate()
    {
        SetValue(0.5f);
        UpdateColors();
    }

    // ********************************************************************************
    // Gameplay messages
    public void UpdateColors()
    {
        EmptyBar.color = EmptyColor;
        FullBar.color = FullColor;
    }

    public void SetValue(float val)
    {
        var sizeDelta = 1f - Mathf.Clamp(val, 0f, 1f);
        var position = (sizeDelta / 2f) * (Flip ? 1f : -1f);

        var transform = FullBar.GetComponent<RectTransform>();
        transform.sizeDelta = new Vector2(-sizeDelta, 0f);
        transform.anchoredPosition = new Vector2(position, 0f);
    }
}
