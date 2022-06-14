using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncyObject : MonoBehaviour
{
    public float bounce = 30f;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Bounce");
            Rigidbody player = collision.gameObject.GetComponent<Rigidbody>();
            player.velocity = Vector3.up * bounce;
        }
    }
}
