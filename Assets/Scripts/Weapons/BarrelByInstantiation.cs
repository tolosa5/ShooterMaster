using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrelByInstantiation : Barrel
{
    [SerializeField] GameObject projectilePref;
    [SerializeField] Transform shootPoint;

    public override void Shoot()
    {
        Instantiate(projectilePref, shootPoint.position, Camera.main.transform.rotation);
    }
}
