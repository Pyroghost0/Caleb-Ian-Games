using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Cinemachine;

public class SelectManager : MonoBehaviour
{
    public List<Transform> selectableObjects = new List<Transform>();
    public InputManager inputManager;
    public CinemachineVirtualCamera cinemachine;

    public bool selectingObject = false;
    public Transform selectedTroop;
    public float maxVelocity = 8f;
    public float velocityAcceleration = 16f;
    private float velocity = 0f;
    private float minX = 0f;
    private float maxX = 50f;
    public bool currentMinionDigStatus = true;

    public Image[] buttonImages;
    public TextMeshProUGUI[] buttonTexts;
    private string[] unselectButtonsText = {"Minions Mine", "Select", "Minions Attack",       "All Retreat", "All Stay", "All Attack",         "Look Left", "+Troop Size", "Look Right"};
    private string[] selectSkeletonText = {"Select Left", "Deselect", "Select Right",       "Retreat", "Stay", "Attack",         "+Defence", "Die", "+Attack"};
    private string[] selectCorpseText = {"Select Left", "Deselect", "Select Right",       "All Minion", "All Tombstones", "All Skeleton",         "Minion", "Tombstones", "Skeleton"};

    // Start is called before the first frame update
    void Start()
    {
        GameObject[] skeletons = GameObject.FindGameObjectsWithTag("Skeleton");
        for (int i = 0; i < skeletons.Length; i++)
        {
            selectableObjects.Add(skeletons[i].transform);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(inputManager.rightButton) && !selectingObject)
        {
            if (velocity < 0f)
            {
                velocity = Mathf.Clamp(velocity + (velocityAcceleration * 2f * Time.deltaTime), -maxVelocity, maxVelocity);
            }
            else
            {
                velocity = Mathf.Clamp(velocity + (velocityAcceleration * Time.deltaTime), -maxVelocity, maxVelocity);
            }
            transform.position = new Vector3(Mathf.Clamp(transform.position.x + (velocity * Time.deltaTime), minX, maxX), 0f, 0f);
        }
        else if (Input.GetKey(inputManager.leftButton) && !selectingObject)
        {
            if (velocity > 0f)
            {
                velocity = Mathf.Clamp(velocity - (velocityAcceleration * 2f * Time.deltaTime), -maxVelocity, maxVelocity);
            }
            else
            {
                velocity = Mathf.Clamp(velocity - (velocityAcceleration * Time.deltaTime), -maxVelocity, maxVelocity);
            }
            transform.position = new Vector3(Mathf.Clamp(transform.position.x + (velocity * Time.deltaTime), minX, maxX), 0f, 0f);
        }
        else if (velocity > 0f)
        {
            velocity = Mathf.Clamp(velocity - (velocityAcceleration * Time.deltaTime), 0f, maxVelocity);
            transform.position = new Vector3(Mathf.Clamp(transform.position.x + (velocity * Time.deltaTime), minX, maxX), 0f, 0f);
        }
        else if (velocity < 0f)
        {
            velocity = Mathf.Clamp(velocity + (velocityAcceleration * Time.deltaTime), -maxVelocity, 0f);
            transform.position = new Vector3(Mathf.Clamp(transform.position.x + (velocity * Time.deltaTime), minX, maxX), 0f, 0f);
        }
    }

    public void Select(Transform newSelect)
    {
        selectingObject = true;
        selectedTroop = newSelect;
        cinemachine.Follow = newSelect;
        cinemachine.m_Lens.OrthographicSize = 3.5f;
        if (newSelect.CompareTag("Skeleton"))
        {
            for (int i = 0; i < 9; i++)
            {
                buttonTexts[i].text = selectSkeletonText[i];
            }
        }
        else /*if (newSelect.CompareTag("Corpse"))*/
        {
            for (int i = 0; i < 9; i++)
            {
                buttonTexts[i].text = selectCorpseText[i];
            }
        }
    }

    public void Deselect()
    {
        selectingObject = false;
        selectedTroop = null;
        transform.position = new Vector3(cinemachine.transform.position.x, 0f, 0f);
        cinemachine.Follow = transform;
        cinemachine.m_Lens.OrthographicSize = 5f;
        for (int i = 0; i < 9; i++)
        {
            buttonTexts[i].text = unselectButtonsText[i];
        }
    }

    public void SelectLeftTroup()
    {
        if (selectableObjects.Count > 1)
        {
            int leastLeftNum = 0;
            int mostRightNum = 0;
            float distence = selectableObjects[0].position.x - selectedTroop.position.x;
            float leastLeft = distence;
            float mostRight = distence;
            if (selectableObjects[0] == selectedTroop)
            {
                leastLeftNum = 1;
                mostRightNum = 1;
                distence = selectableObjects[1].position.x - selectedTroop.position.x;
                leastLeft = distence;
                mostRight = distence;
            }
            for (int i = 1; i < selectableObjects.Count; i++)
            {
                if (selectableObjects[i] != selectedTroop)
                {
                    distence = selectableObjects[i].position.x - selectedTroop.position.x;
                    if (distence <= 0f)
                    {
                        if (leastLeft > 0f || distence > leastLeft)
                        {
                            leastLeftNum = i;
                            leastLeft = distence;
                        }
                    }
                    else /*if (distence > 0f)*/
                    {
                        if (mostRight <= 0f || distence > mostRight)
                        {
                            mostRightNum = i;
                            mostRight = distence;
                        }
                    }
                }
            }
            if (leastLeft <= 0f)
            {
                Select(selectableObjects[leastLeftNum]);
            }
            else
            {
                Select(selectableObjects[mostRightNum]);
            }
        }
        else
        {
            Debug.Log("Select Fail: Selected Only Troop");
        }
    }

    public void SelectRightTroop()
    {
        if (selectableObjects.Count > 1)
        {
            int leastRightNum = 0;
            int mostLeftNum = 0;
            float distence = selectableObjects[0].position.x - selectedTroop.position.x;
            float leastRight = distence;
            float mostLeft = distence;
            if (selectableObjects[0] == selectedTroop)
            {
                leastRightNum = 1;
                mostLeftNum = 1;
                distence = selectableObjects[1].position.x - selectedTroop.position.x;
                leastRight = distence;
                mostLeft = distence;
            }
                for (int i = 1; i < selectableObjects.Count; i++)
            {
                if (selectableObjects[i] != selectedTroop)
                {
                    distence = selectableObjects[i].position.x - selectedTroop.position.x;
                    if (distence >= 0f)
                    {
                        if (leastRight < 0f || distence < leastRight)
                        {
                            leastRightNum = i;
                            leastRight = distence;
                        }
                    }
                    else /*if (distence < 0f)*/
                    {
                        if (mostLeft >= 0f || distence < mostLeft)
                        {
                            mostLeftNum = i;
                            mostLeft = distence;
                        }
                    }
                }
            }
            if (leastRight >= 0f)
            {
                Select(selectableObjects[leastRightNum]);
            }
            else
            {
                Select(selectableObjects[mostLeftNum]);
            }
            //Debug.Log("Least Right: " + selectableObjects[leastRightNum].name + " is " + leastRight +  "\t\t\tMost Left: " + selectableObjects[mostLeftNum].name + " is " + mostLeft);
        }
        else
        {
            Debug.Log("Select Fail: Selected Only Troop");
        }
    }

    public void SelectNearestTroop()
    {
        if (selectableObjects.Count > 0)
        {
            int num = 0;
            float smallestDistence = Mathf.Abs(transform.position.x - selectableObjects[0].position.x);
            for (int i = 1; i < selectableObjects.Count; i++)
            {
                float magnitude = Mathf.Abs(transform.position.x - selectableObjects[i].position.x);
                if (magnitude < smallestDistence)
                {
                    num = i;
                    smallestDistence = magnitude;
                }
            }
            Select(selectableObjects[num]);
        }
        else
        {
            Debug.Log("Select Fail: No Troops/Corpses");
        }
    }

    public void AllMinionsMine()
    {

    }

    public void BuyMinion()
    {

    }

    public void AllMinionsAttack()
    {

    }

    public void AllCorpsesSpawnMinions()
    {

    }

    public void AllCorpsesSpawnTombstones()
    {

    }

    public void AllCorpsesSpawnSkeletons()
    {

    }

    public void AllTroupsLeft()
    {
        for (int i = 0; i < selectableObjects.Count; i++)
        {
            if (selectableObjects[i].CompareTag("Skeleton"))
            {
                selectableObjects[i].GetComponent<Skeleton>().skeletonMode = SkeletonMode.left;
            }
        }
    }

    public void AllTroupsStay()
    {
        for (int i = 0; i < selectableObjects.Count; i++)
        {
            if (selectableObjects[i].CompareTag("Skeleton"))
            {
                selectableObjects[i].GetComponent<Skeleton>().skeletonMode = SkeletonMode.stay;
            }
        }
    }

    public void AllTroupsRight()
    {
        for (int i = 0; i < selectableObjects.Count; i++)
        {
            if (selectableObjects[i].CompareTag("Skeleton"))
            {
                selectableObjects[i].GetComponent<Skeleton>().skeletonMode = SkeletonMode.right;
            }
        }
    }

    public void TroupLeft()
    {
        selectedTroop.GetComponent<Skeleton>().skeletonMode = SkeletonMode.left;
    }

    public void TroupStay()
    {
        selectedTroop.GetComponent<Skeleton>().skeletonMode = SkeletonMode.stay;
    }

    public void TroupRight()
    {
        selectedTroop.GetComponent<Skeleton>().skeletonMode = SkeletonMode.right;
    }

    public void SelectedTroupDestroyed()
    {
        selectableObjects.Remove(selectedTroop);
        if (selectableObjects.Count > 0)
        {
            int num = 0;
            float smallestDistence = Mathf.Abs(selectedTroop.position.x - selectableObjects[0].position.x);
            for (int i = 1; i < selectableObjects.Count; i++)
            {
                float magnitude = Mathf.Abs(selectedTroop.position.x - selectableObjects[i].position.x);
                if (magnitude < smallestDistence)
                {
                    num = i;
                    smallestDistence = magnitude;
                }
            }
            Select(selectableObjects[num]);
        }
        else
        {
            Deselect();
        }
    }
}
