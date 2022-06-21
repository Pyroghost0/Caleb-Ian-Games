using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoticeTrigger : MonoBehaviour
{
	public int noticeNumber;
	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			other.GetComponent<PlayerStatus>().TriggerNotice(noticeNumber);
			Destroy(gameObject);
		}
	}
}
