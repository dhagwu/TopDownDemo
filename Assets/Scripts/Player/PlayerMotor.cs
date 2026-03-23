using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMotor : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference dashAction;

    [Header("References")]
    [SerializeField] private Transform modelRoot;

    [Header("Move Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float turnSpeed = 12f;
    [SerializeField] private float gravity = -20f;

    [Header("Dash Settings")]
    [SerializeField] private float dashSpeed = 12f;
    [SerializeField] private float dashDuration = 0.12f;

    private CharacterController controller;
    private Vector3 verticalVelocity;
    private Vector3 dashDirection;
    private float dashTimer;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    private void OnEnable()
    {
        moveAction.action.Enable();
        dashAction.action.Enable();
        dashAction.action.performed += OnDash;
    }

    private void OnDisable()
    {
        dashAction.action.performed -= OnDash;
        moveAction.action.Disable();
        dashAction.action.Disable();
    }

    private void Update()
    {
        Vector2 input = moveAction.action.ReadValue<Vector2>();

        Vector3 camForward = Camera.main.transform.forward;
        Vector3 camRight = Camera.main.transform.right;

        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();

        Vector3 moveDir = camForward * input.y + camRight * input.x;

        if (moveDir.sqrMagnitude > 1f)
            moveDir.Normalize();

        float currentSpeed = moveSpeed;

        if (dashTimer > 0f)
        {
            dashTimer -= Time.deltaTime;
            moveDir = dashDirection;
            currentSpeed = dashSpeed;
        }

        if (controller.isGrounded && verticalVelocity.y < 0f)
            verticalVelocity.y = -2f;

        verticalVelocity.y += gravity * Time.deltaTime;

        Vector3 finalMove = moveDir * currentSpeed + verticalVelocity;
        controller.Move(finalMove * Time.deltaTime);

        if (moveDir.sqrMagnitude > 0.001f && modelRoot != null)
        {
            Quaternion targetRot = Quaternion.LookRotation(moveDir);
            modelRoot.rotation = Quaternion.Slerp(
                modelRoot.rotation,
                targetRot,
                turnSpeed * Time.deltaTime
            );
        }
    }

    private void OnDash(InputAction.CallbackContext ctx)
    {
        Vector2 input = moveAction.action.ReadValue<Vector2>();
        if (input.sqrMagnitude <= 0.01f) return;

        Vector3 camForward = Camera.main.transform.forward;
        Vector3 camRight = Camera.main.transform.right;

        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();

        dashDirection = (camForward * input.y + camRight * input.x).normalized;
        dashTimer = dashDuration;
    }
}