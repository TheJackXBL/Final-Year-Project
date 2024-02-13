using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetManager : MonoBehaviour
{

    [SerializeField] private Target[] targets;

    [SerializeField] private float numberOfTargetsLeft;

    [SerializeField] private GameObject barrier;

    // Start is called before the first frame update
    void Start()
    {
        CheckForTargets();
    }

    

    public void CheckForTargets()
    {
        targets = null;

        numberOfTargetsLeft = 0;

        targets = gameObject.GetComponentsInChildren<Target>();

        for (int i = 0; i < targets.Length; i++)
        {
            if (targets[i].CheckIfDestroyed() == false)
            {
                numberOfTargetsLeft++;
            }
        }

        if (numberOfTargetsLeft == 0)
        {
            RemoveBarrier();
        }
    }

    void RemoveBarrier()
    {
        barrier.SetActive(false);
    }
}
