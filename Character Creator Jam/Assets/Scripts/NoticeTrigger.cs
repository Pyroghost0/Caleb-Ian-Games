using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NoticeTrigger : MonoBehaviour
{
	public string message;
	public float seconds = 5f;
	private GameObject notice;
	public NoticeTrigger mainTrigger;
	public bool activated = false;

    private void OnTriggerEnter(Collider other)
	{
		if (notice == null) notice = GameObject.FindGameObjectWithTag("Notice");
		if (other.CompareTag("Player") && !activated)
		{
			activated = true;
			if (mainTrigger == null)
            {
				StartCoroutine(DeactivateAfterSeconds());
			}
			else if (!mainTrigger.activated)
            {
				mainTrigger.ActivateMainNotice();
            }
		}
	}

	public void ActivateMainNotice()
    {
		mainTrigger.activated = true;
		StartCoroutine(DeactivateAfterSeconds());
    }

	private IEnumerator DeactivateAfterSeconds()
	{
		notice.transform.GetChild(0).gameObject.SetActive(true);
		notice.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = message;
		yield return new WaitForSeconds(seconds);
		notice.transform.GetChild(0).gameObject.SetActive(false);
	}
}
