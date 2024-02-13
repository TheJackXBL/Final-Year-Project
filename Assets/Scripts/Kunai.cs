using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kunai : MonoBehaviour
{

    public GameObject player;
    public CharacterMovement characterMovement;

    private Collider2D kunaiCollider2D;
    private Rigidbody2D rb;
    [SerializeField] public float moveSpeed;

    [SerializeField] public bool isMoving = true;

    public GameObject triggerCollider;


    [SerializeField] public Vector2 kunaiVec;

    public bool inEnemy = false;

    public Collision2D enemyPinned;

    [SerializeField] private Animator animator;



    // Start is called before the first frame update
    void Start()
    {

        kunaiCollider2D = GetComponent<Collider2D>();

        rb = GetComponent<Rigidbody2D>();

        player = GameObject.FindWithTag("Player");

        characterMovement = player.GetComponent<CharacterMovement>();

        kunaiVec = characterMovement.kunaiVector;

        animator = gameObject.GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //move kunai on instantiation
        if (isMoving)
        {
            rb.velocity = kunaiVec * moveSpeed;
        }
        
    }

    //checks depending on the thing collided (wall/enemy) neeed to be added
    //stops when it hits an object and destroys the collider, leaving the trigger on the child
    private void OnCollisionEnter2D(Collision2D collision)
    {

        

        //stops applying movement
        isMoving = false;

        RumbleManager.instance.RumblePulse(0.1f, 0.1f, 0.1f);

        //activate child object
        triggerCollider.SetActive(true);

        //activate wobble animation
        animator.SetTrigger("Wedge");


        //destroy collider on main parent
        Destroy(kunaiCollider2D);

        if (collision.transform.tag != "Enemy")
        {


            //Destroy(rb);

            //freezes rigidbody to avoid floating away
            rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;

            //slightly wedges kunai into object
            transform.position += new Vector3(kunaiVec.x, kunaiVec.y, 0.0f) * 0.1f;

            

            
        }
        else
        {

            //check if enemy is alive
            if (collision.gameObject.GetComponent<EnemyAI>().IsEnemyAlive())
            {
                inEnemy = true;

                rb.constraints = RigidbodyConstraints2D.FreezeRotation;

                rb.isKinematic = true;

                rb.velocity = new Vector2(0, 0);

                bool directionOfTravel = false; //default to left

                if (transform.position.x > collision.gameObject.transform.position.x)
                {
                    directionOfTravel = false; //hit comes from the right, so player must go left
                }
                else
                {
                    directionOfTravel = true;
                }

                collision.gameObject.GetComponent<EnemyAI>().HitByKunai(directionOfTravel);

                transform.parent = collision.transform;

                

                if (directionOfTravel)
                {
                    transform.eulerAngles = new Vector3(0, 0, -90);
                    transform.localPosition = new Vector3(-0.6f, 0, 0);
                }
                else
                {
                    transform.eulerAngles = new Vector3(0, 0, 90);
                    transform.localPosition = new Vector3(0.6f, 0, 0);
                }

                
            }
            
            

            
        }
        


    }


    //Trigger collider on the child gameObject to pick up the kunai after its landed
    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        if (collision.gameObject == player)
        {
            CollectKunai();
            
        }
    }

    public GameObject GetEnemyInfo()
    {
        return transform.parent.gameObject;
    }



    public void CollectKunai()
    {
        
        //hide kunai
        gameObject.GetComponentInChildren<SpriteRenderer>().enabled = false;

        

        //player can throw another kunai
        //THIS SHOULD BE ALTERED TO USE A BETTER SYSTEM, LIKE A "CAN FIRE KUNAI" BOOL BEING USED INSTEAD

        if (gameObject == characterMovement.currentKunai)
        {
            characterMovement.kunaiFired = false;
        }

        //characterMovement.ResetKunaiThrowCooldown();

        //characterMovement.RemoveKunai(gameObject);


        //destroy kunai after exchanging velocity to the player
        StartCoroutine(DestroyKunai());

        
    }

    IEnumerator DestroyKunai()
    {
        yield return new WaitForSeconds(.1f);

        //destroy kunai
        Destroy(gameObject);

        
    }
}
