using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Parallax : MonoBehaviour
{

    private float lengthX, lengthY;
    private float startPosX, startPosY;
    public GameObject cam;
    public float parallaxEffectX, parallaxEffectY;

    private Vector3 camStartPos;

    // Start is called before the first frame update
    void Start()
    {
        startPosX = transform.position.x;
        startPosY = transform.position.y;

        if (GetComponent<SpriteRenderer>() != null )
        {
            lengthX = GetComponent<SpriteRenderer>().bounds.size.x;
            lengthY = GetComponent<SpriteRenderer>().bounds.size.y;
        }
        else
        {
            lengthX = GetComponent<Tilemap>().cellBounds.size.x;
            lengthY = GetComponent<Tilemap>().cellBounds.size.y;
        }

        

        camStartPos = cam.transform.position;
    }

    
    void FixedUpdate()
    {
        //float temp = (cam.transform.position.x * (1 - parallaxEffect));

        float distanceX = ((cam.transform.position.x * parallaxEffectX) - camStartPos.x);

        float distanceY = ((cam.transform.position.y * parallaxEffectY) - camStartPos.y);

        transform.position = new Vector3(startPosX + distanceX, startPosY + distanceY, transform.position.z);


        //if (temp > startPosX + lengthX)
        //{
        //    startPosX += lengthX;
        //}
        //else if (temp < startPosX - lengthX)
        //{
        //    startPosX -= lengthX;
        //}
    }
}
