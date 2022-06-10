﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Equipment : MonoBehaviour
{
    public string clothingType;//"Head, "Body," or "Legs"   **Needs capital letter**
    public string clothingStyle;//"Deer", "Mech", "Baker," or "POTUS"   **Needs capital letter(s)**

    private PlayerManager playerManager;
    private PlayerStatus player;
    private Gun gun;
    private GameObject suckSpot;
    private bool sucked = false;

    // Start is called before the first frame update
    void Start()
    {
        playerManager = GameObject.FindGameObjectWithTag("Player Manager").GetComponent<PlayerManager>();//File -> Project Settings -> Script Exicution Order
        player = playerManager.player.GetComponent<PlayerStatus>();
        gun = playerManager.gun;
        suckSpot = gun.suckSpot;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject == suckSpot && gun.isSucking && !sucked)
        {
            sucked = true;
            player.Equip(this);
        }
    }
}