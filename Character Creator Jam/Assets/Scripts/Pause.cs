using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Pause : MonoBehaviour
{
    public GameObject menu;
    public bool paused = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    private void OnApplicationFocus(bool focus)
    {
        if (!paused && GameObject.FindGameObjectWithTag("Player") != null)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.Escape))
        {
            if (paused)
            {
                UnPause();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        paused = true;
        Time.timeScale = 0f;
        menu.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
    }

    public void UnPause()
    {
        paused = false;
        Time.timeScale = 1f;
        menu.SetActive(false);
        if (GameObject.FindGameObjectWithTag("Player") != null)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene("Main Screen");
    }

    public void Respawn()
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStatus>().Respawn();
        UnPause();
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit();
    }
}
