using Lolopupka;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float jumpThrust = 5f;
    [SerializeField] float rayRange = 10f;
    [SerializeField] float jumpRayRange = 1f;
    [SerializeField] float orientationAdjustSpeed = 5f;
    [SerializeField] float tiltAmount = 30f;
    [SerializeField] float tiltDetectDist = 1f;

    GameObject player;
    GameObject playerCamera;
    Rigidbody rb;
    LayerMask layerMask;
    proceduralAnimation procAnimScript;

    private bool canJump = true;
    private bool isGrounded = true;

    private Vector3 movement;
    private Quaternion lookRotation;
    private Vector3 rayStartPoint;

    private GameObject procAnimObject;

    private void Start() 
    {
        procAnimObject = FindFirstObjectByType<proceduralAnimation>().gameObject;
        procAnimScript = procAnimObject.GetComponent<proceduralAnimation>(); 
        
        Cursor.visible = false;
        rb = GetComponent<Rigidbody>();
        player = gameObject;
        playerCamera = Camera.main.gameObject;
        layerMask = LayerMask.GetMask("Ground");
    }

    private void FixedUpdate() 
    {
        SphereCollider sphereCollider = player.gameObject.GetComponent<SphereCollider>();
        rayStartPoint = sphereCollider.transform.TransformPoint(sphereCollider.center);
        HandleMovement();
        RayCast();
    }

    void RayCast()
    {
        Vector3 startPos = transform.position;
        Vector3 playerDown = transform.up * -1f;

        RaycastHit hit;
        if(Physics.Raycast(startPos, playerDown, out hit, rayRange, layerMask))
        {
            // Am Able to include jump ray in here if fits
            movement = Vector3.ProjectOnPlane(movement, hit.normal);
            HandleOrientation(hit);
        }
        // Debug.DrawRay(transform.position, playerDown * rayRange, Color.yellow);
    }

    void HandleMovement()
    {
        if (player == null) return;

        Vector3 playerForward = player.transform.forward;
        Vector3 playerRight = player.transform.right;

        float XInput = Input.GetAxis("Horizontal");
        float ZInput = Input.GetAxis("Vertical");

        movement = (playerForward * ZInput + playerRight * XInput).normalized;

        if (movement != Vector3.zero)
        {
            TriggerLookDirection();
            // rb.MovePosition(transform.position + movement * moveSpeed * Time.fixedDeltaTime);
            rb.linearVelocity = new Vector3(movement.x * moveSpeed, rb.linearVelocity.y, movement.z * moveSpeed);
        }

        // HandleJump();
    }

    void HandleJump()
    {
        Vector3 playerDown = transform.up * -1f;

        RaycastHit hit;
        if (Physics.Raycast(transform.position, playerDown, out hit, jumpRayRange, layerMask))
        {
            ProcAnimEnabled(true);
            isGrounded = true;
            canJump = true;
            // Debug.Log("Grounded");
        }
        else
        {
            isGrounded = false;
            // Debug.Log("Not Grounded");
        }
        Debug.DrawRay(transform.position, playerDown * jumpRayRange, Color.blue);

        if (isGrounded && canJump && Input.GetAxis("Jump") > 0)
        {
            // Jump
            ProcAnimEnabled(false);
            rb.AddForce(transform.up * jumpThrust, ForceMode.Impulse);
            canJump = false;
        }
    }

    private void ProcAnimEnabled(bool isEnabled)
    {
        if (procAnimScript != null)
        {
            procAnimScript.enabled = isEnabled;
        }
        else
        {
            Debug.LogWarning("procAnim Script is null");
        }
    }

    void TriggerLookDirection()
    {
        // Get the direction the camera is facing (ignore the vertical component)
        Vector3 cameraForward = playerCamera.transform.forward;
        cameraForward.y = 0f;  // Ignore the vertical (y) direction to prevent tilting up/down

        // If the camera forward is not zero, update the player rotation
        if (cameraForward != Vector3.zero)
        {
            lookRotation = Quaternion.LookRotation(cameraForward, transform.up);
        }
    }

    void HandleOrientation(RaycastHit hit)
    {
        // Quaternion normalRotation = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
        // transform.rotation = Quaternion.Slerp(transform.rotation, normalRotation, Time.fixedDeltaTime * orientationAdjustSpeed);

        Quaternion tiltAngle;
        if (Physics.Raycast(rayStartPoint, player.transform.forward, tiltDetectDist, layerMask))
        {
            tiltAngle = Quaternion.Euler(tiltAmount, 0f, 0f);
        }
        else
        {
            tiltAngle = Quaternion.Euler(0f, 0f, 0f);
        }

        Debug.DrawLine(rayStartPoint, rayStartPoint + player.transform.forward * tiltDetectDist, Color.red);

        

        Quaternion groundRotation = Quaternion.FromToRotation(Vector3.up, hit.normal);

        Quaternion targetRotation = groundRotation * lookRotation * tiltAngle;
    
        transform.rotation = targetRotation;
        // transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * orientationAdjustSpeed);
    }

    //Idea: Gravity is in direction of -normal surface being climbed.

    //Add player orientation for jumping against walls
    //wall climbing

    // Quaternion groundRotation = Quaternion.FromToRotation(Vector3.up, groundNormal); // Align with ground
    // Quaternion tiltRotation = Quaternion.Euler(tiltAngle, 0f, 0f) * groundRotation; // Apply backward tilt *relative* to ground

    // // Blend both target rotations
    // Quaternion targetRotation = Quaternion.Slerp(groundRotation, tiltRotation, tiltWeight); // tiltWeight from 0 to 1

    // // Smoothly rotate current player rotation toward the target
    // transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);

}
