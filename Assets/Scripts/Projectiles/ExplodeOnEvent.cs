using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodeOnEvent : MonoBehaviour
{
    [SerializeField] GameObject explosionPref;
    Hitbox hitbox;

    private void Awake() 
    {
        hitbox = GetComponent<Hitbox>();
    }

    private void OnEnable() 
    {
        hitbox.onHit.AddListener(Explode);
        hitbox.onNoHit.AddListener(Explode);
    }

    private void OnDisable() 
    {
        hitbox.onHit.RemoveListener(Explode);
        hitbox.onNoHit.RemoveListener(Explode);
    }

    void Explode()
    {
        Instantiate(explosionPref, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
