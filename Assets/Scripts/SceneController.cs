using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void LoadNextScene()
    {
        StartCoroutine(LoadSceneCoroutine(SceneManager.GetActiveScene().name));
    }

    IEnumerator LoadSceneCoroutine(string level)
    {
        Time.timeScale = 1.0f;

        yield return new WaitForSeconds(2.5f);

        if (level == "Main Menu")
        {
            SceneManager.LoadScene("Tutorial");
        }
        else if (level == "Tutorial")
        {
            SceneManager.LoadScene("Cyberpunk");
        }
        else
        {
            SceneManager.LoadScene("Main Menu");
        }

        
    }

    public void LoadCyberpunk()
    {
        StartCoroutine(LoadSceneCoroutine("Tutorial"));
    }

    public void ReturnToMenu()
    {
        StartCoroutine(LoadSceneCoroutine("Cyberpunk"));
    }


    public void RestartLevel()
    {
        StartCoroutine(RestartSceneCoroutine());
    }

    IEnumerator RestartSceneCoroutine()
    {

        Time.timeScale = 1.0f;

        yield return new WaitForSeconds(2.5f);

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }


}
