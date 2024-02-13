using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelComplete : MonoBehaviour
{

    [SerializeField] private static GameObject gameManagerGO;
    private GameCommunicationManager gameManager;
    [SerializeField] private ScoreManager scoreManager;

    // Start is called before the first frame update
    void Start()
    {
        gameManagerGO = GameObject.Find("GameManager");

        gameManager = gameManagerGO.GetComponent<GameCommunicationManager>();


    }

    

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            if (SceneManager.GetActiveScene().name == "Tutorial")
            {
                gameManager.NextLevel();
            }
            else
            {
                
                gameManager.EndLevel();
            }



        }
    }
}
