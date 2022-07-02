using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NoticeTrigger : MonoBehaviour
{
	public string message;
	public float seconds = 5f;
	private GameObject notice;
	private bool activated = false;

    private void Start()
    {
		notice = GameObject.FindGameObjectWithTag("Notice");
    }

    private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player") && !activated)
		{
			activated = true;
			notice.transform.GetChild(0).gameObject.SetActive(true);
			notice.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = message;
			StartCoroutine(DeactivateAfterSeconds());
		}
	}

	private IEnumerator DeactivateAfterSeconds()
	{
		yield return new WaitForSeconds(seconds);
		notice.transform.GetChild(0).gameObject.SetActive(false);
	}
}
