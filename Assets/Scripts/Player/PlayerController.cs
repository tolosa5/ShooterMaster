using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public enum States
    {
        Freeze,
        Grappling,
        Swinging,
        Walking,
        Sprinting,
        Air,
    };

    [Header("StateMachine")]
    [HideInInspector] public States currentState;

    [SerializeField] float walkSpeed = 5;
    [SerializeField] float sprintSpeed = 7;
    [SerializeField] float swingSpeed = 8;
    

    [Header("References")]
    private CharacterController chC;
    private Animator aN;
    private Rigidbody rb;

    
    [Header("Movement")]
    private float speed;
    [HideInInspector] public Vector3 dirMov;
    private Vector3 velocityToApply;
    public bool enableMovementOnNextTouch;
    private Vector3 velocityToSet;


    [Header("Camera")]
    public GameObject cam;
    public float grappleFov = 95f;


    [Header("Jump")]
    [SerializeField] float Gravity = -9.8f;
    [SerializeField] float jumpSpeed = 2f;
    private float verticalVelocity; 

    private bool isGrounded;
    [SerializeField] Transform feet;
    [SerializeField] float checkerLenght;
    [SerializeField] LayerMask isGround;

    [Header("Weapon")]
    BarrelByRaycast barrel;


    [Header("Inputs")]
    //para evitar el componente playerActions, viene mejor hacer esto
    [SerializeField] InputActionReference playerMove;
    [SerializeField] InputActionReference shoot;
    public InputActionReference playerJump;

    void Start()
    {
        chC = GetComponent<CharacterController>(); 
        rb = GetComponent<Rigidbody>();
        currentState = States.Walking;
        barrel = GetComponentInChildren<BarrelByRaycast>();
    }

    private void OnEnable() 
    {
        playerMove.action.Enable();
        playerJump.action.Enable();
        shoot.action.Enable();
    }

    void Update()
    {
        velocityToApply = Vector3.zero;
        
        StateHandler();
        Shoot();
        Debug.Log(currentState);
    }

    private void StateHandler()
    {
        switch (currentState)
        {
            default:
            case States.Walking:
            speed = walkSpeed;
            Movement();
            JumpHandler();
            chC.Move(velocityToApply * Time.deltaTime);

            break;

            case States.Freeze:
            speed = 0;

            break;

            case States.Grappling:
            speed = sprintSpeed;
            GoDinamic();

            break;

            case States.Swinging:
            speed = swingSpeed;
            GoDinamic();

            break;

            case States.Air:

            break;
        }
    }

    void Movement()
    {
        if (currentState == States.Grappling) return;
        if (currentState == States.Swinging) return;

        Vector2 rawMove = playerMove.action.ReadValue<Vector2>();
        dirMov = (Vector3.right * rawMove.x) + (Vector3.forward * rawMove.y);
        dirMov.Normalize();

        Transform cameraTransform = Camera.main.transform;
        Vector3 cameraMoveDir = cameraTransform.TransformDirection(dirMov);

        float originalMagnitude = cameraMoveDir.magnitude;
        cameraMoveDir = Vector3.ProjectOnPlane(cameraMoveDir, Vector3.up).normalized * originalMagnitude;

        Vector3 velocity = cameraMoveDir * speed;
        //chC.Move(velocity * Time.deltaTime);
        velocityToApply = velocity;
    }

    void JumpHandler()
    {
        verticalVelocity += Gravity * 1.5f * Time.deltaTime;

        if (chC.isGrounded || currentState == States.Grappling || currentState == States.Swinging)
        verticalVelocity = 0f;

        verticalVelocity += Gravity * Time.deltaTime;
        
        bool mustJump = playerJump.action.WasPerformedThisFrame();
        if (mustJump && chC.isGrounded)
        {
            verticalVelocity = jumpSpeed;
        }

        velocityToApply += verticalVelocity * Vector3.up;
    }

    public bool IsGrounded()
    {
        if (Physics.Raycast(feet.position, Vector3.down, checkerLenght, isGround))
        {
            isGrounded = true;
        }
        return isGrounded;
    }

    public void JumpToPosition(Vector3 targetPosition, float trajectoryHeight)
    {
        currentState = States.Grappling;

        velocityToSet = CalculateJumpVelocity(transform.position, targetPosition, trajectoryHeight);
        Invoke(nameof(SetVelocity), 0.1f);

        //Invoke(nameof(ResetRestrictions), 3f);
    }

    private void SetVelocity()
    {
        enableMovementOnNextTouch = true;
        rb.velocity = velocityToSet;
        cam.GetComponent<MouseCamera>().DoFov(grappleFov);
    }

    public void Reset()
    {
        currentState = States.Walking;
        GoKinematic();
        cam.GetComponent<MouseCamera>().DoFov(85f);
        Destroy(GetComponent<Swinging>().joint);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (enableMovementOnNextTouch)
        {
            enableMovementOnNextTouch = false;
            Reset();
        }
    }

    public Vector3 CalculateJumpVelocity(Vector3 startPoint, Vector3 endPoint, float trajectoryHeight)
    {
        /* la trayectoria del gancho, algo complicada y la pille de por ahi aunque entiendo la base fisica que usa
        pero nunca se me habria ocurrido */
        float gravity = Physics.gravity.y;
        float displacementY = endPoint.y - startPoint.y;
        Vector3 displacementXZ = new Vector3(endPoint.x - startPoint.x, 0f, endPoint.z - startPoint.z);

        Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * trajectoryHeight);
        Vector3 velocityXZ = displacementXZ / (Mathf.Sqrt(-2 * trajectoryHeight / gravity) 
            + Mathf.Sqrt(2 * (displacementY - trajectoryHeight) / gravity));

        return velocityXZ + velocityY;
    }
    
    #region MovementTypes
    public void GoDinamic()
    {
        //para cuando esta con gancho o cosas que necesita fisicas
        if (rb.isKinematic && chC.enabled)
        {
            rb.isKinematic = false;
            chC.enabled = false;
            Debug.Log("dinamic");
        }
    }

    public void GoKinematic()
    {
        //para cuando esta andando
        if (!rb.isKinematic && !chC.enabled)
        {
            rb.isKinematic = true;
            chC.enabled = true;
            Debug.Log("kinematic");
        }
    }
    #endregion

    public void Shoot()
    {
        if (shoot.action.WasPerformedThisFrame() && Cursor.lockState != CursorLockMode.None)
        {
            barrel.Shoot();
        }
    }

    private void OnDisable() 
    {
        playerMove.action.Disable();
        playerJump.action.Disable();
        shoot.action.Disable();
    }
}
