using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class BulletPool : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private int bulletSpawnAmount = 10;
    private ObjectPool<GameObject> pool;

    // Start is called before the first frame update
    void Start()
    {
        pool = new ObjectPool<GameObject>(() =>
        {
            return Instantiate(bulletPrefab); ;
        }, bullet =>
        {
            bullet.gameObject.SetActive(true);
        }, bullet =>
        {
            bullet.gameObject.SetActive(false);
        }, bullet =>
        {
            Destroy(bullet.gameObject);
        }, false, bulletSpawnAmount, 20);
    }

    public GameObject Spawn()
    {
        GameObject currentBullet = null;

        currentBullet = pool.Get();

        return currentBullet;

    }

    public void KillBullet(GameObject currentBullet)
    {
        pool.Release(currentBullet);
    }
}
