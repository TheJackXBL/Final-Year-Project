using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Barrier : MonoBehaviour
{

    [SerializeField] SpriteRenderer sprite;
    [SerializeField] Collider2D barrierCollider;

    [SerializeField] private bool destroyed = false;

    // Start is called before the first frame update
    void Start()
    {
        barrierCollider = GetComponent<Collider2D>();
        sprite = GetComponent<SpriteRenderer>();
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (destroyed == false)
        {
            if (collision.gameObject.tag == "Player")
            {
                if (collision.gameObject.GetComponent<CharacterMovement>().isDashing)
                {
                    BreakBarrier();
                }
            }
        }
        
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (destroyed == false)
        {
            if (collision.gameObject.tag == "Player")
            {
                if (collision.gameObject.GetComponent<CharacterMovement>().isDashing)
                {
                    BreakBarrier();
                }
            }
        }
    }

    void BreakBarrier()
    {
        barrierCollider.enabled = false;

        Color temp = sprite.color;
        temp.a = 0.3f;
        sprite.color = temp;

        destroyed = true;
    }

}
