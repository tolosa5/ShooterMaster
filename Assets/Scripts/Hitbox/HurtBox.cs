using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HurtBox : MonoBehaviour
{
    [SerializeField] UnityEvent onHitNotified;
    [SerializeField] UnityEvent <Transform> onHitNotifiedWithOffender;
    [SerializeField] UnityEvent <Hitbox> onHitNotifiedWithBox;
    [SerializeField] UnityEvent <Barrel> onHitNotifiedWithBarrel;
    [SerializeField] UnityEvent <Explosion> onHitNotifiedWithExplosion;
    

    public virtual void NotifyHit(Hitbox hitbox)
    {
        onHitNotified.Invoke();
        onHitNotifiedWithOffender.Invoke(hitbox.transform);
        onHitNotifiedWithBox.Invoke(hitbox);
    }

    public virtual void NotifyHit(Barrel barrel)
    {
        onHitNotified.Invoke();
        onHitNotifiedWithOffender.Invoke(barrel.transform);
        onHitNotifiedWithBarrel.Invoke(barrel);
    }

    public virtual void NotifyHit(Explosion explosion)
    {
        onHitNotified.Invoke();
        onHitNotifiedWithOffender.Invoke(explosion.transform);
        onHitNotifiedWithExplosion.Invoke(explosion);
    }
}
