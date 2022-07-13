using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBase : MonoBehaviour
{
    public short bones = 0;
    public short health = 1000;
    public short maxHealth = 1000;
    public short maxSkeletons = 3;
    public short maxSkeletonUpgradeAmount = 100;
    public short numSkeletons = 0;

    public void UpgradeMaxSkeletons()
    {
        bones -= maxSkeletonUpgradeAmount;
        maxSkeletons++;
        maxSkeletonUpgradeAmount += 100;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
