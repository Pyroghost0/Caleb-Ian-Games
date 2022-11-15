/* Coded by Ian Connors
 * Qualms
 * Changes the audio in the white house scene
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhiteHouseTrigger : MonoBehaviour
{
	private bool triggered = false;
	public AudioSource audioSource;
	private void Start()
	{
	}
	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player") && !triggered)
		{
			triggered = true;
			
			StartCoroutine(proceduralStartUp());
		}
	}
	private IEnumerator proceduralStartUp()
	{
		AudioManager audioManager = GameObject.FindGameObjectWithTag("Player Manager").GetComponent<PlayerManager>().player.GetComponent<AudioManager>();
		
		yield return new WaitForSeconds(3);
		audioManager.BgmChangeVolume(0.5f);
		audioManager.BgmEchoSettings(50, 0.2f);
		audioManager.ToggleBgmEcho(true);

		yield return new WaitForSeconds(3);
		audioSource.enabled = true;
		audioSource.time = audioManager.bgm.time;
		audioSource.volume = 0.6f;
		audioSource.Play();
	}
}
