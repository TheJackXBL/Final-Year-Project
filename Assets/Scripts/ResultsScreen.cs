using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ResultsScreen : MonoBehaviour
{

    //[SerializeField] private float currentScore;

    private bool resultsDisplayed = false;

    [Header("Text Boxes")]
    [SerializeField] private TextMeshProUGUI levelBonusScoreText;
    [SerializeField] private TextMeshProUGUI timeBonusScoreText;
    [SerializeField] private TextMeshProUGUI timeStringText;
    [SerializeField] private TextMeshProUGUI comboBonusScoreText;
    [SerializeField] private TextMeshProUGUI deathCountText;
    [SerializeField] private TextMeshProUGUI deathScoreText;
    [SerializeField] private TextMeshProUGUI finalScoreText;

    [Header("Effects")]
    [SerializeField] private Image comboFCStar;

    private bool levelTextFinished = false;
    private bool timeTextFinished = false;
    private bool comboTextFinished = false;

    [SerializeField] private EventSystem eventSystem;
    [SerializeField] private Button nextLevelButton;


    [SerializeField] private float finalScore;

    private enum Rank
    {
        S,
        A,
        B,
        C
    }

    [Header("Rank")]
    [SerializeField] Rank rank;
    [SerializeField] private TextMeshProUGUI rankText;
    [SerializeField] private Material rankSMat;
    [SerializeField] private Material rankAMat;
    [SerializeField] private Material rankBMat;
    [SerializeField] private Material rankCMat;
    [SerializeField] private float rankSScore;
    [SerializeField] private float rankAScore;
    [SerializeField] private float rankBScore;
    //[SerializeField] private float rankCScore;

    [SerializeField] private GameObject rankTextSmoke;

    [Header("Parameters")]
    [SerializeField] private float fadeTime;
    [SerializeField] private float incrementTime;


    [Header("Managers")]
    [SerializeField] private ScoreManager scoreManager;

    // Start is called before the first frame update
    void Awake()
    {
        resultsDisplayed = false;
        
    }

    private void OnEnable()
    {
        resultsDisplayed = true;
        DisplayResults();
    }

    private void OnDisable()
    {
        resultsDisplayed = false;
    }

    public void DisplayResults()
    {
        HideAllThings();

        StartCoroutine(DisplayResultsCoroutine());

    }

    private IEnumerator DisplayResultsCoroutine()
    {
        yield return StartCoroutine(IncrementDisplayedScore(levelBonusScoreText, scoreManager.GetCurrentScore()));

        yield return StartCoroutine(ShowTimeScore(timeBonusScoreText, scoreManager.CalculateFinalTimerBonus()));

        yield return StartCoroutine(ShowComboScore(comboBonusScoreText, scoreManager.getFCPoints()));

        yield return StartCoroutine(ShowDeathScore(deathScoreText, scoreManager.GetDeathCount()));

        yield return StartCoroutine(CalculateRank());

        //move this to turn on after all the scores are displayed
        eventSystem.sendNavigationEvents = true;

        eventSystem.SetSelectedGameObject(nextLevelButton.gameObject);
    }
    IEnumerator FadeInText(TextMeshProUGUI tmText, string message)
    {

        for (int i = 0; i < message.Length + 1; i++)
        {
            tmText.text = message.Substring(0, i);
            yield return new WaitForSeconds(fadeTime);
        }


       


    }

    IEnumerator IncrementDisplayedScore(TextMeshProUGUI tmText, float score)
    {

        float displayedScore = 0.0f;

        tmText.transform.parent.gameObject.SetActive(true);

        tmText.text = displayedScore.ToString("00000");

        yield return new WaitForSeconds(0.5f);

        tmText.gameObject.SetActive(true);

        yield return new WaitForSeconds(0.5f);

        while (displayedScore < score)
        {
            if (score - displayedScore < 100)
            {
                displayedScore += score - displayedScore;
            }
            else
            {
                displayedScore += 100;
            }
            

            tmText.text = displayedScore.ToString("00000");

            yield return new WaitForSeconds(incrementTime);
        }


        //EnableNextBool();



    }


    IEnumerator ShowTimeScore(TextMeshProUGUI tmText, float score)
    {


        float displayedScore = 0.0f;

        tmText.transform.parent.gameObject.SetActive(true);

        yield return new WaitForSeconds(0.5f);

        timeStringText.text = scoreManager.returnFinalTime();

        timeStringText.gameObject.SetActive(true);

        yield return new WaitForSeconds(0.5f);

        tmText.text = displayedScore.ToString("00000");

        tmText.gameObject.SetActive(true);

        yield return new WaitForSeconds(0.5f);

        while (displayedScore < score)
        {
            displayedScore += 100;

            tmText.text = displayedScore.ToString("00000");

            yield return new WaitForSeconds(incrementTime);
        }

    }

    IEnumerator ShowComboScore(TextMeshProUGUI tmText, float score)
    {

        
        float displayedScore = score;

        tmText.transform.parent.gameObject.SetActive(true);

        tmText.text = displayedScore.ToString("00000");

        yield return new WaitForSeconds(0.5f);

        tmText.gameObject.SetActive(true);

        if (score > 0)
        {
            yield return new WaitForSeconds(0.5f);

            comboFCStar.gameObject.SetActive(true);
        }

    }

    IEnumerator ShowDeathScore(TextMeshProUGUI tmText, float score)
    {


        float displayedScore = score;

        tmText.transform.parent.gameObject.SetActive(true);

        yield return new WaitForSeconds(0.5f);

        deathCountText.text = score.ToString();

        deathCountText.text = deathCountText.text + "x";

        deathCountText.gameObject.SetActive(true);

        yield return new WaitForSeconds(0.5f);

        tmText.text = (score * 100).ToString();

        tmText.text = "-" + tmText.text;

        tmText.gameObject.SetActive(true);

        

    }

    void HideAllThings()
    {
        levelBonusScoreText.transform.parent.gameObject.SetActive(false);
        timeBonusScoreText.transform.parent.gameObject.SetActive(false);
        comboBonusScoreText.transform.parent.gameObject.SetActive(false);
        deathScoreText.transform.parent.gameObject.SetActive(false);
        finalScoreText.transform.parent.gameObject.SetActive(false);
        //rankText.transform.parent.gameObject.SetActive(false);

        levelBonusScoreText.gameObject.SetActive(false);
        timeBonusScoreText.gameObject.SetActive(false);
        timeStringText.gameObject.SetActive(false);
        comboBonusScoreText.gameObject.SetActive(false);
        deathScoreText.gameObject.SetActive(false);
        deathCountText.gameObject.SetActive(false);
        finalScoreText.gameObject.SetActive(false);
        rankText.gameObject.SetActive(false);
        comboFCStar.gameObject.SetActive(false);
        //rankTextSmoke.SetActive(false);
    }

    //void EnableNextBool()
    //{
    //    if (levelTextFinished == false)
    //    {
    //        levelTextFinished = true;
    //    }
    //    else if (timeTextFinished == false)
    //    {
    //        timeTextFinished = true;
    //    }
    //    else if (comboTextFinished == false)
    //    {
    //        comboTextFinished = true;
    //    }
    //}

    IEnumerator CalculateRank()
    {

        finalScore = float.Parse(levelBonusScoreText.text) + float.Parse(timeBonusScoreText.text) + float.Parse(comboBonusScoreText.text) + float.Parse(deathScoreText.text);

        if (finalScore > rankSScore || scoreManager.isComboFC())
        {
            rank = Rank.S;
        }
        else if (finalScore > rankAScore)
        {
            rank = Rank.A;
        }
        else if (finalScore > rankBScore)
        {
            rank = Rank.B;
        }
        else
        {
            rank = Rank.C;
        }

        yield return StartCoroutine(IncrementDisplayedScore(finalScoreText, finalScore));

        SetRankColor();

        yield return StartCoroutine(ShowRank());

        //scoreManager.SaveScore(finalScore);
        
    }

    
    void SetRankColor()
    {
        if (rank == Rank.S)
        {
            rankText.text = "S";
            rankText.fontMaterial = rankSMat;
        }
        else if (rank == Rank.A)
        {
            rankText.text = "A";
            rankText.fontMaterial = rankAMat;
        }
        else if (rank == Rank.B)
        {
            rankText.text = "B";
            rankText.fontMaterial = rankBMat;
        }
        else if (rank == Rank.C)
        {
            rankText.text = "C";
            rankText.fontMaterial = rankCMat;
        }
    }

    IEnumerator ShowRank()
    {
        yield return new WaitForSeconds(2.0f);

        rankText.gameObject.SetActive(true);

        yield return new WaitForSeconds(0.08f);

        //rankTextSmoke.SetActive(true);

        //rankText.transform.GetChild(0).gameObject.SetActive(false);

        //rankText.GetComponent<Animator>().SetTrigger("ShowRank");
    }

    


    public bool IsResultsShowing()
    {
        return resultsDisplayed;
    }
}
