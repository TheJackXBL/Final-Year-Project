using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathPit : MonoBehaviour
{
    [SerializeField] private GameCommunicationManager gameManager;
    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameCommunicationManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            gameManager.ResetLevel();
        }

        if (collision.gameObject.tag == "Kunai")
        {
            collision.gameObject.GetComponent<Kunai>().CollectKunai();
        }
    }
}
