using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grave : MonoBehaviour
{
    public short bones = 50;
    public bool usedUp = false;
    public Transform graveAssistant;
    public GameObject targetSelect;

    private void Start()
    {
        GetComponent<SpriteRenderer>().sortingOrder = (int)(transform.position.y * -10);
        transform.GetChild(0).GetComponent<SpriteRenderer>().sortingOrder = (int)(transform.position.y * -10) + 1;
    }

    public short DigBones(short bonesDug)
    {
        if (!usedUp)
        {
            bones -= bonesDug;
            if (bones <= 0)
            {
                StartCoroutine(DestroyGraveCoroutine());
                return (short)(bonesDug + bones);
            }
            else
            {
                return bonesDug;
            }
        }
        return 0;
    }

    public void DestroyGrave()
    {
        StartCoroutine(DestroyGraveCoroutine());
    }

    IEnumerator DestroyGraveCoroutine()
    {
        usedUp = true;
        targetSelect.SetActive(false);
        float timer = 0f;
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        SpriteRenderer spriteRenderer2 = transform.GetChild(0).GetComponent<SpriteRenderer>();
        while (timer < 2f)
        {
            spriteRenderer.color = new Color(1f - (timer / 2f), 1f - (timer / 2f), 1f - (timer / 2f), 1f - (timer / 2f));//Would rather it disappear into the ground, but fading also works
            spriteRenderer2.color = new Color(1f - (timer / 2f), 1f - (timer / 2f), 1f - (timer / 2f), 1f - (timer / 2f));
            yield return new WaitForFixedUpdate();
            timer += Time.deltaTime;
        }
        if (graveAssistant != null)
        {
            GameObject.FindGameObjectWithTag("Grave Manager").GetComponent<GraveManager>().graveAssistants.Add(graveAssistant);
        }
        Destroy(gameObject);
    }
}
