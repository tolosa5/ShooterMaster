using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Barrel : MonoBehaviour
{
    [Header("Debug")]
    [SerializeField] bool debugShoot;

    private void OnValidate() 
    {
        if (debugShoot)
        {
            debugShoot = false;
            Shoot();
        }
    }

    public abstract void Shoot();
}
