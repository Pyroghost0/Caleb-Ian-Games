using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public float slime;
    public GameObject centerRay;
    public GameObject[] middleRays;
    public GameObject[] outerRays;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))//Left Click
        {

        }

        if (Input.GetMouseButton(1))//Right Click
        {
            Ray center = new Ray(transform.position, centerRay.transform.position - transform.position);
            RaycastHit centerHit;
            Physics.Raycast(center, out centerHit);
            if (centerHit.collider != null && centerHit.collider.CompareTag("Slime"))
            {
                //Debug.Log("Start: " + center.origin +  ", End: " + centerHit.point + ", Name: " + centerHit.collider.name);
                Debug.DrawRay(center.origin, centerRay.transform.position - transform.position, Color.red);
            }
            for (int i = 0; i < middleRays.Length; i++)
            {
                Ray middle = new Ray(transform.position, middleRays[i].transform.position - transform.position);
                RaycastHit middleHit;
                Physics.Raycast(middle, out middleHit);
                if (middleHit.collider != null && middleHit.collider.CompareTag("Slime"))
                {
                    //Debug.Log("Start: " + center.origin +  ", End: " + centerHit.point + ", Name: " + centerHit.collider.name);
                    Debug.DrawRay(middle.origin, middleRays[i].transform.position - transform.position, Color.red);
                }
            }
            for (int i = 0; i < outerRays.Length; i++)
            {
                Ray outer = new Ray(transform.position, outerRays[i].transform.position - transform.position);
                RaycastHit outerHit;
                Physics.Raycast(outer, out outerHit);
                if (outerHit.collider != null && outerHit.collider.CompareTag("Slime"))
                {
                    //Debug.Log("Start: " + center.origin +  ", End: " + centerHit.point + ", Name: " + centerHit.collider.name);
                    Debug.DrawRay(outer.origin, outerRays[i].transform.position - transform.position, Color.red);
                }
            }
        }
    }
}
