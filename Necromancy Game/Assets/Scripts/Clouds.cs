using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clouds : MonoBehaviour
{
    public float speed;
	//public int cloudDistance;
    public Queue<GameObject> clouds = new(6);
    private int numberOfClouds = 6;
	private bool cont = false;
	private void Start()
	{
		for (int i = 0; i < numberOfClouds; i++)
		{
			clouds.Enqueue(transform.GetChild(i).gameObject);
		}
	}
	// Update is called once per frame
	void Update()
    {
		//distance between clouds = 12.25
        transform.Translate(Vector2.left * speed);
        if (Mathf.FloorToInt(transform.position.x) % 12 == 0)
		{
			if (!cont)
			{
				GameObject nextCloud = clouds.Dequeue();
				nextCloud.transform.Translate(Vector2.right * 72f);
				clouds.Enqueue(nextCloud);
				cont = true;
			}

		}
		else
		{
			cont = false;
		}
    }
}
