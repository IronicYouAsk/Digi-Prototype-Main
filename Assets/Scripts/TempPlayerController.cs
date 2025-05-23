using UnityEngine;
using UnityEngine.InputSystem;

public class TempPlayerController : MonoBehaviour
{
    [SerializeField] float playerSpeed = 5;
    [SerializeField] InputActionReference movementRef;

    Rigidbody rb;
    private Vector2 movementInput;

    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
    }

    void Update()
    {
        GatherInput();
        HandleMovement();

        Debug.Log(movementInput);
    }

    void GatherInput()
    {
        movementInput = movementRef.action.ReadValue<Vector2>();
    }

    void HandleMovement()
    {
        Vector2 movement = (transform.forward * movementInput.y) + (transform.right * movementInput.x);

        // Vector2 targetMovement = movement * playerSpeed;

        rb.MovePosition(movement * playerSpeed * Time.deltaTime);
    }
}
