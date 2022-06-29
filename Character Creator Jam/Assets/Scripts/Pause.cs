using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Pause : MonoBehaviour
{
    public GameObject menu;
    public bool paused = false;
    private Gun gun;

    // Start is called before the first frame update
    void Start()
    {
        gun = GameObject.FindGameObjectWithTag("Gun").GetComponent<Gun>();
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
        if (gun != null)
        {
            gun.enabled = false;
        }
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
        if (gun != null)
        {
            gun.enabled = true;
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
