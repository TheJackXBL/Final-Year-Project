using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

public class EnemyAI : MonoBehaviour
{

    private enum State
    {
        Patrol,
        Stationary,
        Lookout,
        Attack,
        Kunaied,
        Pinned,
        Death
    }
    [SerializeField] private State state;
    [SerializeField] private State startingState;

    private Vector3 startPosition;


    public float moveSpeed;
    public Transform[] patrolPoints;
    public float waitTime;
    int currentPointIndex;

    Vector2 currentPointVec2;
    float patrolAngle;
    Vector2 patrolDirection;

    float roundedXtransform;
    float roundedXpatrolPoint;

    public GameObject detectionCone;


    Rigidbody2D rb;
    bool singleCoroutineBool = false;



    //public bool isFacing = false; //true = right, false = left
    private enum Direction
    {
        Left,
        Right
    }
    [SerializeField] private Direction startingDirection;
    [SerializeField] private Direction currentDirection;

    [SerializeField] private GameObject flippableObjects;

    private GameObject player;
    [SerializeField] Vector2 targetPosition;
    float targetAngle;
    Quaternion targetAngleQuat;
    [SerializeField] Vector2 targetDirection;

    [SerializeField] bool hasLineOfSight;
    public LayerMask ignoreLayers;

    bool isShooting;

    public float fireRate;
    private float fireRateCooldown;

    public float enemyReactionSpeed;
    private float enemyReactionCooldown;

    public GameObject bullet;
    public float bulletSpeed;
    public float bulletLifetime;
    public Transform gunBarrelTransform;

    public BulletPool bulletPool;
    [SerializeField] private GameObject firedBullet;


    public float kunaiForce;


    [SerializeField] bool leftWallPinCheck;
    [SerializeField] bool rightWallPinCheck;
    public Transform leftCheckPoint;
    public Transform rightCheckPoint;


    bool isAlive = true;
    [SerializeField] float deathLaunchSpeed;
    [SerializeField] Vector2 minLaunchDirection;
    [SerializeField] Vector2 maxLaunchDirection;


    [SerializeField] GameObject exclamationMark;

    [SerializeField] ParticleSystem deathBloodParticles;
    [SerializeField] GameObject enemyLockOn;
    [SerializeField] private bool lockOnOnce = false;

    public GameCommunicationManager gameManager;
    public float enemyID;

    [SerializeField] private Canvas enemyCanvas;
    private Vector3 sliderStartingScale;
    [SerializeField] private Slider fireRateCooldownSlider;


    [SerializeField] private Image radialUndetectedTimer;
    [SerializeField] private float undetectedFullTime;
    [SerializeField] private float undetectedRadialAppearTime;
    [SerializeField] private bool isUndetectedDisplayed;
    private float currentUndetectedTime;

    public Animator animator;



    // Start is called before the first frame update
    void Start()
    {

        startPosition = transform.position;

        rb = GetComponent<Rigidbody2D>();

        if (startingState != (State.Lookout) && startingState != State.Stationary)
        {
            currentPointVec2 = new Vector2(patrolPoints[currentPointIndex].position.x, patrolPoints[currentPointIndex].position.y);

            roundedXpatrolPoint = Mathf.Round(currentPointVec2.x * 100f) / 100f;

            patrolDirection = patrolPoints[currentPointIndex].position - transform.position;

            patrolDirection.Normalize();

            
        }

        enemyCanvas.gameObject.SetActive(false);

        sliderStartingScale = fireRateCooldownSlider.transform.localScale;


        if (patrolDirection.x < 0.0f)
        {
            currentDirection = Direction.Left;
        }
        else
        {
            currentDirection = Direction.Right;
        }

        CheckToFlipSprite();

        if (startingDirection == Direction.Left)
        {
            SwapDirection();
        }

        state = startingState;

        player = GameObject.FindWithTag("Player");

        gameManager = GameObject.Find("GameManager").GetComponent<GameCommunicationManager>();

        bulletPool = GameObject.Find("BulletPooler").GetComponent<BulletPool>();

        enemyReactionCooldown = enemyReactionSpeed;
        isShooting = false;

        minLaunchDirection = minLaunchDirection.normalized;
        maxLaunchDirection = maxLaunchDirection.normalized;

        animator.SetBool("Death", !isAlive);

        deathBloodParticles = GetComponent<ParticleSystem>();


        fireRateCooldownSlider.gameObject.SetActive(false);
        UndisplayUndetectedRadial();
        


    }

    // Update is called once per frame
    void FixedUpdate()
    {

        switch (state)
        {
            default:
            case State.Patrol:
                //roundedXtransform = Mathf.Round(transform.position.x * 10f) / 10f;



                if (Mathf.Abs(transform.position.x - currentPointVec2.x) > 0.2f)
                {
                    //rb.velocity = Vector2.MoveTowards(transform.position, patrolPoints[currentPointIndex].position, moveSpeed);



                    //patrolAngle = Vector2.SignedAngle(Vector2.up, patrolDirection);

                    //patrolDirection = new Vector2(Mathf.Cos(patrolAngle * Mathf.Deg2Rad), Mathf.Sin(patrolAngle * Mathf.Deg2Rad));

                    //patrolDirection = patrolDirection.normalized;

                    rb.velocity = new Vector2(patrolDirection.x * moveSpeed, rb.velocity.y);
                }
                else
                {
                    if (singleCoroutineBool == false)
                    {
                        rb.velocity = new Vector2(0.0f, 0.0f);
                        singleCoroutineBool = true;
                        StartCoroutine(Wait());
                    }

                }

                animator.SetFloat("EnemySpeed", Mathf.Abs(rb.velocity.x));

                break;
            case State.Stationary:
                if (singleCoroutineBool == false)
                {
                    rb.velocity = new Vector2(0.0f, 0.0f);
                    singleCoroutineBool = true;
                    StartCoroutine(Pivot(waitTime));
                }

                break;
            case State.Lookout:
                

                break;
            case State.Attack:

                targetPosition = new Vector2(player.transform.position.x, player.transform.position.y);



                targetDirection = targetPosition - new Vector2(transform.position.x, transform.position.y);

                targetDirection = targetDirection.normalized;

                if (fireRateCooldown > 0)
                {
                    fireRateCooldown -= Time.deltaTime;
                }


                RaycastHit2D hit = Physics2D.Raycast(transform.position, targetDirection, 15f, ~ignoreLayers);

                if (hit.collider != null)
                {
                    if (hit.collider.tag == "Player")
                    {
                        hasLineOfSight = true;
                    }
                    else
                    {
                        hasLineOfSight = false;

                        if (currentUndetectedTime > 0)
                        {
                            currentUndetectedTime -= Time.deltaTime;
                        }
                        else
                        {
                            ReturnToIdle();
                        }
                    }

                    
                }

                if (currentUndetectedTime < 5)
                {
                    if (isUndetectedDisplayed == false)
                    {
                        DisplayUndetectedRadial();
                    }

                    radialUndetectedTimer.fillAmount = currentUndetectedTime / undetectedRadialAppearTime;
                }

                if (hasLineOfSight == true)
                {
                    if (enemyReactionCooldown < 0)
                    {
                        if (fireRateCooldown <= 0)
                        {
                            Fire();
                        }
                    }
                    else
                    {
                        enemyReactionCooldown -= Time.deltaTime;
                    }

                    if (lockOnOnce == false)
                    {
                        lockOnOnce = true;
                        enemyLockOn.SetActive(true);
                        //enemyLockOn.GetComponent<Animator>().Play("Enemy Lock-On");
                    }

                    if (currentUndetectedTime != undetectedFullTime)
                    {
                        currentUndetectedTime = undetectedFullTime;

                        UndisplayUndetectedRadial();
                    }

                    if (targetDirection.x < 0 && currentDirection == Direction.Right)
                    {
                        currentDirection = Direction.Left;

                        CheckToFlipSprite();
                    }
                    else if (targetDirection.x > 0 && currentDirection == Direction.Left)
                    {
                        currentDirection = Direction.Right;

                        CheckToFlipSprite();
                    }
                    

                }
                else
                {
                    if (enemyReactionCooldown != enemyReactionSpeed)
                    {
                        enemyReactionCooldown = enemyReactionSpeed;
                    }

                    if (lockOnOnce == true)
                    {
                        lockOnOnce = false;
                        enemyLockOn.SetActive(false);
                    }
                }

                if (fireRateCooldownSlider.gameObject.activeSelf == true)
                {

                    fireRateCooldownSlider.value = 1 - (fireRateCooldown / fireRate);

                    if (fireRateCooldown < 0)
                    {
                        fireRateCooldownSlider.gameObject.SetActive(false);
                    }
                }



                break;
            case State.Kunaied:


                break;
            case State.Pinned: 
                

                break;
            case State.Death:


                break;
        }


    }


    void ReturnToIdle()
    {
        state = startingState;

        detectionCone.SetActive(true);

        if (state == State.Patrol)
        {
            if (patrolDirection.x < 0)
            {
                currentDirection = Direction.Left;
            }
            else
            {
                currentDirection = Direction.Right;
            }
        }

        CheckToFlipSprite();
    }

    IEnumerator Wait()
    {
        yield return new WaitForSeconds(waitTime);

        if (isAlive)
        {
            if (currentPointIndex + 1 < patrolPoints.Length)
            {
                currentPointIndex++;


            }
            else
            {
                currentPointIndex = 0;
            }

            currentPointVec2 = new Vector2(patrolPoints[currentPointIndex].position.x, patrolPoints[currentPointIndex].position.y);

            patrolDirection = patrolPoints[currentPointIndex].position - transform.position;

            patrolDirection.Normalize();

            roundedXpatrolPoint = Mathf.Round(currentPointVec2.x * 10f) / 10f;

            SwapDirection();
        }
        
    }

    IEnumerator Pivot(float waitingTime)
    {

        if (waitingTime != 0)
        {
            waitingTime = waitTime;
        }

        yield return new WaitForSeconds(waitingTime);

        SwapDirection();

        

    }

    void SwapDirection()
    {
        if (currentDirection == Direction.Left)
        {
            currentDirection = Direction.Right;
        }
        else
        {
            currentDirection = Direction.Left;
        }

        CheckToFlipSprite();

        singleCoroutineBool = false;

        //if (fireRateCooldownCanvas.gameObject.activeSelf == true)
        //{
        //    AdjustSlider();
        //}
        
    }

   

    void CheckToFlipSprite()
    {
        if (currentDirection == Direction.Right)
        {
            flippableObjects.transform.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            flippableObjects.transform.localScale = new Vector3(-1, 1, 1);
        }

    }

    //void AdjustSlider()
    //{
    //    if (currentDirection == Direction.Left)
    //    {
    //        fireRateCooldownSlider.transform.localScale = new Vector3(-sliderStartingScale.x, sliderStartingScale.y, sliderStartingScale.z);
    //    }
    //    else
    //    {
    //        fireRateCooldownSlider.transform.localScale = sliderStartingScale;
    //    }
    //}

    void Detected()
    {
        state = State.Attack;

        enemyReactionCooldown = 0.5f;

        detectionCone.SetActive(false);


        currentUndetectedTime = undetectedFullTime;

        enemyCanvas.gameObject.SetActive(true);

    }


    void Fire()
    {

        targetAngle = Vector2.SignedAngle(Vector2.up, targetDirection);

        targetAngleQuat = Quaternion.Euler(0, 0, targetAngle);

        //GameObject firedBullet = Instantiate(bullet, gunBarrelTransform.position, targetAngleQuat);

        firedBullet = bulletPool.Spawn();

        

        firedBullet.transform.position = gunBarrelTransform.position;
        firedBullet.transform.rotation = targetAngleQuat;

        //to avoid moments when the trail jumps across the screen
        firedBullet.GetComponent<TrailRenderer>().Clear();

        firedBullet.GetComponent<Rigidbody2D>().velocity = targetDirection * bulletSpeed;
        


        fireRateCooldown = fireRate;

        isShooting = true;
        animator.SetBool("Shooting", isShooting);

        StartCoroutine(StopShootAnim());

        fireRateCooldownSlider.gameObject.SetActive(true);


        //Destroy(firedBullet, bulletLifetime);
    }

    IEnumerator StopShootAnim()
    {
        yield return new WaitForSeconds(0.417f);

        isShooting = false;
        animator.SetBool("Shooting", isShooting);
    }

    public void HitByKunai(bool directionOfTravel)
    {
        rb.velocity = Vector2.zero;

        //remove detection cone
        Detected();

        Direction tempDirectionCheck = Direction.Left;

        if (directionOfTravel)
        {
            tempDirectionCheck = Direction.Left;
        }
        else
        {
            tempDirectionCheck = Direction.Right;
        }

        //flip sprite so the character is facing away from their direction of travel
        if (tempDirectionCheck != currentDirection)
        {
            SwapDirection();
        }

        //add force to send them flying
        if (directionOfTravel)
        {
            rb.AddForce(Vector2.right * kunaiForce);
        }
        else
        {
            rb.AddForce(Vector2.left * kunaiForce);
        }

        state = State.Kunaied;
        
    }

    void Pin()
    {
        state = State.Pinned;
    }

    public void Death()
    {

        Detected();

        state = State.Death;

        Vector2 deathDirection = RandomDeathDirection(minLaunchDirection, maxLaunchDirection);

        rb.velocity = deathDirection * deathLaunchSpeed;

        isAlive = false;

        animator.SetBool("Death", !isAlive);

        deathBloodParticles.Play();

        gameManager.UpdateCombo();

        gameManager.CheckForEnemies(gameObject.transform.parent.gameObject.GetComponent<EnemyChecker>());

        gameObject.layer = 22;

        enemyCanvas.gameObject.SetActive(false);

    }

    public void ResetEnemy()
    {
        
        state = startingState;

        currentDirection = startingDirection;

        

        transform.position = startPosition;

        currentPointIndex = 0;

        if (startingState != (State.Lookout) && startingState != State.Stationary)
        {
            currentPointVec2 = new Vector2(patrolPoints[currentPointIndex].position.x, patrolPoints[currentPointIndex].position.y);

            roundedXpatrolPoint = Mathf.Round(currentPointVec2.x * 100f) / 100f;

            patrolDirection = patrolPoints[currentPointIndex].position - transform.position;

            patrolDirection.Normalize();

            if (patrolDirection.x < 0.0f)
            {
                currentDirection = Direction.Left;
            }
            else
            {
                currentDirection = Direction.Right;
            }


        }

        

        CheckToFlipSprite();

        isAlive = true;

        animator.SetBool("Death", !isAlive);

        deathBloodParticles.Stop();

        detectionCone.SetActive(true);

        fireRateCooldown = -0.1f;
        enemyReactionCooldown = enemyReactionSpeed;

        gameObject.layer = 9;

        if (firedBullet != null)
        {
            bulletPool.KillBullet(firedBullet);
        }

        enemyCanvas.gameObject.SetActive(false);
    }

    private Vector2 RandomDeathDirection(Vector2 min, Vector2 max)
    {
        return Vector2.Lerp(min, max, Random.value).normalized;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        


        if (collision.gameObject.tag == "Player" && isAlive == true)
        {
            
            Detected();
        }
    }

    public void DetectedByCone()
    {
        GameObject mark = Instantiate(exclamationMark, transform.position + new Vector3(0, 2f, 0), Quaternion.identity);

        Destroy(mark, 1.1f);
        Detected();
    }

    void DisplayUndetectedRadial()
    {
        radialUndetectedTimer.gameObject.SetActive(true);

        isUndetectedDisplayed = true;
    }

    void UndisplayUndetectedRadial()
    {
        if (radialUndetectedTimer != null)
        {
            radialUndetectedTimer.gameObject.SetActive(false);
        }
        
        

        isUndetectedDisplayed = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //if travelling via kunai
        if (state == State.Kunaied)
        {

            //check walls and pin if a wall was collided with
            leftWallPinCheck = Physics2D.OverlapCircle(leftCheckPoint.position, 0.1f, LayerMask.GetMask("Ground"));

            rightWallPinCheck = Physics2D.OverlapCircle(rightCheckPoint.position, 0.1f, LayerMask.GetMask("Ground"));

            if (leftWallPinCheck || rightWallPinCheck)
            {
                Pin();
            }
        }
        
    }

    public bool IsEnemyAlive()
    {
        return isAlive;
    }
}
