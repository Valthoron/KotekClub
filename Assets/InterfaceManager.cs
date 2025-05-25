using TMPro;
using UnityEngine;

public class Hud : MonoBehaviour
{
    // ********************************************************************************
    // Properties
    public GameObject Splash;
    public GameObject Play;
    public TextMeshProUGUI PlayScoreText;
    public GameObject Over;
    public TextMeshProUGUI OverScoreText;

    // ********************************************************************************
    // Unity messages
    public void Awake()
    {
        Splash.SetActive(true);
        Play.SetActive(false);
        Over.SetActive(false);
    }

    void Update()
    {

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
}
