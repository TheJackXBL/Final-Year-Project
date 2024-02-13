using DG.Tweening.Core.Easing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{

    public static PauseManager instance;

    [SerializeField] GameCommunicationManager gameManager;

    [SerializeField] private bool paused;

    [SerializeField] private ResultsScreen resultsScreen;

    

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GetComponent<GameCommunicationManager>();

        paused = false;

        if (instance == null)
        {
            instance = this;
        }

    }    

    public void PauseGame(InputAction.CallbackContext context)
    {
        if (context.started == true && (resultsScreen.IsResultsShowing() == false))
        {
            if (!paused)
            {
                Pause();
            }
            else
            {
                Resume();
            }

        }
    }

    void Pause()
    {
        gameManager.PauseGame();

        paused = true;

        Time.timeScale = 0.0f;
    }

    public void Resume()
    {
        gameManager.UnpauseGame();

        paused = false;

        Time.timeScale = 1.0f;
    }

    public bool IsGamePaused()
    {
        return paused;
    }
}
