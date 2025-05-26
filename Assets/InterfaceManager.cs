using TMPro;
using UnityEngine;

public class Hud : MonoBehaviour
{
    // ********************************************************************************
    // Properties
    public GameObject TouchControls;
    public GameObject Splash;
    public GameObject Play;
    public TextMeshProUGUI PlayScoreText;
    public HealthBar Health;
    public GameObject Over;
    public TextMeshProUGUI OverScoreText;

    // ********************************************************************************
    // Unity messages
    public void Awake()
    {
        TouchControls.SetActive(false);
        Splash.SetActive(true);
        Play.SetActive(false);
        Over.SetActive(false);
    }

    public void Start()
    {
        SetScore(0);
        SetHealth(1.0f);
    }

    public void Update()
    {
        if (Input.touchCount > 0)
            TouchControls.SetActive(true);
    }

    // ********************************************************************************
    // Game events
    public void ShowSplash()
    {
        Splash.gameObject.SetActive(true);
        Play.gameObject.SetActive(false);
        Over.gameObject.SetActive(false);
    }

    public void ShowPlay()
    {
        Splash.gameObject.SetActive(false);
        Play.gameObject.SetActive(true);
        Over.gameObject.SetActive(false);
    }

    public void ShowOver()
    {
        Splash.gameObject.SetActive(false);
        Play.gameObject.SetActive(false);
        Over.gameObject.SetActive(true);
    }

    public void SetScore(int score)
    {
        if (PlayScoreText != null)
            PlayScoreText.text = "Score: " + score.ToString();

        if (OverScoreText != null)
            OverScoreText.text = "Score: " + score.ToString();
    }

    public void SetHealth(float health)
    {
        Health.SetValue(health);
    }
}
