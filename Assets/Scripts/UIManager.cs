using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
//using TMPro.EditorUtilities;
using UnityEngine.EventSystems;

public class UIManager : MonoBehaviour
{
    public GameObject player;
    public CharacterMovement characterMovement;



    public Slider slowdownSlider;
    private float slowdownValue;


    public Image kunaiAvailabilityImage;
    private bool isKunaiAvailable;
    public Color kunaiImageColor;
    private Color kunaiImageUnavailableColor;
    public float kunaiImageUnavailableAlpha;



    public TextMeshProUGUI comboMultiplierGUI;

    [SerializeField] private Image radialComboTimerUI;
    [SerializeField] private float indicatorTimer = 1.0f;
    [SerializeField] private float maxIndicatorTimer = 1.0f;
    [SerializeField] private float indicatorTimerLimiter = 0.7f;


    public TextMeshProUGUI scoreDisplayGUI;
    private string scoreDisplayed;


    [SerializeField] private Image playerDeathPopup;


    [SerializeField] private Image screenWipeImage;
    [SerializeField] private float startingScreenWipeX;
    [SerializeField] private float timeToWipe;




    [SerializeField] private GameObject comboFC;

    [SerializeField] private GameObject resultsGO;

    [SerializeField] private GameObject pauseGO;


    [SerializeField] private EventSystem eventSystem;
    [SerializeField] private Button resumeButton;




    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player");

        characterMovement = player.GetComponent<CharacterMovement>();

        kunaiImageColor = kunaiAvailabilityImage.color;

        kunaiImageUnavailableColor = kunaiImageColor;

        kunaiImageUnavailableColor.a = kunaiImageUnavailableAlpha;

        comboFC.SetActive(true);

        playerDeathPopup.gameObject.SetActive(false);

        //rankText.fontMaterial = rankCMat;

        //rankText.m = rankCMat;

        startingScreenWipeX = screenWipeImage.gameObject.GetComponent<RectTransform>().localPosition.x;

        pauseGO.SetActive(false);

        ScreenWipeOut();

    }

    // Update is called once per frame
    void FixedUpdate()
    {

        if (slowdownSlider != null)
        {
            slowdownValue = characterMovement.slowdownRatio;


            slowdownSlider.value = slowdownValue;
        }
        


        //isKunaiAvailable = characterMovement.kunaiFired;

        //if (isKunaiAvailable == true)
        //{
        //    kunaiAvailabilityImage.color = kunaiImageColor;
        //}
        //else
        //{
        //    kunaiAvailabilityImage.color = kunaiImageUnavailableColor;
        //}

        

    }

    public void UpdateKunaiUI(float currentCooldown, float maxCooldown)
    {
        kunaiAvailabilityImage.fillAmount = 1 - (currentCooldown/maxCooldown);
    }

    public void UpdateComboUI(float comboMultiplier)
    {
        comboMultiplierGUI.text = comboMultiplier.ToString();
    }

    public void UpdateComboTimerUI(float comboTimer, float comboCooldownMax)
    {
        radialComboTimerUI.fillAmount = (comboTimer / comboCooldownMax) * indicatorTimerLimiter;
    }

    public void UpdateScoreUI(float score)
    {

        scoreDisplayed = score.ToString("000000");

        scoreDisplayGUI.text = scoreDisplayed;

    }

    public void DropComboFC()
    {
        comboFC.SetActive(false);
    }

    public void PlayerDeath()
    {
        playerDeathPopup.gameObject.SetActive(true);
    }

    public void ResetLevel()
    {
        playerDeathPopup.gameObject.SetActive(false);
    }

    public void ShowResultsScreen()
    {
        resultsGO.SetActive(true);
    }

    public void ShowPauseScreen()
    {
        eventSystem.SetSelectedGameObject(resumeButton.gameObject);
        eventSystem.sendNavigationEvents = true;

        pauseGO.SetActive(true);
    }

    public void HidePauseScreen()
    {
        eventSystem.sendNavigationEvents = false;
        pauseGO.SetActive(false);
    }


    public void ScreenWipeIn()
    {
        StartCoroutine(MoveInScreenWiper());
    }

    public void ScreenWipeOut()
    {
        StartCoroutine(MoveOutScreenWiper());
    }

    IEnumerator MoveInScreenWiper()
    {

        screenWipeImage.gameObject.SetActive(true);

        screenWipeImage.gameObject.GetComponent<RectTransform>().localPosition = new Vector3(startingScreenWipeX, 0f, 0f);

        // Loop until the alpha reaches 1
        float elapsedTime = 0f;

        float currentX = screenWipeImage.gameObject.GetComponent<RectTransform>().localPosition.x;

        while (screenWipeImage.gameObject.GetComponent<RectTransform>().localPosition.x > 0)
        {
            float newX = Mathf.Lerp(currentX, 0.0f, elapsedTime / timeToWipe);

            screenWipeImage.gameObject.GetComponent<RectTransform>().localPosition = new Vector3(newX, 0f, 0f);

            elapsedTime += Time.deltaTime;

            yield return null;
        }
    }

    IEnumerator MoveOutScreenWiper()
    {

        screenWipeImage.gameObject.SetActive(true);

        screenWipeImage.gameObject.GetComponent<RectTransform>().localPosition = new Vector3(0f, 0f, 0f);

        // Loop until the alpha reaches 1
        float elapsedTime = 0f;

        float currentX = screenWipeImage.gameObject.GetComponent<RectTransform>().localPosition.x;

        while (screenWipeImage.gameObject.GetComponent<RectTransform>().localPosition.x > -startingScreenWipeX)
        {
            float newX = Mathf.Lerp(currentX, -startingScreenWipeX, elapsedTime / timeToWipe);

            screenWipeImage.gameObject.GetComponent<RectTransform>().localPosition = new Vector3(newX, 0f, 0f);

            elapsedTime += Time.deltaTime;

            yield return null;
        }
    }

}
