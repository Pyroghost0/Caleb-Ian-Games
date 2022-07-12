
using System;
using System.Collections;
using UnityEngine;

public class TreeGenerator : MonoBehaviour
{
    public GameObject tree;
    public Sprite[] treeSprites;
    void Start()
    {
        float x = -8.5f;
        for (int i = 0; i < 210; i++)
		{
            float y = UnityEngine.Random.Range(2.25f, 4.5f);
            GameObject t = Instantiate(tree, gameObject.transform);
            t.transform.position = new Vector3(x, y, 0);
            t.GetComponent<SpriteRenderer>().sortingOrder = (int) (y * -10) + 50;
            t.GetComponent<SpriteRenderer>().color = new Color((y - 1.5f) / -3 + 1f, (y - 1.5f) / -3 + 1f, (y - 1.5f) / -3 + 1f, (y - 1.5f) / -1.5f + 1.7f);
            t.GetComponent<SpriteRenderer>().sprite = treeSprites[UnityEngine.Random.Range(0, treeSprites.Length)];
            int rand = UnityEngine.Random.Range(0, 2);
            if (rand == 0)
			{
                t.GetComponent<SpriteRenderer>().flipX =  false;
			}
			else
			{
                t.GetComponent<SpriteRenderer>().flipX = true;
            }
            x += UnityEngine.Random.Range(0.3f, 0.4f);
        }
    }
}
