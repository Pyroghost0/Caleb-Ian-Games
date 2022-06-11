using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public float amountOfSlime = 50f;
    public GameObject player;
    public GameObject bulletPrefab;
    public GameObject cameraBasisObject;

    public GameObject centerRay;
    public GameObject[] middleRays;
    public GameObject[] outerRays;
    public float suckDistence = 25f;
    public float suckPower = .6f;
    public GameObject suckSpot;
    public bool isSucking = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))//Left Click
        {
            float power = 0f;
            if (amountOfSlime > 50f)
            {
                power = 10f;
            }
            else
            {//50 = 10 lost, 25 = 5 lost, 5 = 1 lost
                power = amountOfSlime / 5f;
            }
            amountOfSlime -= power;
            power += 4f;
            Quaternion rotation = Quaternion.Euler(cameraBasisObject.transform.rotation.eulerAngles.x - 5f, cameraBasisObject.transform.rotation.eulerAngles.y, cameraBasisObject.transform.rotation.eulerAngles.z);
            Instantiate(bulletPrefab, transform.position, rotation).transform.localScale *= (power / 14f);
        }

        if (Input.GetMouseButton(1))//Right Click
        {
            List<GameObject> suckedObjects = new List<GameObject> { };
            List<float> slimeSuckPower = new List<float> { };
            isSucking = true;


            //Center
            Ray center = new Ray(suckSpot.transform.position, centerRay.transform.position - suckSpot.transform.position);
            RaycastHit centerHit;
            Physics.Raycast(center, out centerHit);
            if (centerHit.collider != null && centerHit.collider.gameObject.layer == 9)
            {
                float distence = (centerHit.collider.gameObject.transform.position - suckSpot.transform.position).magnitude;
                if (distence < suckDistence)
                {
                    suckedObjects.Add(centerHit.collider.gameObject);
                    slimeSuckPower.Add(suckDistence - distence);
                }
            }
            if (centerHit.collider != null)
            {
                Debug.Log("Start: " + center.origin +  ", End: " + centerHit.point + ", Name: " + centerHit.collider.name);
                Debug.DrawRay(center.origin, centerRay.transform.position - suckSpot.transform.position, Color.red);
            }
            //Middle Circle
            for (int i = 0; i < middleRays.Length; i++)
            {
                Ray middle = new Ray(suckSpot.transform.position, middleRays[i].transform.position - suckSpot.transform.position);
                Debug.Log(middle.direction);
                RaycastHit middleHit;
                Physics.Raycast(middle, out middleHit);
                if (middleHit.collider != null && middleHit.collider.gameObject.layer == 9)
                {
                    float distence = (middleHit.collider.gameObject.transform.position - suckSpot.transform.position).magnitude;
                    if (distence < suckDistence)
                    {
                        int slimeNum = -1;
                        for (int j = 0; j < suckedObjects.Count; j++)
                        {
                            if (suckedObjects[j] == middleHit.collider.gameObject)
                            {
                                slimeNum = j;
                                break;
                            }
                        }
                        if (slimeNum == -1)
                        {
                            suckedObjects.Add(middleHit.collider.gameObject);
                            slimeSuckPower.Add(.8f * (suckDistence - distence));
                        }
                        else
                        {
                            slimeSuckPower[slimeNum] += .8f * (suckDistence - distence);
                        }
                    }
                }
                if (middleHit.collider != null)
                {
                    Debug.Log("Start: " + center.origin + ", End: " + middleHit.point + ", Name: " + middleHit.collider.name);
                    Debug.DrawRay(center.origin, middleRays[i].transform.position - suckSpot.transform.position, Color.red);
                }
            }
            //Outer Circle
            for (int i = 0; i < outerRays.Length; i++)
            {
                Ray outer = new Ray(suckSpot.transform.position, outerRays[i].transform.position - suckSpot.transform.position);
                RaycastHit outerHit;
                Physics.Raycast(outer, out outerHit);
                if (outerHit.collider != null && outerHit.collider.gameObject.layer == 9)
                {
                    float distence = (outerHit.collider.gameObject.transform.position - suckSpot.transform.position).magnitude;
                    if (distence < suckDistence)
                    {
                        int slimeNum = -1;
                        for (int j = 0; j < suckedObjects.Count; j++)
                        {
                            if (suckedObjects[j] == outerHit.collider.gameObject)
                            {
                                slimeNum = j;
                                break;
                            }
                        }
                        if (slimeNum == -1)
                        {
                            suckedObjects.Add(outerHit.collider.gameObject);
                            slimeSuckPower.Add(.5f * (suckDistence - distence));
                        }
                        else
                        {
                            slimeSuckPower[slimeNum] += .5f * (suckDistence - distence);
                        }
                    }
                }
                /*if (outerHit.collider != null)
                {
                    Debug.Log("Start: " + center.origin + ", End: " + outerHit.point + ", Name: " + outerHit.collider.name);
                    Debug.DrawRay(center.origin, outerRays[i].transform.position - suckSpot.transform.position, Color.red);
                }*/
            }

            //Suck
            for (int i = 0; i < suckedObjects.Count; i++)
            {
                Vector3 direction = (suckSpot.transform.position - suckedObjects[i].transform.position).normalized;
                suckedObjects[i].GetComponent<Rigidbody>().AddForce(slimeSuckPower[i] * direction * suckPower, ForceMode.Force);
            }
        }

        if (Input.GetMouseButtonUp(1))//Right Click
        {
            isSucking = false;
        }

    }

    public void SuckedSlime()
    {
        if (amountOfSlime < 50f)
        {
            amountOfSlime += 10f;
        }
        else
        {//50 = 10 gained, 75 = 5 gained, 95 = 1 gained
            amountOfSlime += (100f - amountOfSlime) / 5f;
        }
    }
}
