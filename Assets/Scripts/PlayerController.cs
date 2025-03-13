using Lolopupka;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SocialPlatforms;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float jumpThrust = 5f;
    [SerializeField] float rayRange = 10f;
    [SerializeField] float jumpRayRange = 1f;
    [SerializeField] float orientationAdjustSpeed = 5f;

    GameObject player;
    GameObject playerCamera;
    Rigidbody rb;
    LayerMask layerMask;

    private bool canJump = true;
    private bool isGrounded = true;

    private Vector3 movement;

    private void Start() 
    {
        Cursor.visible = false;
        rb = GetComponent<Rigidbody>();
        player = gameObject;
        playerCamera = Camera.main.gameObject;
        layerMask = LayerMask.GetMask("Ground");
    }

    private void FixedUpdate() 
    {
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

        HandleJump();
    }

    void HandleJump()
    {
        Vector3 playerDown = transform.up * -1f;

        RaycastHit hit;
        if (Physics.Raycast(transform.position, playerDown, out hit, jumpRayRange, layerMask))
        {
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
            rb.AddForce(transform.up * jumpThrust, ForceMode.Impulse);
            canJump = false;
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
            Quaternion lookRotation = Quaternion.LookRotation(cameraForward, transform.up);
            player.transform.rotation = lookRotation;
        }
    }

    void HandleOrientation(RaycastHit hit)
    {
        Quaternion normalRotation = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
        transform.rotation = Quaternion.Slerp(transform.rotation, normalRotation, Time.fixedDeltaTime * orientationAdjustSpeed);
    }

    //Idea: Gravity is in direction of -normal surface being climbed.

    //Add player orientation for jumping against walls
    //wall climbing
}
