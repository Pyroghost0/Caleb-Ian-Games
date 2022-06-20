using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainScreenManager : MonoBehaviour
{
    public void StartGame()
    {
        //SceneManager.LoadScene("Tutorial");
        StartCoroutine(WaitLoad());
    }

    IEnumerator WaitLoad()
    {
        AsyncOperation ao1 = SceneManager.LoadSceneAsync("Player", LoadSceneMode.Additive);
        AsyncOperation ao2 = SceneManager.LoadSceneAsync("Deer Level", LoadSceneMode.Additive);
        yield return new WaitUntil(() => ao1.isDone && ao2.isDone);
        SceneManager.UnloadSceneAsync(SceneManager.GetSceneByName("Main Screen"));
    }

    public void ExitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit();
    }
}
