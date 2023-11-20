using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroy : MonoBehaviour
{
    public static DontDestroy dontDestroy;
    private void Awake() 
    {
        if (dontDestroy != null)
        {
            Destroy(gameObject);
        }
        else
        {
            dontDestroy = this;
        }
        DontDestroyOnLoad(gameObject);
    }
}
