using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Score
{
    public string name;
    public float score;
    public string time;

    public Score(string name, float time, float score)
    {
        this.name = name;
        this.time = CalculateDisplayedTime(time);
        this.score = score;
        
    }

    string CalculateDisplayedTime(float time)
    {
        float minutes = Mathf.Floor(time / 60);
        float seconds = time % 60;
        float milliseconds = ((time - (Mathf.Floor(time))) * 100);

        string tempString = minutes + "." + seconds.ToString("00") + "." + milliseconds.ToString("00");

        return tempString;
    }
}
