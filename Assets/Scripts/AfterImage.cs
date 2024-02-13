using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AfterImage : MonoBehaviour
{
    [SerializeField]
    private float activeTimeMax = 0.35f;
    private float activeTimeRemaining;
    [SerializeField]
    private float activeTimeThresholdBeforeFade;

    private float alpha;

    [SerializeField]
    private float alphaSet = 0.8f;
    [SerializeField]
    private float alphaMultiplier = 0.85f;

    private Transform player;

    private SpriteRenderer SR;
    private SpriteRenderer playerSR;

    [SerializeField]
    private Color baseColor;
    private Color color;


    private GameObject afterImagePoolGO;
    private AfterImagePool afterImagePool;


    private void OnEnable()

    {

        afterImagePoolGO = GameObject.Find("AfterImagePooler");
        afterImagePool = afterImagePoolGO.GetComponent<AfterImagePool>();

        SR = GetComponent<SpriteRenderer>();
        player = GameObject.FindGameObjectWithTag("Player").transform; playerSR = player.GetComponentInChildren<SpriteRenderer>();
        alpha = alphaSet;
        activeTimeRemaining = activeTimeMax;
        SR.sprite = playerSR.sprite;
        SR.flipX = playerSR.flipX;
        transform.position = player.position;
        transform.rotation = playerSR.gameObject.transform.rotation;
        color = baseColor;
        SR.color = color;
    }

    private void FixedUpdate()
    {
        

        if (activeTimeRemaining > 0)
        {
            activeTimeRemaining -= Time.deltaTime;
        }
        else
        {
            afterImagePool.KillAfterImage(gameObject);
        }

        if (activeTimeRemaining < activeTimeThresholdBeforeFade)
        {
            alpha *= alphaMultiplier;
            color.a = alpha;
            SR.color = color;
        }
    }
}
