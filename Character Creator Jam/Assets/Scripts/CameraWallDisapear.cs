﻿/* Coded by Caleb Kahn
 * Qualms
 * Makes the wall disappear when camera hits a wall connecting the camera to the player
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class CameraWallDisapear : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Wall") || other.CompareTag("Ground"))
        {
            other.gameObject.GetComponent<MeshRenderer>().shadowCastingMode = ShadowCastingMode.ShadowsOnly;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Wall") || other.CompareTag("Ground"))
        {
            other.gameObject.GetComponent<MeshRenderer>().shadowCastingMode = ShadowCastingMode.On;
        }
    }
}
