using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Message : MonoBehaviour
{

    [SerializeField] GameObject messageTriggerArea;
    [SerializeField] GameObject message;

    [SerializeField] string messageText;

    [SerializeField] TextMeshProUGUI tmText;
    [SerializeField] float fadeTime;

    private float t;
    private int latestLetter = 1;

    // Start is called before the first frame update
    void Start()
    {
        messageTriggerArea = transform.GetChild(0).gameObject;
        message = transform.GetChild(1).gameObject;

        

        tmText.text = messageText;
        

    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            DisplayMessage();
        }
    }


    private void DisplayMessage()
    {
        messageTriggerArea.SetActive(false);

        message.SetActive(true);

        StartCoroutine(FadeInText());
    }

    IEnumerator FadeInText()
    {

        for(int i = 0; i < messageText.Length + 1; i++)
        {
            tmText.text = messageText.Substring(0, i);
            yield return new WaitForSeconds(fadeTime);
        }


        //for (int i = 0; i < latestLetter; i++)
        //{
        //    Color color = tmText.textInfo.characterInfo[i].color;
        //    float alpha = Mathf.Lerp(0.0f, 1.0f, t);
        //    color.a = alpha;
        //    tmText.textInfo.characterInfo[i].color = color;
        //    yield return new WaitForSeconds(fadeTime);
        //}

        //if (tmText.textInfo.characterInfo[latestLetter - 1].color.a > 0.3f && latestLetter < tmText.text.Length - 1)
        //{
        //    latestLetter = latestLetter + 1;
        //}

        //t = t + 0.1f;

        
    }
}
