using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

public class MouseCamera : MonoBehaviour
{
    float h;
    float v;
    float rotX;

    [SerializeField] float sensitivity = 50;

    Transform body;

    [SerializeField] InputActionReference mouseX, mouseY;

    private void OnEnable() 
    {
        mouseX.action.Enable();
        mouseY.action.Enable();
    }

    void Start()
    {
        body = transform.parent;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void CursorStates()
    {
        if (Cursor.lockState != CursorLockMode.Locked)
            Cursor.lockState = CursorLockMode.Locked;
        else
            Cursor.lockState = CursorLockMode.None;
    }

    void Update()
    {
        h = mouseX.action.ReadValue<float>();
        v = mouseY.action.ReadValue<float>();

        body.Rotate(new Vector3(0, h, 0) * sensitivity * Time.deltaTime);
        
        rotX -= v * sensitivity * Time.deltaTime;
        rotX = Mathf.Clamp(rotX, -80f, 80f);

        transform.localRotation = Quaternion.Euler(rotX, 0, 0);
    }

    public void DoFov(float endValue)
    {
        /* uso el paquete y la libreria de dotween para hacer interpolaciones 
        mas faciles y limpias entre puntos */
        GetComponent<Camera>().DOFieldOfView(endValue, 0.25f);
    }

    private void OnDisable() 
    {
        mouseX.action.Disable();
        mouseY.action.Disable();
    }
}
