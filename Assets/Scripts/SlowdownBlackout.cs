using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlowdownBlackout : MonoBehaviour
{

    [SerializeField] private Image slowdownImage;

    private float defaultAlpha;

    [SerializeField] private float timeToFade;

    // Start is called before the first frame update
    void Start()
    {
        defaultAlpha = slowdownImage.color.a;

        slowdownImage.color = new Color(slowdownImage.color.r, slowdownImage.color.g, slowdownImage.color.b, 0.0f);
    }


    public void FadeInSlow()
    {
        StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn()
    {

        slowdownImage.color = new Color(slowdownImage.color.r, slowdownImage.color.g, slowdownImage.color.b, 0.0f);

        // Loop until the alpha reaches 1
        float elapsedTime = 0f;

        float currentAlpha = slowdownImage.color.a;

        while (slowdownImage.color.a < defaultAlpha)
        {
            float newAlpha = Mathf.Lerp(currentAlpha, defaultAlpha, elapsedTime / timeToFade);

            slowdownImage.color = new Color(slowdownImage.color.r, slowdownImage.color.g, slowdownImage.color.b, newAlpha);

            elapsedTime += Time.deltaTime;

            yield return null;
        }
        
    }

    public void FadeOutSlow()
    {
        StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut()
    {

        slowdownImage.color = new Color(slowdownImage.color.r, slowdownImage.color.g, slowdownImage.color.b, defaultAlpha);

        // Loop until the alpha reaches 1
        float elapsedTime = 0f;

        float currentAlpha = slowdownImage.color.a;

        while (slowdownImage.color.a != 0.0f)
        {
            float newAlpha = Mathf.Lerp(currentAlpha, 0.0f, elapsedTime / timeToFade);

            slowdownImage.color = new Color(slowdownImage.color.r, slowdownImage.color.g, slowdownImage.color.b, newAlpha);

            elapsedTime += Time.deltaTime;

            yield return null;
        }

    }
}
