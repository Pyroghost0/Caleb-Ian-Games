using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainScreenManager : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("Character Creator");
        //StartCoroutine(WaitLoad());
    }

    IEnumerator WaitLoad()
    {
        AsyncOperation ao1 = SceneManager.LoadSceneAsync("Character Creator", LoadSceneMode.Additive);
        yield return new WaitUntil(() => ao1.isDone);
        SceneManager.UnloadSceneAsync(SceneManager.GetSceneByName("Main Screen"));
    }

    public void ExitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit();
    }
}
