using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Pause : MonoBehaviour
{
    public GameObject menu;
    public bool paused = false;
    public GameObject startMenu;
    public GameObject optionsMenu;
    private Gun gun;
    private GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        if (GameObject.FindGameObjectWithTag("Gun") != null)
        {
            gun = GameObject.FindGameObjectWithTag("Gun").GetComponent<Gun>();
        }
        if (GameObject.FindGameObjectWithTag("Player") != null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
    }

    private void OnApplicationFocus(bool focus)
    {
        if (!paused && player != null && (GameObject.FindGameObjectWithTag("Boss") == null || player.GetComponent<PlayerMovement>().enabled))
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
        if (optionsMenu != null)
        {
            ExitOptions();
        }
        if (player != null)
        {
            player.GetComponent<PlayerMovement>().walk.enabled = false;
        }
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
        if (player != null)
        {
            player.GetComponent<PlayerMovement>().walk.enabled = true;
        }
        Time.timeScale = 1f;
        menu.SetActive(false);
        if (player != null && (GameObject.FindGameObjectWithTag("Boss") == null || player.GetComponent<PlayerMovement>().enabled))
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        if (gun != null)
        {
            if (GameObject.FindGameObjectWithTag("Boss") == null || GameObject.FindGameObjectWithTag("Boss").GetComponent<BossBehavior>().enabled)
            {
                gun.enabled = true;
            }
        }
    }
    
    public void Respawn()
    {
        if (GameObject.FindGameObjectWithTag("Boss") == null || GameObject.FindGameObjectWithTag("Boss").GetComponent<BossBehavior>().enabled)
        {
            if (player != null)
            {
                player.GetComponent<PlayerStatus>().Respawn();
            }
        }
        UnPause();
    }

    public void EnterOptions()
    {
        startMenu.SetActive(false);
        optionsMenu.SetActive(true);
    }

    public void ExitOptions()
    {
        startMenu.SetActive(true);
        optionsMenu.SetActive(false);
    }
    public void BackToMainMenu()
    {
        if (player != null)
        {
            player.GetComponent<PlayerStatus>().UnlockLevel("Save Equipment Info");
        }
        Time.timeScale = 1f;
        SceneManager.LoadScene("Main Screen");
    }

    

    public void QuitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit();
    }
}
