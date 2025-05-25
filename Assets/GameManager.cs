using UnityEngine;

public class GameManager : MonoBehaviour
{
    // ********************************************************************************
    // Members
    private bool _anyKeyPressed = false;

    // ********************************************************************************
    // Properties
    public Spawner Spawner;
    public Dude Dude;
    public Hud Hud;

    // ********************************************************************************
    // Unity messages
    public void Start()
    {
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

    // ********************************************************************************
    // Private methods
    private void StartGame()
    {
        Spawner.Active = true;
        Hud.ShowPlay();
    }
}
