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
    public string sceneName;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.Log("Loading player...");
            StartCoroutine(WaitLoadPlayer());
		}
		else
		{
            gun = GameObject.FindGameObjectWithTag("Gun").GetComponent<Gun>();
            player.GetComponent<AudioManager>().ChangeScene(sceneName);
            player.GetComponent<PlayerStatus>().currentSpawnPosition = firstCheckpoint;
        }
        StartCoroutine(DelayForLoading());
    }
    private IEnumerator WaitLoadPlayer()
	{
        AsyncOperation ao = SceneManager.LoadSceneAsync(loadedPlayerScene, LoadSceneMode.Additive);
        if (ao == null)
        {
            Debug.LogError("Unable to load player " + loadedPlayerScene);
        }
        yield return new WaitUntil(() => ao.isDone);
        player = GameObject.FindGameObjectWithTag("Player");
        gun = GameObject.FindGameObjectWithTag("Gun").GetComponent<Gun>();
        if (sceneName != "Tutorial")
        {
            player.GetComponent<PlayerStatus>().LoadData(false);
		}
		else
		{
            player.GetComponent<PlayerStatus>().isTutorial = true;
		}
        player.GetComponent<AudioManager>().ChangeScene(sceneName);
        player.GetComponent<PlayerStatus>().currentSpawnPosition = firstCheckpoint;
    }
    IEnumerator DelayForLoading()
    {
        yield return new WaitUntil(() => (player != null));
        player.GetComponent<PlayerMovement>().enabled = false;
        yield return new WaitForFixedUpdate();
        yield return new WaitForSeconds(.5f);
        player.GetComponent<PlayerMovement>().enabled = true;
    }
}
