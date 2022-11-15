/* Coded by Caleb Kahn
 * Qualms
 * Checks if in ground / ice
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundChecker : MonoBehaviour
{
    public bool inGround;
    public bool onIce;
    //public LayerMask groundLayer;

    private void OnTriggerStay(Collider collider)
    {
        //Debug.Log(collider.name + ", " + collider.gameObject.layer);
        if (collider.gameObject.layer >= 8 && collider.gameObject.layer != 11)
        {
            inGround = true;
        }
        if (collider.gameObject.layer == 13)
        {
            onIce = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer >=8 && other.gameObject.layer != 11)
        {
            inGround = false;
        }
        if (other.gameObject.layer == 13)
        {
            onIce = false;
        }
    }
}
