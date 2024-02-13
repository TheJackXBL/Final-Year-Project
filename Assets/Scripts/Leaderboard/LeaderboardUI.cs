using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LeaderboardUI : MonoBehaviour
{

    public RowUI rowUI;
    public LeaderboardScoreManager scoreManager;

    // Start is called before the first frame update
    void Start()
    {
        //scoreManager.AddScore(new Score("Jack", 2500.42f, 1400));
        //scoreManager.AddScore(new Score("Jasmin", 300, 10000));

        var scores = scoreManager.GetHighScores().ToArray();

        for (int i = 0; i < scores.Length; i++)
        {
            var row = Instantiate(rowUI, transform).GetComponent<RowUI>();

            row.rank.text = (i + 1).ToString();
            row.playerName.text = scores[i].name;
            row.time.text = scores[i].time;
            row.score.text = scores[i].score.ToString();
        }
    }

    
}
