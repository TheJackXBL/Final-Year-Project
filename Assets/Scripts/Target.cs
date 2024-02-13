using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{

    private SpriteRenderer sprite;

    [SerializeField] float alphaReduce;
    [SerializeField] float timeBetweenAlphaChange;
    [SerializeField] float fallAmount;

    [SerializeField] private bool isDestroyed = false;

    [SerializeField] TargetManager targetManager;

    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Sprite destroyedSprite;

    // Start is called before the first frame update
    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        isDestroyed = false;

        targetManager = transform.parent.GetComponent<TargetManager>();

        spriteRenderer = GetComponent<SpriteRenderer>();
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Kunai")
        {
            if (!isDestroyed)
            {
                BreakTarget();
                collision.gameObject.GetComponent<Kunai>().CollectKunai();
            }
        }
    }

    void BreakTarget()
    {
        isDestroyed = true;

        spriteRenderer.sprite = destroyedSprite;

        StartCoroutine(Remove());

        targetManager.CheckForTargets();
    }

    IEnumerator Remove()
    {
        Color spriteColorTemp = sprite.color;

        while (spriteColorTemp.a > 0)
        {
            spriteColorTemp.a = spriteColorTemp.a - alphaReduce;
            sprite.color = spriteColorTemp;
            transform.position = new Vector3(transform.position.x, transform.position.y - fallAmount, transform.position.z);
            yield return new WaitForSeconds(timeBetweenAlphaChange);
        }
        

        Destroy(gameObject);
    }

    public bool CheckIfDestroyed()
    {
        return isDestroyed;
    }
}
