using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class ScoreManager : MonoBehaviour
{

    [SerializeField] float comboMultiplier = 1f;
    [SerializeField] float maxCombo = 3f;
    [SerializeField] bool comboFC = false;


    [SerializeField] float comboCooldownTime = 0.5f;
    private float comboCooldownCounter;

    private bool resetOnceBool;

    private float playerDeathCount;


    public float enemyDeathScore;

    private float currentScore;
    private float currentScoreTemp;
    private float methodBonus;
    private float enemyTypeBonus;
    private float calculatedScoreToAdd;

    private bool activeTimer = false;
    private bool updateTimerOnFalseOnce = false;

    [Header("Time")]
    [SerializeField] private float currentTime;
    [SerializeField] private bool increaseTimer = true;

    [Header("Time Score Bonuses")]
    [SerializeField] private float sRankTimeScore;
    [SerializeField] private float aRankTimeScore;
    [SerializeField] private float bRankTimeScore;
    [SerializeField] private float cRankTimeScore;

    [Header("Managers")]
    [SerializeField] GameCommunicationManager gameManager;
    [SerializeField] private ResultsScreen resultsScreen;
    [SerializeField] private LeaderboardScoreManager leaderboardManager;


    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameCommunicationManager>();

        currentTime = 0;

        comboFC = true;

        increaseTimer = true;

        playerDeathCount = 0;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (comboCooldownCounter > 0 && activeTimer == true)
        {
            comboCooldownCounter -= Time.deltaTime;
            gameManager.UpdateRadialTimerUI(comboCooldownCounter, comboCooldownTime);
        }
        else if (activeTimer == false && updateTimerOnFalseOnce == false)
        {
            updateTimerOnFalseOnce = true;
            gameManager.UpdateRadialTimerUI(comboCooldownCounter, comboCooldownTime);
        }
        else if (activeTimer == false)
        {

        }
        else if (resetOnceBool == false)
        {
            resetOnceBool = true;
            gameManager.ResetCombo();
        }

        if (increaseTimer)
        {
            if ((PauseManager.instance.IsGamePaused() == false) && (resultsScreen.IsResultsShowing() == false))
            {
                currentTime += Time.deltaTime;

            }
        }
        
        
    }

    public float CalculateScore(EnemyAI enemy, string method)
    {
        methodBonus = 1;
        enemyTypeBonus = enemyDeathScore;

        if (method == "tp")
        {
            methodBonus = 0.75f;
        }

        if (method == "reflect")
        {
            methodBonus = 0.8f;
        }

        if (enemy.enemyID == 1)
        {
            enemyTypeBonus *= 1.5f;
        }

        calculatedScoreToAdd = enemyTypeBonus * methodBonus;

        calculatedScoreToAdd *= comboMultiplier;

        return calculatedScoreToAdd;
        
    }

    public float UpdateScore(float scoreToAdd)
    {
        currentScoreTemp += scoreToAdd;
        return currentScoreTemp;
    }

    public void BankPoints()
    {
        currentScore = currentScoreTemp;
    }

    public void RewindPoints()
    {
        currentScoreTemp = currentScore;
    }

    public float GetCurrentScore()
    {
        return currentScore;
    }

    public float UpdateCombo()
    {
        resetOnceBool = false;
        updateTimerOnFalseOnce = false;

        comboCooldownCounter = comboCooldownTime;

        if (comboMultiplier < maxCombo)
        {
            comboMultiplier++;
        }

        return comboMultiplier;
    }

    public float ResetCombo()
    {
        comboCooldownCounter = 0;
        comboMultiplier = 1;

        if (comboFC == true)
        {
            comboFC = false;

            gameManager.DropComboFC();
        }

        return comboMultiplier;
    }

    public float CalculateFinalTimerBonus()
    {
        float timerScore = 0.0f;

        if (currentTime < 75)
        {
            timerScore = sRankTimeScore;
        }
        else if (currentTime < 120)
        {
            timerScore = aRankTimeScore;
        }
        else if (currentTime < 180)
        {
            timerScore = bRankTimeScore;
        }
        else
        {
            timerScore = cRankTimeScore;
        }

        return timerScore;
    }

    public string returnFinalTime()
    {
        float minutes = Mathf.Floor(currentTime / 60);
        float seconds = currentTime % 60;
        float milliseconds = ((currentTime - (Mathf.Floor(currentTime))) * 100);

        string tempString = minutes + "." + seconds.ToString("00") + "." + milliseconds.ToString("00");

        return tempString;
    }

    public float getFCPoints()
    {
        if (comboFC)
        {
            return 2500;
        }
        else
        {
            return 0;
        }
    }

    public bool isComboFC()
    {
        return comboFC;
    }

    public void PlayTimer()
    {
        activeTimer = true;
    }

    public void PauseTimer()
    {
        activeTimer = false;
    }

    public void IncreaseDeathCount()
    {
        playerDeathCount = playerDeathCount + 1;
    }

    public float GetDeathCount()
    {
        return playerDeathCount;
    }

    //public void SaveScore(float score)
    //{
    //    Score newScore = new Score(name, currentTime, score);

    //    leaderboardManager.AddScore(newScore);
    //}
}
