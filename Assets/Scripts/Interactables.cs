using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class Interactables : MonoBehaviour
{
    [SerializeField] UnityEvent interactAction;
    [SerializeField] UnityEvent eraseInteraction;
    [SerializeField] bool needInput;

    [Header("Key")]
    [SerializeField] InputActionReference interactKey;

    [Header("Checking")]
    [SerializeField] bool isInRange;
    [SerializeField] bool wasInRange;

    private void OnEnable() 
    {
        interactKey.action.Enable();
    }

    private void Update()
    {
        if (isInRange && needInput)
        {
            if (interactKey.action.WasPerformedThisFrame())
            {
                interactAction.Invoke();
            }
        }
        else if (!needInput && isInRange)
        {
            interactAction.Invoke();
        }
        else if (wasInRange)
        {
            eraseInteraction.Invoke();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            isInRange = true;
            Debug.Log("Player now in range");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isInRange = false;
            wasInRange = true;
            Debug.Log("Player now not in range");
        }
    }

    private void OnDisable() 
    {
        interactKey.action.Disable();
    }
}
