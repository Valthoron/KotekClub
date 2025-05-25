using UnityEngine;

public class HealthBar : MonoBehaviour
{
	// ********************************************************************************
	// Properties
	public GameObject FullBar;
	public GameObject EmptyBar;

	// ********************************************************************************
	// Unity messages
	void Start()
    {

    }

    void Update()
    {
        
    }

	// ********************************************************************************
	// Gameplay messages
	public void SetValue(float value)
	{
		value = Mathf.Clamp(value, 0.0f, 1.0f);

		Vector3 scale = FullBar.transform.localScale;
		scale.x = value;

		Vector3 position = FullBar.transform.localPosition;
		position.x = -(1.0f - value) * 10.0f / 2.0f;

		FullBar.transform.localScale = scale;
		FullBar.transform.localPosition = position;
	}
}
