using UnityEngine;

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
