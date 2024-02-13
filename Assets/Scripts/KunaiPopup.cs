using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KunaiPopup : MonoBehaviour
{

    [SerializeField] Animator popupAnim;
    // Start is called before the first frame update
    void Start()
    {
        //popupAnim = gameObject.GetComponent<Animator>();
    }

    private void OnEnable()
    {
        popupAnim.Play("Kunai Pickup Display");

        StartCoroutine(HidePopup());
    }

    private void OnDisable()
    {
        
    }


    IEnumerator HidePopup()
    {
        yield return new WaitForSeconds(1.5f);

        gameObject.SetActive(false);
    }
}
