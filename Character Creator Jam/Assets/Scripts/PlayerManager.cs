﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour
{
    public GameObject player;
    public Gun gun;
    public string loadedPlayerScene = "Player";
    public GameObject firstCheckpoint;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.Log("Loading player...");
            AsyncOperation ao = SceneManager.LoadSceneAsync(loadedPlayerScene, LoadSceneMode.Additive);
            if (ao == null)
            {
                Debug.LogError("Unable to load player " + loadedPlayerScene);
            }
            player = GameObject.FindGameObjectWithTag("Player");
        }
        gun = GameObject.FindGameObjectWithTag("Gun").GetComponent<Gun>();
        player.GetComponent<PlayerStatus>().currentSpawnPosition = firstCheckpoint;

    }

    IEnumerator DelayForLoading()
    {
        player.GetComponent<PlayerMovement>().enabled = false;
        yield return new WaitForFixedUpdate();
        yield return new WaitForSeconds(.5f);
        player.GetComponent<PlayerMovement>().enabled = true;
    }
}
