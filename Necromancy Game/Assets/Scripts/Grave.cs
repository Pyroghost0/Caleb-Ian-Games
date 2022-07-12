using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grave : MonoBehaviour
{
    public short bones = 50;
    public bool usedUp = false;
    public Transform graveAssistant;

    private void Start()
    {
        GetComponent<SpriteRenderer>().sortingOrder = (int)(transform.position.y * -10);
    }

    public short DigBones(short bonesDug)
    {
        if (!usedUp)
        {
            bones -= bonesDug;
            if (bones <= 0)
            {
                StartCoroutine(DestroyGrave());
                return (short)(bonesDug + bones);
            }
            else
            {
                return bonesDug;
            }
        }
        return 0;
    }

    IEnumerator DestroyGrave()
    {
        usedUp = true;
        float timer = 0f;
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        while (timer < 1f)
        {
            spriteRenderer.color = new Color(1f, 1f, 1f, 1f - timer);//Would rather it disappear into the ground, but fading also works
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
