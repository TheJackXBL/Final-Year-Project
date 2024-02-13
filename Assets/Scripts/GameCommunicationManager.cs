using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCommunicationManager : MonoBehaviour
{

    public CharacterMovement player;
    public EnemyChecker checker;
    public LightManager lightManager;
    public UIManager uiManager;
    public MainMenuManager mainMenuManager;
    public ScoreManager scoreManager;
    public CameraController cameraManager;
    public HitStop hitStopManager;
    public ResultsScreen resultsManager;
    public PauseManager pauseManager;
    public SceneController sceneManager;

    public GameObject level;
    //public EnemyChecker enemyChecker;

    

    private float comboMultiplier;

    private float scoreToAdd;

    //private void Start()
    //{
    //    EndLevel();
    //}

    public void HitStop(float duration)
    {
        hitStopManager.Stop(duration);
    }

    public void AddScoreToTemp(EnemyAI enemyDefeated, string method)
    {
        scoreToAdd = scoreManager.CalculateScore(enemyDefeated, method);

        uiManager.UpdateScoreUI(scoreManager.UpdateScore(scoreToAdd));
    }

    public void UpdateCombo()
    {
        comboMultiplier = scoreManager.UpdateCombo();

        uiManager.UpdateComboUI(comboMultiplier);
    }

    public void UpdateRadialTimerUI(float comboTimer, float comboCooldownMax)
    {
        uiManager.UpdateComboTimerUI(comboTimer, comboCooldownMax);
    }

    public void ResetCombo()
    {
        comboMultiplier = scoreManager.ResetCombo();

        uiManager.UpdateComboUI(comboMultiplier);
    }

    public void DropComboFC()
    {
        uiManager.DropComboFC();
    }

    public void UpdateKunaiUI(float currentCooldown, float maxCooldown)
    {
        uiManager.UpdateKunaiUI(currentCooldown, maxCooldown);
    }

    public void CheckForEnemies(EnemyChecker enemyChecker)
    {
        checker = enemyChecker;

        float enemyCount = checker.CheckForEnemies();

        if (enemyCount == 0)
        {
            scoreManager.PauseTimer();

            checker.OpenDoor();

        }
        else
        {
            scoreManager.PlayTimer();
        }
    }

    public void LoadNextArena(float orthoSize, Vector3 playerSpawnPoint)
    {
        cameraManager.MoveCameraConfiner(orthoSize);

        if (checker != null)
        {
            checker.HidePreviousLevel();
        }
        

        scoreManager.BankPoints();

        //fetch next level in hierarchy
        int nextLevel = cameraManager.GetCurrentLevelConfiner();

        lightManager = level.transform.GetChild(nextLevel - 1).GetComponentInChildren<LightManager>();

        if (lightManager != null)
        {
            lightManager.HideCurrentLights();
        }
        
        //get light manager and show next lights
        lightManager = level.transform.GetChild(nextLevel).GetComponentInChildren<LightManager>();

        if (lightManager != null)
        {
            lightManager.ShowNextLights();
        }

        //Set player spawn to transform of collider
        player.SetPlayerSpawn(playerSpawnPoint);

        

    }

    public void PlayerDeath()
    {
        uiManager.PlayerDeath();

        scoreManager.IncreaseDeathCount();
    }

    public void ResetLevel()
    {
        player.ResetPlayer();

        scoreManager.ResetCombo();

        scoreManager.RewindPoints();

        uiManager.UpdateScoreUI(scoreManager.GetCurrentScore());

        int currentLevel = cameraManager.GetCurrentLevelConfiner();

        EnemyChecker currentEnemyChecker = level.transform.GetChild(currentLevel).GetComponentInChildren<EnemyChecker>();

        currentEnemyChecker.ResetEnemies();

        uiManager.ResetLevel();

    }

    public void EndLevel()
    {

        scoreManager.BankPoints();

        uiManager.ShowResultsScreen();

        cameraManager.ChangeCameraOrtho(4.0f);

        resultsManager.DisplayResults();

        player.DisableMovement();

        
    }

    public void NextLevel()
    {
        if (uiManager != null)
        {
            uiManager.ScreenWipeIn();
        }
        else if (mainMenuManager != null)
        {
            mainMenuManager.ScreenWipeIn();
        }
        

        sceneManager.LoadNextScene();
    }

    public void LoadCyberpunk()
    {
        if (uiManager != null)
        {
            uiManager.ScreenWipeIn();
        }
        else if (mainMenuManager != null)
        {
            mainMenuManager.ScreenWipeIn();
        }


        sceneManager.LoadCyberpunk();
    }

    

    public void RestartLevel()
    {
        uiManager.ScreenWipeIn();

        sceneManager.RestartLevel();
    }

    public void ReturnToMenu()
    {
        uiManager.ScreenWipeIn();

        sceneManager.ReturnToMenu();
    }

    public void PauseGame()
    {
        uiManager.ShowPauseScreen();
    }

    public void UnpauseGame()
    {
        uiManager.HidePauseScreen();
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
