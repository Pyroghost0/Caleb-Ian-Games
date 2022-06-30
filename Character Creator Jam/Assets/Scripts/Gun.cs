using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public float amountOfSlime = 50f;
    public GameObject player;
    public GameObject bulletPrefab;
    public GameObject cameraBasisObject;
    public Animator reticleAnimation;

    public GameObject centerRay;
    public GameObject[] middleRays;
    public GameObject[] outerRays;
    public GameObject slimeBar;
    public RectTransform rectSlimeBar;
    public float rectSlime;
    public float suckDistence = 25f;
    public float suckPower = .6f;
    public GameObject suckSpot;
    public GameObject suckParticles;
    public bool isSucking = false;
    public float powerMultiplier = 1f;

    public Animator playerAnim;

    [SerializeField] private List<int> slimeColors;
    [SerializeField] private Material[] materials;
    private int playerSkinColor;
    private int playerHairColor;

    private GameObject bullet;
    private bool shootCooldown = true;
    public float maxSuckTime = 3f;
    public float suckCooldownMultiplier = 2f;
    private float suckTimer = 0f;
    public RectTransform recSuckBar;
    private float recWidth;
    private bool canSuck = true;
    public GameObject cameraCenter;
    //public bool canMove;

    // Start is called before the first frame update
    void Start()
    {
        recWidth = recSuckBar.rect.width;
        rectSlimeBar = slimeBar.GetComponent<RectTransform>();
        rectSlime = rectSlimeBar.rect.width;
        rectSlimeBar.sizeDelta = new Vector2((amountOfSlime / 100) * rectSlime, rectSlimeBar.rect.height);

        playerSkinColor = player.GetComponent<PlayerStatus>().skinColor;
        playerHairColor = player.GetComponent<PlayerStatus>().hairColor;

        slimeColors = new List<int>();
        materials = new Material[12];
        int offset = 0;
        for (int i = 0; i < materials.Length / 2; i++)
        {
            if (i == playerHairColor) offset = 1;
            materials[i] = (Material)Resources.Load("Hair " + (i + offset));
        }
        offset = 0;
        for (int i = 0; i < materials.Length / 2; i++)
        {
            if (i == playerSkinColor) offset = 1;
            materials[i + materials.Length / 2] = (Material)Resources.Load("Skin " + (i + offset));
        }
        //canMove = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && shootCooldown)//Left Click
        {
            StartCoroutine(ShootCooldown());
            reticleAnimation.SetTrigger("Shoot");
            playerAnim.SetTrigger("Shoot");
            if (amountOfSlime > 0.5f)
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
                rectSlimeBar.sizeDelta = new Vector2((amountOfSlime / 100) * rectSlime, rectSlimeBar.rect.height);
                power += 4f;

                Quaternion rotation = Quaternion.Euler(transform.rotation.eulerAngles.x-7f, transform.rotation.eulerAngles.y, cameraBasisObject.transform.rotation.eulerAngles.z);
                Ray gunRay = new Ray(suckSpot.transform.position, rotation * Vector3.forward);
                RaycastHit gunRayHit;
                Physics.Raycast(gunRay, out gunRayHit);
                Ray centerCameraRay = new Ray(cameraCenter.transform.position, cameraCenter.transform.forward);
                RaycastHit centerCameraHit;
                Physics.Raycast(centerCameraRay, out centerCameraHit);

                /*Debug.Log("Gun: " + gunRayHit.collider.name);
                Debug.Log("Camera: " + centerCameraHit.collider.name);
                Debug.DrawRay(suckSpot.transform.position, rotation.normalized * Vector3.forward * 20f, Color.green, 4f);
                Debug.DrawRay(cameraCenter.transform.position, cameraCenter.transform.forward.normalized*20f, Color.red, 4f);*/

                if (gunRayHit.collider == null || gunRayHit.distance > 8.5f)//8.5 is roughly where the rays collide
                {
                    if (centerCameraHit.collider != null && centerCameraHit.distance > 8.5f)
                    {
                        rotation = Quaternion.LookRotation(centerCameraHit.point - suckSpot.transform.position);
                    }
                    else
                    {
                        rotation = Quaternion.Euler(transform.rotation.eulerAngles.x - 7f, transform.rotation.eulerAngles.y-3f, cameraBasisObject.transform.rotation.eulerAngles.z);
                    }
                }
                //Debug.DrawRay(suckSpot.transform.position, rotation.normalized * Vector3.forward * 20f, Color.blue, 4f);

                bullet = Instantiate(bulletPrefab, suckSpot.transform.position, rotation);
                bullet.transform.localScale *= (powerMultiplier * power / 14f);
                if (slimeColors.Count == 0)
                {
                    slimeColors.Add(0);
                }
                bullet.transform.GetChild(0).GetComponent<Renderer>().material = materials[slimeColors[0]];
                slimeColors.RemoveAt(0);
            }
        }

        if (Input.GetMouseButton(1) && canSuck)//Right Click
        {
            suckParticles.SetActive(true);
            reticleAnimation.SetBool("Suck", true);
            playerAnim.SetBool("Sucking", true);
            isSucking = true;

            List<GameObject> suckedObjects = new List<GameObject> { };
            List<float> slimeSuckPower = new List<float> { };
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
            /*if (centerHit.collider != null)
            {
                Debug.Log("Start: " + center.origin +  ", End: " + centerHit.point + ", Name: " + centerHit.collider.name);
                Debug.DrawRay(center.origin, centerRay.transform.position - suckSpot.transform.position, Color.red);
            }*/
            //Middle Circle
            for (int i = 0; i < middleRays.Length; i++)
            {
                Ray middle = new Ray(suckSpot.transform.position, middleRays[i].transform.position - suckSpot.transform.position);
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
                /*if (middleHit.collider != null)
                {
                    Debug.Log("Start: " + center.origin + ", End: " + middleHit.point + ", Name: " + middleHit.collider.name);
                    Debug.DrawRay(center.origin, middleRays[i].transform.position - suckSpot.transform.position, Color.red);
                }*/
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
            suckParticles.SetActive(false);
            reticleAnimation.SetBool("Suck", false);
            playerAnim.SetBool("Sucking", false);
            isSucking = false;
        }

        if (isSucking)
        {
            recSuckBar.gameObject.SetActive(true);
            suckTimer += Time.deltaTime;
            if (suckTimer >= maxSuckTime)
            {
                canSuck = false;
                suckTimer = maxSuckTime;
                suckParticles.SetActive(false);
                reticleAnimation.SetBool("Suck", false);
                playerAnim.SetBool("Sucking", false);
                isSucking = false;
            }
            recSuckBar.sizeDelta = new Vector2((suckTimer / maxSuckTime) * recWidth, recSuckBar.rect.height);
        }
        else if (suckTimer > 0f)
        {
            suckTimer -= Time.deltaTime * suckCooldownMultiplier;
            if (suckTimer <= 0f)
            {
                canSuck = true;
                recSuckBar.gameObject.SetActive(true);
            }
            else
            {
                recSuckBar.sizeDelta = new Vector2((suckTimer / maxSuckTime) * recWidth, recSuckBar.rect.height);
            }
        }
    }

    IEnumerator ShootCooldown()
    {
        shootCooldown = false;
        yield return new WaitForSeconds(.2f);
        shootCooldown = true;
    }


    public void SuckedSlime(int slimeColor)
    {
        if (amountOfSlime < 50f)
        {
            amountOfSlime += 10f;
        }
        else
        {//50 = 10 gained, 75 = 5 gained, 95 = 1 gained
            amountOfSlime += (100f - amountOfSlime) / 5f;
        }
        if (slimeColors.Count < 45)
		{
            for (int i = 0; i < 5; i++)
                slimeColors.Add(slimeColor);
		}
        rectSlimeBar.sizeDelta = new Vector2((amountOfSlime / 100) * rectSlime, rectSlimeBar.rect.height);
    }
}
