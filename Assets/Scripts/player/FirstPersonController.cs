using UnityEngine;

public class FirstPersonController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 4f;
    public float sprintSpeed = 7f;
    public float turnSpeed = 120f;
    public float gravity = -9.81f;

    [Header("Camera")]
    public Transform playerCamera;

    private CharacterController characterController;
    private Vector3 velocity;

    void Start()
    {
        characterController = GetComponent<CharacterController>();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (playerCamera != null)
        {
            playerCamera.localRotation = Quaternion.identity;
        }
    }

    void Update()
    {
        HandleJoystickMovement();
        ApplyGravity();
    }

    void HandleJoystickMovement()
    {
        // Joystick Up/Down = Forward/Backward
        float moveInput = Input.GetAxis("Vertical");

        // Joystick Left/Right = Turn Left/Right
        float turnInput = Input.GetAxis("Horizontal");

        // Rotate the player
        transform.Rotate(Vector3.up * turnInput * turnSpeed * Time.deltaTime);

        // Sprint button: Physical joystick button 6
        // LeftShift is kept only for testing from keyboard
        bool isSprinting = Input.GetKey(KeyCode.JoystickButton6) || Input.GetKey(KeyCode.LeftShift);

        float currentSpeed = isSprinting ? sprintSpeed : moveSpeed;

        // Move player forward/backward based on where player is facing
        Vector3 move = transform.forward * moveInput;
        characterController.Move(move * currentSpeed * Time.deltaTime);
    }

    void ApplyGravity()
    {
        if (characterController.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);
    }
}