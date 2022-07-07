using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Equipment : MonoBehaviour
{
    public string clothingType;//"Head", "Body", or "Legs"   **Needs capital letter**
    public string clothingStyle;//"Deer", "Mech", "Baker", or "POTUS"   **Needs capital letter(s)**

    private PlayerManager playerManager;
    private PlayerStatus player;
    private Gun gun;
    private GameObject suckSpot;
    private bool sucked = false;

    // Start is called before the first frame update
    void Start()
    {
        FindPlayer();
    }
	private void FindPlayer()
	{
        playerManager = GameObject.FindGameObjectWithTag("Player Manager").GetComponent<PlayerManager>();//File -> Project Settings -> Script Exicution Order
        if (playerManager.player != null && playerManager.gun != null)
		{
            player = playerManager.player.GetComponent<PlayerStatus>();
            gun = playerManager.gun;
            suckSpot = gun.suckSpot;
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (player == null) FindPlayer();
        if (other.gameObject == suckSpot && gun.isSucking && !sucked && gameObject.layer != 2)
        {
            sucked = true;
            player.Equip(this);
        }
    }
}
