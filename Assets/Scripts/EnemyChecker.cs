using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyChecker : MonoBehaviour
{

    public EnemyAI[] allEnemies;
    [SerializeField] private List<GameObject> aliveEnemies;

    [SerializeField] private GameObject doorObject;
    [SerializeField] private GameObject arrowObject;

    private void Start()
    {
        doorObject = transform.GetChild(0).gameObject;
        arrowObject= transform.GetChild(1).gameObject;
    }


    public float CheckForEnemies()
    {
        aliveEnemies.Clear();

        aliveEnemies = new List<GameObject>();

        allEnemies = transform.GetComponentsInChildren<EnemyAI>();

        foreach(EnemyAI enemy in allEnemies)
        {
            if (enemy.IsEnemyAlive() == true)
            {
                aliveEnemies.Add(enemy.gameObject);
            }
        }
        return aliveEnemies.Count;
    }

    public void ResetEnemies()
    {
        allEnemies = transform.GetComponentsInChildren<EnemyAI>();

        foreach(EnemyAI enemy in allEnemies)
        {
            enemy.ResetEnemy();
        }
    }

    public void OpenDoor()
    {
        doorObject.SetActive(false);

        arrowObject.SetActive(true);

        //arrowObject.GetComponent<Animator>().StartPlayback();

    }

    public void HidePreviousLevel()
    {
        arrowObject.SetActive(false);
    }
}
