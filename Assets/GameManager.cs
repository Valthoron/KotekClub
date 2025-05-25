using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // ********************************************************************************
    // Members
    private bool _anyKeyPressed = false;
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
        if (!_anyKeyPressed && Input.anyKeyDown)
        {
            _anyKeyPressed = true;
            StartGame();
        }
    }

    // ********************************************************************************
    // Game events
    private void OnDudeHealthChanged(float health)
    {
        Hud.SetHealth(health);
    }

    private void OnDudeDefeated()
    {
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

    // ********************************************************************************
    // Private methods
    private void StartGame()
    {
        Spawner.Active = true;
        Hud.ShowPlay();
    }
}
