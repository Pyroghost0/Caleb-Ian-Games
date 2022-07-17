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

    public float maxVelocity = 8f;
    public float velocityAcceleration = 16f;
    private float velocity = 0f;
    public float minX = 0f;
    public float maxX = 36f;

    public bool selectingObject = false;
    public Transform selectedTroop;
    public bool currentMinionDigStatus = true;
    public SkeletonMode currentSkeletonMode = SkeletonMode.stay;
    public PlayerBase playerBase;
    public GameObject impossibleActionPrefab;
    public bool corpseActionFail = false;

    public RectTransform rectHealthBar;
    public float rectHealth;
    public TextMeshProUGUI healthValue;
    public TextMeshProUGUI selectedObjectName;
    public TextMeshProUGUI boneValue;
    public GameObject boneCostObject0;
    public TextMeshProUGUI boneCostValue0;
    public RectTransform troopCapacity;
    public TextMeshProUGUI troopCapacityText;
    public GameObject boneCostObject1;
    public TextMeshProUGUI boneCostValue1;
    public GameObject boneCostObject2;
    public TextMeshProUGUI boneCostValue2;
    public TextMeshProUGUI[] buttonTexts;
    private string[] unselectButtonsText = {"Minions Mine", "Select", "Minions Attack",       "All Retreat", "All Stay", "All Attack",         "Look Left", "+Troop Size", "Look Right"};
    private string[] selectSkeletonText = {"Select Left", "Deselect", "Select Right",       "Retreat", "Stay", "Attack",         "+Defence", "Die", "+Attack"};
    private string[] selectCorpseText = {"Select Left", "Deselect", "Select Right",       "All Minion", "All Tombstones", "All Skeleton",         "Minion", "Tombstones", "Skeleton"};

    // Start is called before the first frame update
    void Start()
    {
        rectHealth = rectHealthBar.rect.width;
        GameObject[] skeletons = GameObject.FindGameObjectsWithTag("Skeleton");
        for (int i = 0; i < skeletons.Length; i++)
        {
            selectableObjects.Add(skeletons[i].transform);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(inputManager.rightButton) && !selectingObject && !inputManager.holdInputWait)
        {
            if (!inputManager.holdRight)
            {
                inputManager.holdRight = true;
                inputManager.MovingAroundInputChange(inputManager.buttonImages[8], inputManager.rightButton);
            }
            if (Input.GetKey(inputManager.leftButton))
            {
                if (!inputManager.holdLeft)
                {
                    inputManager.holdLeft = true;
                    inputManager.MovingAroundInputChange(inputManager.buttonImages[6], inputManager.leftButton);
                }
                if (velocity > 0f)
                {
                    velocity = Mathf.Clamp(velocity - (velocityAcceleration * Time.deltaTime), 0f, maxVelocity);
                    transform.position = new Vector3(Mathf.Clamp(transform.position.x + (velocity * Time.deltaTime), minX, maxX), 0f, 0f);
                }
                else /*if (velocity < 0f)*/
                {
                    velocity = Mathf.Clamp(velocity + (velocityAcceleration * Time.deltaTime), -maxVelocity, 0f);
                    transform.position = new Vector3(Mathf.Clamp(transform.position.x + (velocity * Time.deltaTime), minX, maxX), 0f, 0f);
                }
            }
            else
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
        }
        else if (Input.GetKey(inputManager.leftButton) && !selectingObject && !inputManager.holdInputWait)
        {
            if (!inputManager.holdLeft)
            {
                inputManager.holdLeft = true;
                inputManager.MovingAroundInputChange(inputManager.buttonImages[6], inputManager.leftButton);
            }
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
        if (selectingObject)
        {
            if (selectedTroop.CompareTag("Skeleton"))
            {
                selectedTroop.GetComponent<Skeleton>().selectBars.SetActive(false);
            }
            else /*if (selectedTroop.CompareTag("Corpse"))*/
            {
                selectedTroop.GetComponent<Corpse>().selectBars.SetActive(false);
            }
        }
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
            Skeleton skeleton = newSelect.GetComponent<Skeleton>();
            selectedObjectName.text = skeleton.skeletonName;
            rectHealthBar.gameObject.SetActive(true);
            rectHealthBar.sizeDelta = new Vector2(((float)skeleton.health / skeleton.maxHealth) * rectHealth, rectHealthBar.rect.height);
            healthValue.text = skeleton.health + "\n" + skeleton.maxHealth;
            boneCostObject0.SetActive(false);
            if (skeleton.defenceBoneUpgradeAmount == -1)
            {
                boneCostObject1.SetActive(false);
            }
            else
            {
                boneCostObject1.SetActive(true);
                boneCostValue1.text = "-" + skeleton.defenceBoneUpgradeAmount.ToString();
            }
            if (skeleton.attackBoneUpgradeAmount == -1)
            {
                boneCostObject2.SetActive(false);
            }
            else
            {
                boneCostObject2.SetActive(true);
                boneCostValue2.text = "-" + skeleton.attackBoneUpgradeAmount.ToString();
            }
            troopCapacity.anchoredPosition = new Vector3(265f, -25, 0f);
            selectedTroop.GetComponent<Skeleton>().selectBars.SetActive(true);
        }
        else /*if (newSelect.CompareTag("Corpse"))*/
        {
            for (int i = 0; i < 9; i++)
            {
                buttonTexts[i].text = selectCorpseText[i];
            }
            selectedObjectName.text = newSelect.GetComponent<Corpse>().corpseName;
            rectHealthBar.gameObject.SetActive(false);
            healthValue.text = "0\n0";
            boneCostObject0.SetActive(false);
            boneCostObject1.SetActive(false);
            boneCostObject2.SetActive(false);
            troopCapacity.anchoredPosition = new Vector3(265f, -25, 0f);
            selectedTroop.GetComponent<Corpse>().selectBars.SetActive(true);
        }
    }

    public void Deselect()
    {
        if (selectingObject)
        {
            if (selectedTroop.CompareTag("Skeleton"))
            {
                selectedTroop.GetComponent<Skeleton>().selectBars.SetActive(false);
            }
            else /*if (selectedTroop.CompareTag("Corpse"))*/
            {
                selectedTroop.GetComponent<Corpse>().selectBars.SetActive(false);
            }
        }
        selectingObject = false;
        selectedTroop = null;
        transform.position = new Vector3(cinemachine.transform.position.x, 0f, 0f);
        cinemachine.Follow = transform;
        cinemachine.m_Lens.OrthographicSize = 5f;
        for (int i = 0; i < 9; i++)
        {
            buttonTexts[i].text = unselectButtonsText[i];
        }
        selectedObjectName.text = "Player Base";
        rectHealthBar.gameObject.SetActive(true);
        rectHealthBar.sizeDelta = new Vector2(((float)playerBase.health / playerBase.maxHealth) * rectHealth, rectHealthBar.rect.height);
        healthValue.text = playerBase.health + "\n" + playerBase.maxHealth;
        boneCostObject0.SetActive(true);
        boneCostValue0.text = "-" + playerBase.maxSkeletonUpgradeAmount.ToString();
        boneCostObject1.SetActive(false);
        boneCostObject2.SetActive(false);
        troopCapacity.anchoredPosition = new Vector3(340f, -25, 0f);
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
            InvalidNotice notice = Instantiate(impossibleActionPrefab).GetComponent<InvalidNotice>();
            notice.text.text = "Select Fail: Selecting Only Object";
            notice.textPosition.anchoredPosition = new Vector2(-340f, 150f);
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
            InvalidNotice notice = Instantiate(impossibleActionPrefab).GetComponent<InvalidNotice>();
            notice.text.text = "Select Fail: Selecting Only Object";
            notice.textPosition.anchoredPosition = new Vector2(-190f, 150f);
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
            InvalidNotice notice = Instantiate(impossibleActionPrefab).GetComponent<InvalidNotice>();
            notice.text.text = "Select Fail: Nothing To Select";
            notice.textPosition.anchoredPosition = new Vector2(-2655f, 150f);
        }
    }

    public void AllMinionsMine()
    {
        currentMinionDigStatus = true;
        GameObject[] minions = GameObject.FindGameObjectsWithTag("Minion");
        for (int i = 0; i < minions.Length; i++)
        {
            minions[i].GetComponent<Minion>().inDiggingMode = true;
        }
    }

    public void AllMinionsAttack()
    {
        currentMinionDigStatus = false;
        GameObject[] minions = GameObject.FindGameObjectsWithTag("Minion");
        for (int i = 0; i < minions.Length; i++)
        {
            minions[i].GetComponent<Minion>().inDiggingMode = false;
        }
    }

    public void AllCorpsesSpawnMinions()
    {
        GameObject[] corpses = GameObject.FindGameObjectsWithTag("Corpse");
        for (int i = 0; i < corpses.Length; i++)
        {
            for (int j = i+1; j < corpses.Length; j++)
            {
                if (corpses[i].transform.position.x > corpses[j].transform.position.x)
                {
                    GameObject temp = corpses[i];
                    corpses[i] = corpses[j];
                    corpses[j] = temp;
                }
            }
        }
        for (int i = 0; i < corpses.Length && playerBase.numSkeletons + i < playerBase.maxSkeletons; i++)
        {
            corpses[i].GetComponent<Corpse>().SpawnMinion();
            if (i + 1 < corpses.Length && playerBase.numSkeletons + i + 1 == playerBase.maxSkeletons)
            {
                corpseActionFail = true;
                break;
            }
        }
        if (corpseActionFail)
        {
            InvalidNotice notice = Instantiate(impossibleActionPrefab).GetComponent<InvalidNotice>();
            notice.text.text = "Max Troops Reached";
            notice.textPosition.anchoredPosition = new Vector2(-75f, 150f);
            corpseActionFail = false;
        }
    }

    public void AllCorpsesSpawnTombstones()
    {
        GameObject[] corpses = GameObject.FindGameObjectsWithTag("Corpse");
        for (int i = 0; i < corpses.Length; i++)
        {
            corpses[i].GetComponent<Corpse>().SpawnTombstones();
        }
        if (corpseActionFail)
        {
            InvalidNotice notice = Instantiate(impossibleActionPrefab).GetComponent<InvalidNotice>();
            notice.text.text = "Graveyard Is Full";
            notice.textPosition.anchoredPosition = new Vector2(0f, 150f);
            corpseActionFail = false;
        }
    }

    public void AllCorpsesSpawnSkeletons()
    {
        if (playerBase.numSkeletons + 1 == playerBase.maxSkeletons)
        {
            selectedTroop.GetComponent<Corpse>().SpawnSkeleton();
        }
        else if (playerBase.numSkeletons == playerBase.maxSkeletons)
        {
            InvalidNotice notice = Instantiate(impossibleActionPrefab).GetComponent<InvalidNotice>();
            notice.text.text = "Max Troops Reached";
            notice.textPosition.anchoredPosition = new Vector2(75f, 150f);
            corpseActionFail = false;
        }
        else
        {
            GameObject[] corpses = GameObject.FindGameObjectsWithTag("Corpse");
            if (playerBase.numSkeletons + corpses.Length == playerBase.maxSkeletons)
            {
                Transform previouslySelectedTroop = selectedTroop;
                short numSpawned = 0;
                for (int i = 0; i < corpses.Length && playerBase.numSkeletons + numSpawned + 1 < playerBase.maxSkeletons; i++)
                {
                    if (previouslySelectedTroop != corpses[i].transform)
                    {
                        numSpawned++;
                        corpses[i].GetComponent<Corpse>().SpawnSkeleton();
                    }
                }
                previouslySelectedTroop.GetComponent<Corpse>().SpawnSkeleton();
            }
            else
            {
                Transform previouslySelectedTroop = selectedTroop;
                for (int i = 0; i < corpses.Length; i++)
                {
                    for (int j = i + 1; j < corpses.Length; j++)
                    {
                        if (corpses[i].transform.position.x < corpses[j].transform.position.x)
                        {
                            GameObject temp = corpses[i];
                            corpses[i] = corpses[j];
                            corpses[j] = temp;
                        }
                    }
                }
                short numSpawned = 0;
                for (int i = 0; i < corpses.Length && playerBase.numSkeletons + numSpawned + 1 < playerBase.maxSkeletons; i++)
                {
                    if (previouslySelectedTroop != corpses[i].transform)
                    {
                        numSpawned++;
                        corpses[i].GetComponent<Corpse>().SpawnSkeleton();
                    }
                    if (((numSpawned == i && i + 2 < corpses.Length) || (numSpawned != i && i + 1 < corpses.Length)) && playerBase.numSkeletons + numSpawned + 1 == playerBase.maxSkeletons)
                    {
                        corpseActionFail = true;
                        break;
                    }
                }
                previouslySelectedTroop.GetComponent<Corpse>().SpawnSkeleton();
                if (corpseActionFail)
                {
                    InvalidNotice notice = Instantiate(impossibleActionPrefab).GetComponent<InvalidNotice>();
                    notice.text.text = "Max Troops Reached";
                    notice.textPosition.anchoredPosition = new Vector2(75f, 150f);
                    corpseActionFail = false;
                }
            }
        }
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
                Skeleton skeleton = selectableObjects[i].GetComponent<Skeleton>();
                skeleton.skeletonMode = SkeletonMode.stay;
                skeleton.stayGoal = selectableObjects[i].position;
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

    public void TroupStay()
    {
        Skeleton skeleton = selectedTroop.GetComponent<Skeleton>();
        skeleton.skeletonMode = SkeletonMode.stay;
        skeleton.stayGoal = selectedTroop.position;
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
