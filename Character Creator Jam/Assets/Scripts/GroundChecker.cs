using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundChecker : MonoBehaviour
{
    public bool inGround;
    //public LayerMask groundLayer;

    private void OnTriggerStay(Collider collider)
    {
        //Debug.Log(collider.name + ", " + collider.gameObject.layer);
        if (collider.gameObject.layer >= 8)
        {
            inGround = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer >=8)
        {
            inGround = false;
        }
    }
}
