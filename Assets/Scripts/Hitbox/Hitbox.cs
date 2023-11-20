using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Hitbox : MonoBehaviour
{
    public UnityEvent onHit;
    public UnityEvent onNoHit;
    public UnityEvent <Collider> onHitWithCollider;

    private void OnTriggerEnter(Collider other) 
    {
        CheckCollider(other);
    }

    private void OnCollisionEnter(Collision other) 
    {
        
    }

    void CheckCollider(Collider other)
    {
        if(other.TryGetComponent<HurtBox>(out HurtBox hurtBox))
        {
            hurtBox.NotifyHit(this);
            onHit.Invoke();
            onHitWithCollider.Invoke(other);
        }
        else
        {
            onNoHit.Invoke();
        }
    }
}
