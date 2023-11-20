using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrelByRaycast : Barrel
{
    [SerializeField] Transform shootPoint;
    [SerializeField] LayerMask hiteable;
    [SerializeField] float maxRange = 100f;
    [SerializeField] GameObject tracerPrefab;

    public override void Shoot()
    {
        Vector3 finalShotPosition = transform.position + transform.forward * maxRange;

        RaycastHit hit;
        if (Physics.Raycast(shootPoint.position, transform.forward, out hit, maxRange, hiteable))
        {
            Debug.Log("shoot");
            if (hit.collider.TryGetComponent<HurtBox>(out HurtBox hurtbox))
            {
                hurtbox.NotifyHit(this);
                Debug.Log("hit");
            }
        }

        GameObject tracerGO = Instantiate(tracerPrefab);
        Tracer tracer = tracerGO.GetComponent<Tracer>();
        tracer.Init(shootPoint.position, finalShotPosition);
    }

    private void OnDrawGizmos() 
    {
        Debug.DrawRay(shootPoint.position, transform.forward);
    }
}
