using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StageSelectMenuManager : MonoBehaviour
{

    [SerializeField] private EventSystem eventSystem;

    [SerializeField] private Button tutorialButton;
    [SerializeField] private Button cyberpunkButton;

    [SerializeField] private GameObject notAvailableSign;
    [SerializeField] private GameObject leaderboard;
    private bool naOnceBool;

    // Start is called before the first frame update
    void Start()
    {
        naOnceBool = false;
    }

    private void FixedUpdate()
    {
        if (eventSystem.currentSelectedGameObject == tutorialButton.gameObject)
        {
            if (naOnceBool == false)
            {
                naOnceBool = true;
                notAvailableSign.SetActive(true);
                leaderboard.SetActive(false);
            }
        }
        else
        {
            if (naOnceBool == true)
            {
                naOnceBool = false;
                notAvailableSign.SetActive(false);
                leaderboard.SetActive(true);
            }
        }
    }

    private void OnEnable()
    {
        eventSystem.SetSelectedGameObject(cyberpunkButton.gameObject);

        notAvailableSign.SetActive(false);
        naOnceBool = false;
    }
}
