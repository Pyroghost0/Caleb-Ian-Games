using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Corpse : MonoBehaviour
{
    private SelectManager selectManager;
    public GameObject skeletonPrefab;
    public short bones = 10;

    // Start is called before the first frame update
    void Start()
    {
        selectManager = GameObject.FindGameObjectWithTag("Select Manager").GetComponent<SelectManager>();
        selectManager.selectableObjects.Add(transform);
        GetComponent<SpriteRenderer>().sortingOrder = (int)(transform.position.y * -10);
    }

    public void SpawnMinion()
    {

    }

    public void SpawnTombStones()
    {

    }

    public void AddBones()
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
    }

    public void SpawnSkeleton()
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
    
}
