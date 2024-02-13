using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class AfterImagePool : MonoBehaviour
{
    [SerializeField] private GameObject afterImagePrefab;
    [SerializeField] private int afterImageSpawnAmount = 5;
    private ObjectPool<GameObject> pool;

    // Start is called before the first frame update
    void Start()
    {
        pool = new ObjectPool<GameObject>(() =>
        {
            return Instantiate(afterImagePrefab); ;
        }, afterImage =>
        {
            afterImage.gameObject.SetActive(true);
        }, afterImage =>
        {
            afterImage.gameObject.SetActive(false);
        }, afterImage =>
        {
            Destroy(afterImage.gameObject);
        }, false, afterImageSpawnAmount, 20);

        //Spawn();

    }

    public GameObject Spawn()
    {
        GameObject currentAfterImage = null;

        currentAfterImage = pool.Get();




        return currentAfterImage;

    }

    public void KillAfterImage(GameObject currentAfterImage)
    {
        pool.Release(currentAfterImage);
    }
}
