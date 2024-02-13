using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightManager : MonoBehaviour
{

    //[SerializeField] private GameObject[] allLights;
    private int numberOfLights;
    [SerializeField] private bool startingLight;

    // Start is called before the first frame update
    void Start()
    {
        numberOfLights = transform.childCount;

        if (startingLight)
        {
            gameObject.SetActive(true);
        }
        else
        {
            for (int i = 0; i < numberOfLights; i++)
            {
                transform.GetChild(i).gameObject.SetActive(false);
            }
        }
        
    }

    public void ShowNextLights()
    {
        for (int i = 0; i < numberOfLights; i++)
        {
            transform.GetChild(i).gameObject.SetActive(true);
        }
    }

    public void HideCurrentLights()
    {
       gameObject.SetActive(false);
    }
}
