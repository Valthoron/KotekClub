using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // ********************************************************************************
    // Structs/enums
    private enum GameState : int
    {
        Splash,
        Play,
        Over
    }

    // ********************************************************************************
    // Members
    private GameState _state = GameState.Splash;
    private int _score = 0;

    // ********************************************************************************
    // Properties
    public Spawner Spawner;
    public Dude Dude;
    public Hud Hud;

    // ********************************************************************************
    // Unity messages
    public void Start()
    {
        Spawner.BaddieSpawned.AddListener(OnBaddieSpawned);

        Dude.HealthChanged.AddListener(OnDudeHealthChanged);
        Dude.Defeated.AddListener(OnDudeDefeated);

        Hud.ShowSplash();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Application.isEditor)
                EditorApplication.isPlaying = false;
            else
                Application.Quit();
        }

        switch (_state)
        {
            case GameState.Splash:
                Update_Splash();
                break;

            case GameState.Play:
                Update_Play();
                break;

            case GameState.Over:
                Update_Over();
                break;
        }
    }

    // ********************************************************************************
    // Game events
    private void Update_Splash()
    {
        if (Input.anyKeyDown)
        {
            Spawner.Active = true;
            Hud.ShowPlay();
        }
    }

    private void Update_Play()
    {

    }

    private void Update_Over()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    private void OnDudeHealthChanged(float health)
    {
        Hud.SetHealth(health);
    }

    private void OnDudeDefeated()
    {
        _state = GameState.Over;
        Spawner.Active = false;
        Hud.ShowOver();
    }

    private void OnBaddieSpawned(Baddie baddie)
    {
        baddie.Defeated.AddListener(OnBaddieDefeated);
    }

    private void OnBaddieDefeated(Baddie baddie)
    {
        _score += 1;
        Hud.SetScore(_score);
    }
}
