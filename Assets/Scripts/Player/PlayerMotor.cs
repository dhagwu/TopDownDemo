using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMotor : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference dashAction;

    [Header("Config")]
    [SerializeField] private PlayerConfigSO config;

    [Header("References")]
    [SerializeField] private Transform modelRoot;
    [SerializeField] private Animator animator;

    private CharacterController controller;
    private Vector3 verticalVelocity;
    private Vector3 dashDirection;
    private float dashTimer;
    private float dashCooldownTimer;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    private void OnEnable()
    {
        if (moveAction != null)
            moveAction.action.Enable();

        if (dashAction != null)
        {
            dashAction.action.Enable();
            dashAction.action.performed += OnDash;
        }
    }

    private void OnDisable()
    {
        if (dashAction != null)
        {
            dashAction.action.performed -= OnDash;
            dashAction.action.Disable();
        }

        if (moveAction != null)
            moveAction.action.Disable();
    }

    private void Update()
    {
        if (config == null || moveAction == null)
            return;

        if (dashCooldownTimer > 0f)
            dashCooldownTimer -= Time.deltaTime;

        Vector2 input = moveAction.action.ReadValue<Vector2>();
        Vector3 moveDir = GetCameraRelativeMove(input);

        bool isDashing = dashTimer > 0f;
        if (isDashing)
        {
            dashTimer -= Time.deltaTime;
            moveDir = dashDirection;
        }

        if (controller.isGrounded && verticalVelocity.y < 0f)
            verticalVelocity.y = -2f;

        verticalVelocity.y += config.gravity * Time.deltaTime;

        float currentSpeed = isDashing ? config.dashSpeed : config.moveSpeed;
        Vector3 finalMove = moveDir * currentSpeed + verticalVelocity;

        controller.Move(finalMove * Time.deltaTime);

        if (moveDir.sqrMagnitude > 0.001f && modelRoot != null)
        {
            Quaternion targetRot = Quaternion.LookRotation(moveDir);
            modelRoot.rotation = Quaternion.Slerp(
                modelRoot.rotation,
                targetRot,
                config.turnSpeed * Time.deltaTime
            );
        }

        if (animator != null)
        {
            float speedValue = moveDir.magnitude;
            animator.SetFloat("Speed", speedValue);
        }
    }

    private Vector3 GetCameraRelativeMove(Vector2 input)
    {
        Vector3 moveDir;

        if (Camera.main == null)
        {
            moveDir = new Vector3(input.x, 0f, input.y);
            if (moveDir.sqrMagnitude > 1f)
                moveDir.Normalize();
            return moveDir;
        }

        Transform cam = Camera.main.transform;

        Vector3 camForward = cam.forward;
        Vector3 camRight = cam.right;

        camForward.y = 0f;
        camRight.y = 0f;

        camForward.Normalize();
        camRight.Normalize();

        moveDir = camForward * input.y + camRight * input.x;

        if (moveDir.sqrMagnitude > 1f)
            moveDir.Normalize();

        return moveDir;
    }

    private void OnDash(InputAction.CallbackContext ctx)
    {
        if (config == null || moveAction == null)
            return;

        if (dashCooldownTimer > 0f)
            return;

        Vector2 input = moveAction.action.ReadValue<Vector2>();
        Vector3 moveDir = GetCameraRelativeMove(input);

        if (moveDir.sqrMagnitude <= 0.001f)
            return;

        dashDirection = moveDir.normalized;
        dashTimer = config.dashDuration;
        dashCooldownTimer = config.dashCooldown;
    }
}