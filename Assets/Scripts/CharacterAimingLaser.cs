using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAimingLaser : MonoBehaviour
{


    [SerializeField] GameObject player;
    [SerializeField] CharacterMovement characterMovement;

    [SerializeField] Vector2 aimVec;

    [SerializeField] LineRenderer lineRenderer;

    RaycastHit2D aimRaycast;

    public LayerMask ignoreLayers;
    

    public GameObject impactCircle;


    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player");

        characterMovement = player.GetComponent<CharacterMovement>();

        lineRenderer = GetComponent<LineRenderer>();
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        //aimRaycast = characterMovement.aimLaserRaycast;

        if (characterMovement.isAiming == true)
        {
            aimVec = characterMovement.aimVector;

            lineRenderer.enabled = true;

            

            aimRaycast = Physics2D.Raycast(gameObject.transform.position, aimVec, 50f, ~ignoreLayers);
            
            if (aimRaycast.collider != null)
            {
                lineRenderer.SetPosition(0, gameObject.transform.position);
                lineRenderer.SetPosition(1, aimRaycast.point);

                impactCircle.SetActive(true);

                impactCircle.transform.position = aimRaycast.point;



            }
            else
            {
                lineRenderer.enabled=false;
                impactCircle.SetActive(false);
            }


        }
        else
        {
            lineRenderer.enabled = false;
            impactCircle.SetActive(false);
        }
    }
}
