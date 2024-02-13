using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class LeaderboardScoreManager : MonoBehaviour
{
    public List<Score> scores;
    private ScoreData sd;

    private void Awake()
    {
        sd = new ScoreData();

        LoadScores();
        //var json = PlayerPrefs.GetString("scores", "{}");
        //sd = JsonUtility.FromJson<ScoreData>(json);
    }

    void LoadScores()
    {
        if (File.Exists(Application.dataPath + "/scores.txt"))
        {
            string saveString = File.ReadAllText(Application.dataPath + "/scores.txt");

            sd = JsonUtility.FromJson<ScoreData>(saveString);
        }
    }

    public IEnumerable<Score> GetHighScores()
    {
        return sd.scores.OrderByDescending(x => x.score);
    }

    public void AddScore(Score score)
    {
        sd.scores.Add(score);
    }

    private void OnDestroy()
    {
        SaveScore();
    }

    public void SaveScore()
    {
        
        var json = JsonUtility.ToJson(sd);

        File.WriteAllText(Application.dataPath + "/scores.txt", json);
        
        //PlayerPrefs.SetString("scores", json);
    }
}
