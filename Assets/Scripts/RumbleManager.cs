using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RumbleManager : MonoBehaviour
{

    public static RumbleManager instance;

    private Gamepad pad;

    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void RumblePulse(float lowFrequency, float highFrequency, float duration)
    {
        pad = Gamepad.current;

        if (pad != null)
        {
            pad.SetMotorSpeeds(lowFrequency, highFrequency);

            StartCoroutine(StopRumble(duration));
        }
    }

    private IEnumerator StopRumble(float duration)
    {
        yield return new WaitForSeconds(duration);

        pad.SetMotorSpeeds(0.0f, 0.0f);
    }

    
}
