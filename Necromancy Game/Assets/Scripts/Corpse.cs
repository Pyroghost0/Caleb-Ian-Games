using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Corpse : MonoBehaviour
{
    private SelectManager selectManager;
    private PlayerBase playerBase;
    public GameObject skeletonPrefab;
    public GameObject minionPrefab;
    public short numTombstones = 3;

    // Start is called before the first frame update
    void Start()
    {
        selectManager = GameObject.FindGameObjectWithTag("Select Manager").GetComponent<SelectManager>();
        playerBase = GameObject.FindGameObjectWithTag("Player Base").GetComponent<PlayerBase>();
        selectManager.selectableObjects.Add(transform);
        GetComponent<SpriteRenderer>().sortingOrder = (int)(transform.position.y * -10);
    }

    public void SpawnMinion()
    {
        if (playerBase.numSkeletons < playerBase.maxSkeletons)
        {
            for (int i = 0; i < selectManager.selectableObjects.Count; i++)
            {
                if (selectManager.selectableObjects[i] == transform)
                {
                    if (selectManager.selectableObjects[i] == selectManager.selectingObject)
                    {
                        selectManager.SelectedTroupDestroyed();
                    }
                    else
                    {
                        selectManager.selectableObjects.RemoveAt(i);
                    }
                    break;
                }
            }
            Instantiate(minionPrefab, transform.position, transform.rotation);
            Destroy(gameObject);
        }
        else
        {
            selectManager.corpseActionFail = true;
        }
    }

    public void SpawnTombstones()
    {
        GraveManager graveManager = GameObject.FindGameObjectWithTag("Grave Manager").GetComponent<GraveManager>();
        if (graveManager.graveAssistants.Count > 0)
        {
            for (int i = 0; i < selectManager.selectableObjects.Count; i++)
            {
                if (selectManager.selectableObjects[i] == transform)
                {
                    if (selectManager.selectableObjects[i] == selectManager.selectingObject)
                    {
                        selectManager.SelectedTroupDestroyed();
                    }
                    else
                    {
                        selectManager.selectableObjects.RemoveAt(i);
                    }
                    break;
                }
            }
            graveManager.SpawnGraves(numTombstones);
            Destroy(gameObject);
        }
        else
        {
            selectManager.corpseActionFail = true;
        }
    }

    /*public void AddBones()
    {
        if (selectManager.selectedTroop == transform)
        {
            selectManager.SelectedTroupDestroyed();
        }
        else
        {
            for (int i = 0; i < selectManager.selectableObjects.Count; i++)
            {
                if (selectManager.selectableObjects[i] == transform)
                {
                    selectManager.selectableObjects.RemoveAt(i);
                    break;
                }
            }
        }
        GameObject.FindGameObjectWithTag("Player Base").GetComponent<PlayerBase>().bones += bones;
        Destroy(gameObject);
    }*/

    public void SpawnSkeleton()
    {
        if (playerBase.numSkeletons < playerBase.maxSkeletons)
        {
            for (int i = 0; i < selectManager.selectableObjects.Count; i++)
            {
                if (selectManager.selectableObjects[i] == transform)
                {
                    selectManager.selectableObjects.RemoveAt(i);
                    break;
                }
            }
            Transform addedSkeleton = Instantiate(skeletonPrefab, transform.position, transform.rotation).transform;
            selectManager.selectableObjects.Add(addedSkeleton);
            selectManager.Select(addedSkeleton);
            Destroy(gameObject);
        }
        else
        {
            selectManager.corpseActionFail = true;
        }
    }
    
}
