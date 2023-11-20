using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField] float range = 5f;
    [SerializeField] float force = 1000f;
    [SerializeField] float upwardsModifier = 1000f;
    [SerializeField] LayerMask isExpulsable;
    [SerializeField] GameObject visualExplosionPrefab;

    private void Start() 
    {
        Collider[] colls = Physics.OverlapSphere(transform.position, range, isExpulsable);
        if (colls.Length > 0)
        {
            for (int i = 0; i < colls.Length; i++)
            {
                if (colls[i].TryGetComponent<HurtBox>(out HurtBox hurtBox))
                {
                    hurtBox.NotifyHit(this);
                }
                if (colls[i].TryGetComponent<Rigidbody>(out Rigidbody rb))
                {
                    rb.AddExplosionForce(force, transform.position, range, upwardsModifier);
                }
            }
        }
        Instantiate(visualExplosionPrefab, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    private void OnDrawGizmos() 
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
