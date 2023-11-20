using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Swinging : MonoBehaviour
{
    [Header("References")]
    [SerializeField] LineRenderer lr;
    public Transform gunTip, cam, player;
    public LayerMask isGrappleable;
    private PlayerController playerController;


    [Header("Swinging")]
    private float maxSwingDistance = 25f;
    private Vector3 currentGrapplePosition;
    private Vector3 swingPoint;
    [HideInInspector] public SpringJoint joint;

    [Header("Hook")]
    public float grappleDelayTime;
    public float overshootYAxis;
    private Vector3 grapplePoint;

    private bool grappling;

    public float grapplingCd;
    private float grapplingCdTimer;


    [Header("AerialMovement")]
    public Transform orientation;
    private Rigidbody rb;
    public float horizontalThrustForce;
    public float forwardThrustForce;
    public float extendCableSpeed;


    [Header("Prediction")]
    public RaycastHit predictionHit;
    public float predictionSphereCastRadius;
    public Transform predictionPoint;


    [Header("Input")]
    [SerializeField] InputActionReference playerGrapple;
    [SerializeField] InputActionReference playerHook;
    [SerializeField] InputActionReference[] aerialMovementDirections;

    private void OnEnable() 
    {
        playerGrapple.action.Enable();
        playerHook.action.Enable();
        for (int i = 0; i < aerialMovementDirections.Length; i++)
        {
            aerialMovementDirections[i].action.Enable();
        }
    }

    private void Start() 
    {
        playerController = GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    private void Update()
    {
        CheckForSwingPoints();
        Inputs();

        if (grapplingCdTimer > 0)
            grapplingCdTimer -= Time.deltaTime;

        lr.enabled = true;

        
    }

    private void FixedUpdate() 
    {
        if (joint != null) AerialMovement();
    }

    private void Inputs()
    {
        if (playerGrapple.action.WasPressedThisFrame())
        {
            StartSwing();
        }
        if (playerGrapple.action.WasReleasedThisFrame())
        {
            StopSwing();
        }

        if (playerHook.action.WasPerformedThisFrame())
        {
            StartGrapple();
            Debug.Log("input");
        }
    }

    #region Rope visual
    private void LateUpdate() 
    {
        DrawRope();
    }

    private void DrawRope()
    {
        // no dibujar la linea si no esta hookeando
        if (grappling)
        {
            lr.SetPosition(0, gunTip.position);
        }

        if (!joint) return;
        Debug.Log("dibujando");

        currentGrapplePosition = Vector3.Lerp(currentGrapplePosition, swingPoint, Time.deltaTime * 8f);

        lr.SetPosition(1, swingPoint);
        lr.SetPosition(0, gunTip.position);
    }
    #endregion

    #region Prediction
    private void CheckForSwingPoints()
    {
        if (joint != null) return;

        RaycastHit sphereCastHit;
        Physics.SphereCast(cam.position, predictionSphereCastRadius, cam.forward, 
                        out sphereCastHit, maxSwingDistance, isGrappleable);

        RaycastHit raycastHit;
        Physics.Raycast(cam.position, cam.forward, 
                        out raycastHit, maxSwingDistance, isGrappleable);

        Vector3 realHitPoint;

        //directo
        if (raycastHit.point != Vector3.zero)
            realHitPoint = raycastHit.point;

        //indirecto
        else if (sphereCastHit.point != Vector3.zero)
            realHitPoint = sphereCastHit.point;

        //fallar
        else
        realHitPoint = Vector3.zero;

        if (realHitPoint != Vector3.zero)
        {
            predictionPoint.gameObject.SetActive(true);
            predictionPoint.position = realHitPoint;
        }

        else
        predictionPoint.gameObject.SetActive(false);

        predictionHit = raycastHit.point == Vector3.zero ? sphereCastHit : raycastHit;
    }
    #endregion

    #region Swing
    private void StartSwing()
    {
        // sale si no hay prediccion
        if (predictionHit.point == Vector3.zero) return;

        //desactiva el grapple
        if(TryGetComponent<Hook>(out Hook hook))
        hook.StopGrapple();

        playerController.currentState = PlayerController.States.Swinging;
        playerController.GoDinamic();

        swingPoint = predictionHit.point;

        //se hace el tema de la liana con un springJoint
        joint = player.gameObject.AddComponent<SpringJoint>();
        joint.autoConfigureConnectedAnchor = false;
        joint.connectedAnchor = swingPoint;

        float distanceFromPoint = Vector3.Distance(player.position, swingPoint);

        joint.maxDistance = distanceFromPoint * 0.7f;
        joint.minDistance = distanceFromPoint * 0.3f;

        joint.spring = 8f;
        joint.damper = 12f;
        joint.massScale = 5f;

        lr.positionCount = 2;
        currentGrapplePosition = gunTip.position;
        playerController.enableMovementOnNextTouch = true;
    }

    public void StopSwing()
    {
        playerController.currentState = PlayerController.States.Walking;
        lr.positionCount = 0;

        Destroy(joint);
    }

    private void AerialMovement()
    {
        //derecha
        if (aerialMovementDirections[0].action.WasPerformedThisFrame()) 
        rb.AddForce(transform.right * horizontalThrustForce);

        // izquierda
        if (aerialMovementDirections[1].action.WasPerformedThisFrame()) 
        rb.AddForce(transform.right * -horizontalThrustForce);

        // adelante
        if (aerialMovementDirections[2].action.WasPerformedThisFrame())
        rb.AddForce(transform.forward * horizontalThrustForce);

        // acortar liana
        if (playerController.playerJump.action.WasPerformedThisFrame())
        {
            Vector3 directionToPoint = swingPoint - transform.position;
            rb.AddForce(directionToPoint.normalized * forwardThrustForce);

            float distanceFromPoint = Vector3.Distance(transform.position, swingPoint);

            joint.maxDistance = distanceFromPoint * 0.8f;
            joint.minDistance = distanceFromPoint * 0.25f;
        }

        // alargar liana
        if (aerialMovementDirections[3].action.WasPerformedThisFrame())
        {
            float extendedDistanceFromPoint = Vector3.Distance(transform.position, swingPoint) + extendCableSpeed;

            joint.maxDistance = extendedDistanceFromPoint * 0.8f;
            joint.minDistance = extendedDistanceFromPoint * 0.25f;
        }
    }
    #endregion

    #region Hook
    private void StartGrapple()
    {
        if (predictionHit.point == Vector3.zero) return;

        if (grapplingCdTimer > 0) return;
        Debug.Log("hook");

        cam.GetComponent<MouseCamera>().DoFov(85f);

        // desactiva la liana
        GetComponent<Swinging>().StopSwing();
        playerController.GoDinamic();
        Debug.Log("hook2");

        grappling = true;
        Debug.Log("hook3");

        playerController.currentState = PlayerController.States.Grappling;
        Debug.Log("hook4");

        grapplePoint = predictionHit.point;
        Invoke(nameof(ExecuteGrapple), grappleDelayTime);

        lr.enabled = true;
        if (grapplePoint != null)
        {
            lr.SetPosition(1, grapplePoint);
            
        }
    }

    private void ExecuteGrapple()
    {
        Vector3 lowestPoint = new Vector3(transform.position.x, transform.position.y - 1f, transform.position.z);

        float grapplePointRelativeYPos = grapplePoint.y - lowestPoint.y;
        float highestPointOnArc = grapplePointRelativeYPos + overshootYAxis;

        if (grapplePointRelativeYPos < 0) highestPointOnArc = overshootYAxis;

        playerController.JumpToPosition(grapplePoint, highestPointOnArc);

        Invoke(nameof(StopGrapple), 1f);
    }

    public void StopGrapple()
    {
        playerController.currentState = PlayerController.States.Walking;

        grappling = false;

        cam.GetComponent<MouseCamera>().DoFov(85f);

        grapplingCdTimer = grapplingCd;

        lr.enabled = false;
    }
    #endregion

    private void OnDisable() 
    {
        playerGrapple.action.Disable();
        playerHook.action.Disable();
        for (int i = 0; i < aerialMovementDirections.Length; i++)
        {
            aerialMovementDirections[i].action.Disable();
        }
    }
}
