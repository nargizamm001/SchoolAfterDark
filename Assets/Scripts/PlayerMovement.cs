using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 4f;
    public float runSpeed = 7f;
    public float jumpHeight = 1.5f;
    public float gravity = -9.81f;

    [Header("Mouse Look")]
    public float mouseSensitivity = 2f;

    [Header("Camera References")]
    public Camera firstPersonCamera;
    public Camera thirdPersonCamera;
    public Transform thirdPersonCameraPivot;

    [Header("Third Person Camera")]
    public float thirdPersonDistance = 4f;
    public float cameraCollisionRadius = 0.25f;
    public LayerMask cameraCollisionLayers = ~0;

    [Header("Animation")]
    public Animator animator;

    [Header("Player Model")]
    public GameObject playerModel;

    private CharacterController controller;
    private Vector3 velocity;
    private float cameraPitch;
    private bool isThirdPerson = false;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }

        if (playerModel == null && animator != null)
        {
            playerModel = animator.gameObject;
        }

        SetCameraMode(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            SetCameraMode(!isThirdPerson);
        }

        RotateView();
        MovePlayer();
        UpdateThirdPersonCameraCollision();
    }

    void SetCameraMode(bool thirdPerson)
    {
        isThirdPerson = thirdPerson;

        if (firstPersonCamera != null)
        {
            firstPersonCamera.gameObject.SetActive(true);
            firstPersonCamera.enabled = !isThirdPerson;
        }

        if (thirdPersonCamera != null)
        {
            thirdPersonCamera.gameObject.SetActive(true);
            thirdPersonCamera.enabled = isThirdPerson;
        }

        SetPlayerModelVisible(isThirdPerson);
    }

    void SetPlayerModelVisible(bool visible)
    {
        if (playerModel == null) return;

        Renderer[] renderers = playerModel.GetComponentsInChildren<Renderer>();

        foreach (Renderer renderer in renderers)
        {
            renderer.enabled = visible;
        }
    }

    void RotateView()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxisRaw("Mouse Y") * mouseSensitivity;

        transform.Rotate(0f, mouseX, 0f);

        cameraPitch -= mouseY;
        cameraPitch = Mathf.Clamp(cameraPitch, -80f, 80f);

        if (!isThirdPerson && firstPersonCamera != null)
        {
            firstPersonCamera.transform.localRotation = Quaternion.Euler(cameraPitch, 0f, 0f);
        }

        if (isThirdPerson && thirdPersonCameraPivot != null)
        {
            thirdPersonCameraPivot.localRotation = Quaternion.Euler(cameraPitch, 0f, 0f);
        }
    }

    void MovePlayer()
    {
        bool isGrounded = controller.isGrounded;

        if (isGrounded && velocity.y < 0f)
        {
            velocity.y = -2f;
        }

        float x = 0f;
        float z = 0f;

        // У тебя было отзеркалено управление, поэтому оставляю так:
        if (Input.GetKey(KeyCode.A)) x = 1f;
        if (Input.GetKey(KeyCode.D)) x = -1f;

        if (Input.GetKey(KeyCode.W)) z = -1f;
        if (Input.GetKey(KeyCode.S)) z = 1f;

        Vector3 move = transform.right * x + transform.forward * z;
        move = Vector3.ClampMagnitude(move, 1f);

        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        float currentSpeed = isRunning ? runSpeed : walkSpeed;

        controller.Move(move * currentSpeed * Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

            if (animator != null)
            {
                animator.SetTrigger("Jump");
            }
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        if (animator != null)
        {
            float animationSpeed = move.magnitude;

            if (isRunning && animationSpeed > 0.1f)
            {
                animationSpeed = 2f;
            }

            animator.SetFloat("Speed", animationSpeed);
            animator.SetBool("IsGrounded", isGrounded);
        }
    }

    void UpdateThirdPersonCameraCollision()
{
    if (!isThirdPerson || thirdPersonCamera == null || thirdPersonCameraPivot == null)
    {
        return;
    }

    Vector3 pivotPosition = thirdPersonCameraPivot.position + Vector3.up * 0.2f;

    Vector3 desiredDirection = -thirdPersonCameraPivot.forward;
    Vector3 desiredCameraPosition = pivotPosition + desiredDirection * thirdPersonDistance + Vector3.up * 0.3f;

    Vector3 direction = desiredCameraPosition - pivotPosition;
    float maxDistance = direction.magnitude;

    float finalDistance = maxDistance;

    RaycastHit[] hits = Physics.SphereCastAll(
        pivotPosition,
        cameraCollisionRadius,
        direction.normalized,
        maxDistance,
        cameraCollisionLayers,
        QueryTriggerInteraction.Ignore
    );

    foreach (RaycastHit hit in hits)
    {
        if (hit.collider == null)
            continue;

        if (hit.collider.transform == transform)
            continue;

        if (hit.collider.transform.IsChildOf(transform))
            continue;

        if (hit.distance < finalDistance)
        {
            finalDistance = hit.distance;
        }
    }

    float wallBuffer = 0.7f;
    float minDistance = 0.8f;

    finalDistance = Mathf.Clamp(finalDistance - wallBuffer, minDistance, maxDistance);

    Vector3 finalPosition = pivotPosition + direction.normalized * finalDistance;

    thirdPersonCamera.transform.position = finalPosition;
    thirdPersonCamera.transform.LookAt(thirdPersonCameraPivot.position + Vector3.up * 0.7f);
}
}