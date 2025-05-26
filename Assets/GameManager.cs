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
    // Constants
    private const int ScorePerDifficulty = 4;

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

    // ********************************************************************************
    // Game events
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
        baddie.SetDifficulty(_score / ScorePerDifficulty);
        baddie.Defeated.AddListener(OnBaddieDefeated);
    }

    private void OnBaddieDefeated(Baddie baddie)
    {
        if (_state != GameState.Play)
            return;

        _score += 1;
        Hud.SetScore(_score);
    }

    // ********************************************************************************
    // Input events
    public void OnBegin()
    {
        if ((_state == GameState.Splash) && UnityEngine.Rendering.SplashScreen.isFinished)
        {
            _state = GameState.Play;
            Spawner.Active = true;
            Hud.ShowPlay();
        }
    }

    public void OnRestart()
    {
        if (_state == GameState.Over)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    public void OnExit()
    {
#if UNITY_EDITOR

        if (Application.isEditor)
            EditorApplication.isPlaying = false;
        else
            Application.Quit();

#elif !UNITY_WEBGL

        Application.Quit();

#endif
    }
}
