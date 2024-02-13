using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
//using UnityEditor.Experimental.GraphView;
//using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.SceneManagement;
using UnityEngine.Windows;

public class CharacterMovement : MonoBehaviour
{

    //private float gravityValue = 9.81f;

    [Header("Move and Jump")]
    public float moveSpeed = 5.0f;
	public bool isGrounded = false;
	public Transform groundCheckPoint;
	public float jumpHeight = 6.0f;





	public float hangTime = 0.2f;
	private float hangCounter;

	public float jumpBufferLength = 0.1f;
	private float jumpBufferCounter;

	private bool groundedOnce = false;

    private Rigidbody2D rb;
	[SerializeField] private float inputX;
    [SerializeField] private float inputY;

	[Header("Dash")]
	private bool canDash;
    public float dashTime = 0.2f;
    private float dashCounter;
	public bool isDashing = false;
	public bool dashUsedBool = true;

	public float dashCooldownTime = 0.5f;
	private float dashCooldownCounter;
	
	public float afterImageDistance;
	private Vector2 afterImageLastImagePos;
	private AfterImagePool afterImagePool;

	private bool dashCheckOnce;


	public Vector2 directionVec;
	private float lerpDashToNormalDirection;


    [Header("Recovery Speeds")]

    public float groundRecoverySpeed = 0.25f;
	public float airRecoverySpeed = 0.6f;
    public float dashAirRecoverySpeed = 1.25f;

    private float recoveryTime = 2.0f;


    Tweener movementTween;
    TweenParams tweenParams = new TweenParams().SetId("movement");




    [Header("Aiming")]
	public Vector2 aimVector;
    private float aimInputX;
    private float aimInputY;
    private Vector3 defaultAimVector;
	public GameObject aimArrow;
	public RaycastHit2D aimLaserRaycast;
	public bool isAiming;




    [Header("Kunai")]
    private bool canTeleport;
    public GameObject kunaiObject;
	public bool kunaiFired;
	public Vector2 kunaiVector;

	public float kunaiCooldown;
	private float kunaiCooldownCounter;

	[SerializeField] GameObject kunaiPopup;
	private bool kunaiPopupOnce = false;

	private List<GameObject> kunaiList = new List<GameObject>();

	public float maxKunai;



    [Header("Slowdown")]
    public bool isSlowing = false;
	public bool canSlowdown = true;
	public float slowdownTime;
    private float slowdownTimeRemaining;
	public float slowdownRatio;
	private bool slowdownOnce;

    [Header("Cling")]
    public bool isLeftWall = false;
    public bool isRightWall = false;
    public bool isCeiling = false;

    public Transform leftCheckPoint;
    public Transform rightCheckPoint;
    public Transform ceilingCheckPoint;

	public bool isClinging = false;
	Vector3 clingPosition;
	bool isClingJumping = false;
	Vector3 clingJumpDirection;
    public float clingJumpTime = 0.15f;
    private float clingJumpCounter;

    [Header("Teleport")]

    public GameObject currentKunai;
	public bool isTeleporting = false;



	private Vector3 playerSpawn;
	[SerializeField] private bool isAlive = true;
    [SerializeField] float deathLaunchSpeed;
    [SerializeField] Vector2 minLaunchDirection;
    [SerializeField] Vector2 maxLaunchDirection;
	[SerializeField] private float deathCooldown;



    [Header("Sprites / VFX")]

    public Animator animator;
    private enum Direction
    {
        Left,
        Right
    }
    [SerializeField] private Direction startingDirection;
    [SerializeField] private Direction currentDirection;

    public GameObject playerSpriteObject;
	public ParticleSystem landingSmokeVFX;
	public ParticleSystem wallJumpParticles;
	public ParticleSystem teleportSmokeVFX;

    

    [Header("Manager")]
    public GameCommunicationManager gameManager;

	[SerializeField] private PlayerInput playerInput;

    [SerializeField] private SlowdownBlackout slowdownBlackout;


    private void Start()
    {

		playerInput = GetComponent<PlayerInput>();

		EnableMovement();

        rb = GetComponent<Rigidbody2D>();

        defaultAimVector = aimArrow.transform.localPosition;

		slowdownTimeRemaining = slowdownTime;

        afterImagePool = GameObject.Find("AfterImagePooler").GetComponent<AfterImagePool>();

		playerSpawn = gameObject.transform.position;

		if (SceneManager.GetActiveScene().name == "Tutorial")
		{
			SetTutorial();
		}

        gameManager = GameObject.Find("GameManager").GetComponent<GameCommunicationManager>();

        isAlive = true;

        animator.SetBool("isAlive", isAlive);

        //DOTween.Init();

        startingDirection = Direction.Right;
		currentDirection = startingDirection;

    }

	void FixedUpdate()
	{

		if (aimVector == Vector2.zero)
		{
			//aimArrow.GetComponent<SpriteRenderer>().enabled = false;
			isAiming = false;
		}
		else
		{
			//aimArrow.GetComponent<SpriteRenderer>().enabled = true;
			isAiming = true;
		}

		if (kunaiCooldownCounter > 0)
		{
			kunaiCooldownCounter -= Time.deltaTime;
			gameManager.UpdateKunaiUI(kunaiCooldownCounter, kunaiCooldown);
		}
		else
		{
			if (kunaiPopupOnce == false)
			{
				kunaiPopupOnce= true;
				ResetKunaiCooldown();
			}
		}

		if (isSlowing == true)
		{
			slowdownTimeRemaining -= Time.unscaledDeltaTime;
		}
		else
		{
			if (slowdownTimeRemaining < slowdownTime)
			{
                slowdownTimeRemaining += Time.unscaledDeltaTime;
				
				if (slowdownTimeRemaining > slowdownTime)
				{
					slowdownTimeRemaining = slowdownTime;

                }
				
            }
            
        }


		if (slowdownTimeRemaining <= 0)
		{
			canSlowdown= false;
			Time.timeScale = 1.0f;

			isSlowing = false;

			slowdownTimeRemaining = 0;

            if (slowdownOnce)
            {
                slowdownBlackout.FadeOutSlow();
                slowdownOnce = false;
            }


        }


		slowdownRatio = slowdownTimeRemaining / slowdownTime;



		//check if dash can be used again
		if (dashCooldownCounter >= 0) 
		{ 
			dashCooldownCounter -= Time.deltaTime;
		}


		//check to continue dash
        if (dashCounter > 0)
        {
            dashCounter -= Time.deltaTime;

            //after image code HERE

			if ((Vector2.Distance(new Vector2(transform.position.x, transform.position.y), afterImageLastImagePos)) > afterImageDistance)
			{
                GameObject afterImage = afterImagePool.Spawn();
				afterImageLastImagePos = new Vector2(transform.position.x, transform.position.y);

				
            }

            


        }
        else if (dashCheckOnce == false)
        {
            isDashing = false;
			dashCheckOnce = true;

			if (isClinging == false)
			{
                playerSpriteObject.transform.eulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
            }
        }

        //check to continue cling jump
        if (clingJumpCounter > 0)
        {
            clingJumpCounter -= Time.deltaTime;
        }
        else
        {
            isClingJumping = false;
        }

        //reset dash
        //set turning mobility depending on if grounded or airborne
        if (isGrounded)
        {
            dashUsedBool = false;
			recoveryTime = groundRecoverySpeed;
        }
		else
		{
			recoveryTime = airRecoverySpeed;
		}

		//slows down turning speed shortly after dash
		if (dashCooldownCounter >= 0)
		{
			recoveryTime = dashAirRecoverySpeed;
		}

        if (isDashing == false && isClingJumping == false)
		{
			//lerpDashToNormalDirection = Mathf.Lerp(rb.velocity.x, inputX * moveSpeed, recoveryTime);

			

			//movementTween = DOTween.To(() => lerpDashToNormalDirection, x => lerpDashToNormalDirection = x, inputX * moveSpeed, recoveryTime).SetAs(tweenParams);
			//movementTween.Play();


			//if player has just teleported (in order to conserve momentum
			if (isTeleporting == true)
			{
				//if kunai is moving, set velocity to kunai's velocity
                if (currentKunai.GetComponent<Kunai>().isMoving == true)
                {
                    rb.velocity = currentKunai.GetComponent<Kunai>().kunaiVec * currentKunai.GetComponent<Kunai>().moveSpeed;
                }


                //sets variable to current velocity to avoid snapping
                lerpDashToNormalDirection = rb.velocity.x;

				//can throw kunai again
				currentKunai = null;


				//teleport complete
                isTeleporting = false;
            }
			
			//to avoid weird lerping and snapping
            DOTween.Kill("movement");


            //performs a linear lerp on variable from current value to direction of input, basically lerping rb.velocity.x
            DOTween.To(() => lerpDashToNormalDirection, x => lerpDashToNormalDirection = x, inputX * moveSpeed, recoveryTime).SetAs(tweenParams);

			//sets velocity
            rb.velocity = new Vector2(lerpDashToNormalDirection, rb.velocity.y);

            //rb.velocity = new Vector2(inputX * moveSpeed, rb.velocity.y);
        }
		else if (isClingJumping)
		{
            //stops movement tween
            DOTween.Kill("movement");

            //move character in direction of cling jump
            rb.velocity = clingJumpDirection * (moveSpeed * 1.2f);
            //dashUsedBool = true;

            //sets variable to current velocity to avoid snapping
            lerpDashToNormalDirection = rb.velocity.x;
        }
		else if (isDashing) 
		{
			//stops movement tween
			DOTween.Kill("movement");

			//move character in direction of dash
			rb.velocity = directionVec * (moveSpeed * 1.2f);
			//dashUsedBool = true;

			//sets variable to current velocity to avoid snapping
			lerpDashToNormalDirection = rb.velocity.x;
		}
		
		

		isGrounded = Physics2D.OverlapCircle(groundCheckPoint.position, 0.1f, LayerMask.GetMask("Ground"));


		//hangtime
		if (isGrounded)
		{
			hangCounter = hangTime;

			if (groundedOnce == false)
			{
				groundedOnce= true;
				OnLanding();
			}

			if ((isAlive == false) && (rb.velocity.x != 0))
			{
				rb.velocity = new Vector2(0, 0);
			}
			
		}
		else
		{
			hangCounter -= Time.deltaTime;

			if (groundedOnce == true)
			{
				groundedOnce = false;
			}
		}

		

		if (isClinging == true)
		{
			rb.velocity = new Vector2(0.0f, 0.0f);

			transform.position = clingPosition;
		}

		if (isAlive == false && deathCooldown >= 0)
		{
			deathCooldown -= Time.deltaTime;
		}


        //if (Input.GetButtonDown("Jump"))
        //{
        //	jumpBufferCounter = jumpBufferLength;
        //}
        //else
        //{
        //	jumpBufferCounter -= Time.deltaTime;
        //}




        //Short hops
        //if (Input.GetButtonUp("Jump") && rb.velocity.y > 0)
        //{
        //	rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
        //}

        animator.SetFloat("xVelocity", Mathf.Abs(rb.velocity.x));
        animator.SetFloat("yVelocity", (rb.velocity.y));
		animator.SetBool("isGrounded", isGrounded);
        animator.SetBool("isDashing", isDashing);
        animator.SetBool("isClinging", isClinging);


		if (rb.velocity.x < 0 && currentDirection != Direction.Left)
		{
			SetSpriteDirection(Direction.Left);
		}
		else if (rb.velocity.x > 0 && currentDirection != Direction.Right)
		{
            SetSpriteDirection(Direction.Right);
        }

		

    }

	private void SetSpriteDirection(Direction direction)
	{
		currentDirection = direction;

		if (currentDirection == Direction.Left)
		{
            playerSpriteObject.GetComponent<SpriteRenderer>().flipX = true;
        }
		else
		{
            playerSpriteObject.GetComponent<SpriteRenderer>().flipX = false;
        }
	}

	public void Move(InputAction.CallbackContext context)
	{
		if (isAlive)
		{
            inputX = context.ReadValue<Vector2>().x;
            inputY = context.ReadValue<Vector2>().y;
        }
		
	}

	public void Jump(InputAction.CallbackContext context) 
	{
		
		if (isAlive)
		{
            jumpBufferCounter = jumpBufferLength;

            //Jump code
            if (jumpBufferCounter >= 0 && hangCounter > 0 && isClinging == false && context.started == true)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpHeight);
                jumpBufferCounter = 0;

                StartCoroutine(DelayGroundedOnceCheck());

            }
            else if (jumpBufferCounter >= 0 && isClinging == true && context.started == true)
            {
                isClingJumping = true;
                UnCling();



            }

            //Short hops
            if (context.canceled && rb.velocity.y > 0)
            {
                rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
            }
        }
		else
		{
			if (deathCooldown < 0)
			{
				gameManager.ResetLevel();
			}
		}

        

        
    }

	public void Dash(InputAction.CallbackContext context)
	{

        if (context.started == true && dashCooldownCounter <= 0 && dashUsedBool == false && isAlive)
        {
            directionVec = new Vector2(inputX, inputY);

            directionVec = directionVec.normalized;


			//improve code to dash in direction you are facing
			if (directionVec == Vector2.zero)
			{
                if (currentDirection == Direction.Left)
                {
                    directionVec = new Vector2(-1.0f, 0.0f);
                }
                else
                {
                    directionVec = new Vector2(1.0f, 0.0f);
                }
            }

            float angle = Vector2.SignedAngle(Vector2.up, directionVec) + 90 + (directionVec.x < 0 ? 180 : 0);

            playerSpriteObject.transform.eulerAngles = new Vector3(0.0f, 0.0f, angle);

			isDashing = true;

			//how long the dash lasts
			dashCounter = dashTime;

			//how long before you can dash again
			dashCooldownCounter = dashCooldownTime;

			//used dash
			dashUsedBool = true;

			dashCheckOnce = false;


            if (isClinging== true)
			{
				
				UnCling();
			}
        }

        

		



	}


    public void Aim(InputAction.CallbackContext context)
    {
        aimInputX = context.ReadValue<Vector2>().x;
        aimInputY = context.ReadValue<Vector2>().y;

		aimVector = context.ReadValue<Vector2>();

        aimVector = aimVector.normalized;

		
		//reset aimArrow transform so angle is displayed properly
		//aimArrow.transform.localPosition = defaultAimVector;
		//aimArrow.transform.rotation = Quaternion.identity;

		//calculate difference between desired vector and up vector
        float angle = Vector2.SignedAngle(Vector2.up, aimVector);

		//rotate aimer around the player, by that angle
        //aimArrow.transform.RotateAround(gameObject.transform.position, Vector3.forward, angle);

		RaycastHit2D raycastHit2D = Physics2D.Raycast(gameObject.transform.position, aimVector);

		

    }

    public void Fire(InputAction.CallbackContext context)
    {

		//if button is pushed down
		if (context.started == true && isAlive)
		{

			

			//if a kunai has not already been thrown
            if (kunaiCooldownCounter <= 0)
            {

				if (currentKunai != null)
				{
					currentKunai.GetComponent<Kunai>().CollectKunai();
				}

				if (isAiming)
				{
                    //fetch vector of when it is thrown
                    kunaiVector = aimVector;
                }
				else
				{
					if (currentDirection == Direction.Left)
					{
						kunaiVector = new Vector2(-1.0f, 0.0f);
                    }
                    else
                    {
						kunaiVector = new Vector2(1.0f, 0.0f);
                    }
                    
				}
				

				//calculate angle to rotate kunai by, probably can put this in a shared angle float with Aim function
                float angle = Vector2.SignedAngle(Vector2.up, kunaiVector);

				//spawn vector
				Vector3 kunaiSpawnVector = new Vector3(gameObject.transform.position.x + aimVector.x, gameObject.transform.position.y + aimVector.y, gameObject.transform.position.z);

				//Instantiate kunai at player location, layer mask collision matrix prevents the player and kunai from colliding
                currentKunai = Instantiate(kunaiObject, kunaiSpawnVector, Quaternion.identity);

				//rotate to face direction of travel
                currentKunai.transform.Rotate(0.0f, 0.0f, angle);

				//prevent spamming fire
                kunaiFired = true;

                kunaiCooldownCounter = kunaiCooldown;

				//kunaiList.Add(currentKunai);

				kunaiPopupOnce = false;

                //StartCoroutine(CheckForKunai());


            }
			else if (kunaiFired == true)
			{
				//Teleport();
			}
        }
		


        







    }

    public void Slow(InputAction.CallbackContext context)
    {
		
		

		//isSlowing = false;

  //      Time.timeScale = 1.0f;

        if (slowdownTimeRemaining > 0.5f)
        {
            canSlowdown = true;
        }

        if (canSlowdown == true && (PauseManager.instance.IsGamePaused() == false))
		{
            if (context.interaction is HoldInteraction && (context.started == true || context.performed == true))
            {
                Time.timeScale = 0.5f;
                isSlowing = true;

                if (slowdownOnce == false)
				{
                    slowdownBlackout.FadeInSlow();
					slowdownOnce = true;
                }
				
            }
            else
            {
                Time.timeScale = 1.0f;
                isSlowing = false;

                if (slowdownOnce)
				{
                    slowdownBlackout.FadeOutSlow();
					slowdownOnce = false;
                }
                
            }
        }
		

        


		//if (context.canceled == true)
		//{
		//	slowTime = false;
		//	Time.timeScale = 1.0f;
		//}











    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
		if (isGrounded== false && isAlive)
		{
			if (collision.gameObject.layer == 7)
			{
                Cling();
            }
            
        }

		
		
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        CheckEnemyDash(collision);

        if (collision.transform.tag == "Bullet")
		{
			if (isDashing == true)
			{
				if (collision.gameObject.GetComponent<Bullet>().isReflected == false)
				{
					gameManager.HitStop(0.1f);

                    RumbleManager.instance.RumblePulse(0.4f, 0.4f, 0.15f);

                    CameraController.instance.ShakeCamera(2.0f, 0.1f);

                    collision.gameObject.GetComponent<Bullet>().Reflect();
                }
				
			}
			else
			{
				if (isAlive)
				{
					PlayerDeath();
				}
			}
		}
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
		CheckEnemyDash(collision);
    }

	private void CheckEnemyDash(Collider2D collision)
	{
        if (collision.transform.tag == "Enemy")
        {
            if (isDashing == true)
            {
                if (collision.gameObject.GetComponent<EnemyAI>().IsEnemyAlive())
                {

                    gameManager.HitStop(0.1f);

                    RumbleManager.instance.RumblePulse(0.25f, 0.25f, 0.175f);

                    CameraController.instance.ShakeCamera(1f, 0.1f);

                    collision.gameObject.GetComponent<EnemyAI>().Death();

                    if (kunaiCooldownCounter > 0)
                    {
                        ResetKunaiCooldown();
                    }


                    gameManager.AddScoreToTemp(collision.GetComponent<EnemyAI>(), "dash");




                }

            }

        }
    }

    void Cling()
	{
        isLeftWall = Physics2D.OverlapCircle(leftCheckPoint.position, 0.1f, LayerMask.GetMask("Ground"));

        isRightWall = Physics2D.OverlapCircle(rightCheckPoint.position, 0.1f, LayerMask.GetMask("Ground"));

        isCeiling = Physics2D.OverlapCircle(ceilingCheckPoint.position, 0.1f, LayerMask.GetMask("Ground"));

		if (isLeftWall == true | isRightWall | isCeiling)
		{
			clingPosition = transform.position;

			isClinging = true;
		}

		//check to see if this code should go inside that ^^
		dashCooldownCounter = 0;
		dashUsedBool= false;
		isDashing = false;

		if (isLeftWall)
		{
			playerSpriteObject.GetComponent<SpriteRenderer>().flipX= true;
		}
		else if (isRightWall)
		{
            playerSpriteObject.GetComponent<SpriteRenderer>().flipX = false;
        }
		else if (isCeiling)
		{
            playerSpriteObject.GetComponent<SpriteRenderer>().flipX = false;
            playerSpriteObject.transform.eulerAngles = new Vector3(0.0f, 0.0f, 90f);
        }


    }

	void UnCling()
	{
		if (isClingJumping == true)
		{

            

            if (isLeftWall)
            {
                clingJumpDirection = new Vector2(5.0f, jumpHeight / 1.5f);
                jumpBufferCounter = 0;
            }
            else if (isRightWall)
            {
                clingJumpDirection = new Vector2(-5.0f, jumpHeight / 1.5f);
                jumpBufferCounter = 0;
            }
            else if (isCeiling)
            {
				if (inputX < 0)
				{
                    clingJumpDirection = new Vector2(-5.0f, jumpHeight / 1.5f);
                    jumpBufferCounter = 0;
                }
				else
				{
                    clingJumpDirection = new Vector2(5.0f, jumpHeight / 1.5f);
                    jumpBufferCounter = 0;
                }
            }

			clingJumpDirection.Normalize();

            //how long the cling jump lasts
            clingJumpCounter = clingJumpTime;

			wallJumpParticles.gameObject.transform.rotation =Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, Vector2.SignedAngle(Vector2.up, clingJumpDirection));

            wallJumpParticles.Play();
        }
        

		isClinging = false;

        playerSpriteObject.GetComponent<SpriteRenderer>().flipX = false;

        playerSpriteObject.transform.eulerAngles = new Vector3(0.0f, 0.0f, 0.0f);


    }

	public void Teleport(InputAction.CallbackContext context)
	{

		if (context.started == true && isAlive)
		{
            if (kunaiFired == true && canTeleport == true)
            {
				if (isClinging)
				{
                    UnCling();
                }
                

                ResetStates();

                //StartCoroutine(CheckForKunai());

				ParticleSystem smokeVFX = Instantiate(teleportSmokeVFX, transform.position, Quaternion.identity);
				Destroy(smokeVFX.gameObject, smokeVFX.main.duration);


                //duhh
                isTeleporting = true;


                //teleport
                transform.position = currentKunai.transform.position;



                if (currentKunai.GetComponent<Kunai>().isMoving == false)
                {



                    if (currentKunai.GetComponent<Kunai>().inEnemy == true)
                    {
                        GameObject currentEnemy = currentKunai.GetComponent<Kunai>().GetEnemyInfo();

                        gameManager.HitStop(0.1f);

                        RumbleManager.instance.RumblePulse(0.4f, 0.4f, 0.2f);

                        currentEnemy.GetComponent<EnemyAI>().Death();

                        if (kunaiCooldownCounter > 0)
                        {
                            ResetKunaiCooldown();
                        }

                        gameManager.AddScoreToTemp(currentEnemy.GetComponent<EnemyAI>(), "tp");

                    }
                    else
                    {
                        //check around for walls to cling
                        Cling();
                    }

                }
                //else
                //{

                //    //Collect and Despawn Kunai as we're teleporting to it before making its trigger collider visible, more of a fix than anything
                //    currentKunai.GetComponent<Kunai>().CollectKunai();
                //}

                //RemoveKunai(currentKunai);

                if (currentKunai.GetComponent<Kunai>().inEnemy == false)
				{
                    RumbleManager.instance.RumblePulse(0.15f, 0.15f, 0.1f);
                }
                    

                currentKunai.GetComponent<Kunai>().CollectKunai();

                



                //StartCoroutine(CheckForKunai());

            }
        }
		
		


	}

	

	public void ResetKunaiCooldown()
	{
		kunaiCooldownCounter = -0.1f;

		kunaiPopup.SetActive(true);

		gameManager.UpdateKunaiUI(kunaiCooldownCounter, kunaiCooldown);
	}

	public void SetPlayerSpawn(Vector3 spawnPoint)
	{
		playerSpawn = spawnPoint;
	}

	public void ResetPlayer()
	{

		rb.velocity = new Vector2(0,0);

		if (currentKunai != null)
		{
            currentKunai.GetComponent<Kunai>().CollectKunai();
        }
		
		ResetKunaiCooldown();

		ResetStates();

		Respawn();

		inputX = 0;
		inputY = 0;

        

    }

	void Respawn()
	{
		transform.position = playerSpawn;
		transform.rotation = Quaternion.identity;

		isAlive = true;

        animator.SetBool("isAlive", isAlive);
        //animator.SetTrigger("Player Death");
    }

	void PlayerDeath()
	{
		ResetStates();

        Vector2 deathDirection = RandomDeathDirection(minLaunchDirection, maxLaunchDirection);

        rb.velocity = deathDirection * deathLaunchSpeed;

        isAlive = false;

		

        animator.SetBool("isAlive", isAlive);
		animator.SetTrigger("PlayerDeath");

        gameManager.HitStop(0.2f);

        gameManager.PlayerDeath();

    }

    private Vector2 RandomDeathDirection(Vector2 min, Vector2 max)
    {
        return Vector2.Lerp(min, max, Random.value).normalized;
    }

	


    void ResetStates()
	{

		isDashing= false;

		isClinging = false;

		isLeftWall = false;
		isRightWall = false;
		isCeiling = false;

		isTeleporting = false;
	}

	void SetTutorial()
	{
		canTeleport = false;
	}

	public void EnableTeleport()
	{
		canTeleport = true;
	}
	void OnLanding()
	{

		ParticleSystem landingVFX = landingSmokeVFX;

  //      var mainModule = landingVFX.main;

  //      mainModule.startSpeed = new ParticleSystem.MinMaxCurve(rb.velocity.y / 5);

		//mainModule.startSpeed = Mathf.Abs(mainModule.startSpeed);



        Instantiate(landingVFX, groundCheckPoint.position, Quaternion.Euler(-90f, 0f, 0f));
	}

	//Delays chaning groundedOnce to true for 0.1s so that the smoke VFX doesn't play when the player jumps
	IEnumerator DelayGroundedOnceCheck()
	{
		

        yield return new WaitForSeconds(.1f);

        groundedOnce= false;
    }

	public void EnableMovement()
	{
		playerInput.enabled = true;
	}

	public void DisableMovement()
	{
        playerInput.enabled = false;
    }

  //  IEnumerator CheckForKunai()
  //  {
  //      yield return new WaitForSeconds(.1f);

  //      if (kunaiList.Count > 0)
  //      {
  //          currentKunai = kunaiList[kunaiList.Count - 1];
  //      }

  //      if (kunaiList.Count > 0)
		//{
		//	kunaiTPOnce = true;
		//}


  //  }

	//public void RemoveKunai(GameObject kunai)
	//{
	//	kunaiList.Remove(kunai);
	//}



    //public void ResetKunaiThrowCooldown()
    //{
    //	kunaiThrowCooldownCounter = 0;
    //}
}