using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
//using TMPro.EditorUtilities;
using UnityEngine.EventSystems;
using Cinemachine;

public class MainMenuManager : MonoBehaviour
{

    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject stageSelectMenu;
    [SerializeField] private GameObject creditsMenu;

    [SerializeField] private CinemachineVirtualCamera _cameraVCAM;
    [SerializeField] private Vector3 mainMenuCameraPosition;
    [SerializeField] private Transform stageSelectTransform;
    [SerializeField] private Vector3 stageSelectCameraPosition;

    [SerializeField] private Image screenWipeImage;
    [SerializeField] private float startingScreenWipeX;
    [SerializeField] private float timeToWipe;

    [SerializeField] private EventSystem eventSystem;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button creditsResumeButton;




    // Start is called before the first frame update
    void Start()
    {

        mainMenuCameraPosition = _cameraVCAM.transform.position;

        ShowMainMenu();

        startingScreenWipeX = screenWipeImage.gameObject.GetComponent<RectTransform>().localPosition.x;

        ScreenWipeOut();

        eventSystem.sendNavigationEvents = true;

        eventSystem.SetSelectedGameObject(resumeButton.gameObject);


        stageSelectCameraPosition = stageSelectTransform.position;


    }

    private void OnEnable()
    {
        eventSystem.SetSelectedGameObject(resumeButton.gameObject);
    }

    public void ShowMainMenu()
    {

        HideAllScreens();
        mainMenu.SetActive(true);

        _cameraVCAM.transform.position = mainMenuCameraPosition;

        eventSystem.SetSelectedGameObject(resumeButton.gameObject);

    }

    public void ShowStageSelect()
    {
        
        HideAllScreens();
        stageSelectMenu.SetActive(true);

        _cameraVCAM.transform.position = stageSelectCameraPosition;
    }

    public void ShowCredits()
    {

        HideAllScreens();
        creditsMenu.SetActive(true);

        //_cameraVCAM.transform.position = mainMenuCameraPosition;

        eventSystem.SetSelectedGameObject(creditsResumeButton.gameObject);

    }



    void HideAllScreens()
    {
        mainMenu.SetActive(false);
        stageSelectMenu.SetActive(false);
        creditsMenu.SetActive(false);
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
