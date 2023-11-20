using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Tracer : MonoBehaviour
{
    [SerializeField] float lifeTime = 2;
    private LineRenderer line;

    private void Awake() 
    {
        line = GetComponent<LineRenderer>();
    }

    private void Start() 
    {
        DOTween.To(() => line.widthMultiplier, (x) => line.widthMultiplier = x, 0, lifeTime);
        Destroy(gameObject, lifeTime);
    }

    public void Init(Vector3 startPosition, Vector3 endPosition)
    {
        Vector3[] positions = {startPosition, endPosition};
        line.SetPositions(positions);
    }
}
