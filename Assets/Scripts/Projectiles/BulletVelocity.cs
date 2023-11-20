using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BulletVelocity : MonoBehaviour
{
    [SerializeField] float startVelocity = 10f; //m/s

    private void Start() 
    {
        GetComponent<Rigidbody>().velocity = transform.forward * startVelocity;
    }
}
