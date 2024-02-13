using DG.Tweening.Core.Easing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportEnabler : MonoBehaviour
{

    [SerializeField] private GameObject playerGO;
    private CharacterMovement player;

    // Start is called before the first frame update
    void Start()
    {
        playerGO = GameObject.Find("Player");

        player = playerGO.GetComponent<CharacterMovement>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            player.EnableTeleport();
        }
    }
}
