using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTransitionCollider : MonoBehaviour
{

    [SerializeField] private static GameObject gameManagerGO;
    private GameCommunicationManager gameManager;
    private GameObject wall;

    [SerializeField] private float nextLevelOrthoSize = 7;

    private bool hasMovedCamera;
    // Start is called before the first frame update
    void Start()
    {

        gameManagerGO = GameObject.Find("GameManager");

        gameManager = gameManagerGO.GetComponent<GameCommunicationManager>();

        if (transform.childCount > 0)
        {
            wall = transform.GetChild(0).gameObject;
        }

        hasMovedCamera = false;
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasMovedCamera == false)
        {
            if (collision.gameObject.tag == "Player")
            {
                gameManager.LoadNextArena(nextLevelOrthoSize, gameObject.transform.position);

                if (collision.gameObject.GetComponent<CharacterMovement>().currentKunai != null)
                {
                    collision.gameObject.GetComponent<CharacterMovement>().currentKunai.GetComponent<Kunai>().CollectKunai();
                    
                }
                hasMovedCamera = true;
            }
        }
        
        if (collision.gameObject.tag == "Kunai")
        {
            collision.gameObject.GetComponent<Kunai>().CollectKunai();
        }

        
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if (collision.gameObject.transform.position.x > gameObject.transform.position.x)
            {
                ActivateWall();
                gameObject.GetComponent<Collider2D>().enabled = false;
            }
        }
        
    }

    void ActivateWall()
    {
        if (wall != null)
        {
            wall.SetActive(true);
        }
        
    }
}
