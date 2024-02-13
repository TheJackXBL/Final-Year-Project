using DG.Tweening.Core.Easing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private GameObject bulletPoolGO;
    private BulletPool bulletPool;

    public TrailRenderer trailRenderer;

    public bool isReflected = false;

    [SerializeField] ParticleSystem reflectParticle;

    private GameCommunicationManager gameManager;

    

    private void Start()
    {
        bulletPoolGO = GameObject.Find("BulletPooler"); 
        bulletPool = bulletPoolGO.GetComponent<BulletPool>();

        trailRenderer = gameObject.GetComponent<TrailRenderer>();

        gameManager = GameObject.Find("GameManager").GetComponent<GameCommunicationManager>();

    }

    private void OnEnable()
    {
        isReflected = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag != "Enemy" && collision.tag != "MainCamera" && collision.tag != "Player" && collision.tag != "DashTriggerCollider")
        {
            bulletPool.KillBullet(gameObject);
            //Destroy(gameObject);
        }
        else if (collision.tag == "Enemy" && isReflected == true)
        {

            if (collision.gameObject.GetComponent<EnemyAI>().IsEnemyAlive())
            {
                bulletPool.KillBullet(gameObject);

                CameraController.instance.ShakeCamera(0.75f, 0.1f);

                RumbleManager.instance.RumblePulse(0.1f, 0.1f, 0.1f);

                collision.gameObject.GetComponent<EnemyAI>().Death();

                gameManager.AddScoreToTemp(collision.gameObject.GetComponent<EnemyAI>(), "reflect");
            }
            
            
            
        }
        
    }

    public void Reflect()
    {
        Vector2 currentDirection = GetComponent<Rigidbody2D>().velocity;

        currentDirection = currentDirection * -1;

        GetComponent<Rigidbody2D>().velocity = currentDirection * 1.25f;

        isReflected = true;

        ParticleSystem reflectPart = Instantiate(reflectParticle, transform.position, Quaternion.identity);

        Destroy(reflectPart.gameObject, reflectParticle.main.duration);

    }
}
