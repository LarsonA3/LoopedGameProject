using UnityEngine;
using UnityEngine.InputSystem;



public class TopDownController : MonoBehaviour
{

    private Collider playerCollider;
    public float moveSpeed = 6f;
    public float gravity = -20f;
    public float stickRotationSpeed = 10f;
    public float stickDeadzone = 0.2f;

    //dash stuff
    private float dashDistance = 2f;
    private float dashCooldown = 1f;

    private CharacterController cc;
    private Camera cam;
    private float verticalVelocity;
    private Vector2 moveInput;
    private float lastDashTime = -99f;

    private bool isGamepad;

    public void OnMove(InputValue value) => moveInput = value.Get<Vector2>();

    void Start()
    {
        cc = GetComponent<CharacterController>();
        cam = Camera.main;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        playerCollider = GetComponent<Collider>();
    }

    void Update()
    {
        Move();
        Rotate();

        if (Mouse.current != null && Mouse.current.delta.ReadValue().sqrMagnitude > 0.1f)
            isGamepad = false;

        if (Keyboard.current.leftShiftKey.wasPressedThisFrame) Trytodash();

        Gamepad pad = Gamepad.current;
        if (pad != null && pad.buttonSouth.wasPressedThisFrame) Trytodash();
    }


    void Move()
    {
        Vector3 move = new Vector3(moveInput.x, 0f, moveInput.y).normalized * moveSpeed;

        if (cc.isGrounded) verticalVelocity = -2f;
        else verticalVelocity += gravity * Time.deltaTime;

        move.y = verticalVelocity;
        cc.Move(move * Time.deltaTime);
    }


    // ---------------------------

    void Trytodash()
    {
        if (Time.time < lastDashTime + dashCooldown) return;

        Vector3 dashDir = new Vector3(moveInput.x, 0f, moveInput.y).normalized;
        if (dashDir == Vector3.zero) dashDir = transform.forward;

        Vector3 destination = FindDashDestination(dashDir);

        //if nothing cancel dash
        if (destination == transform.position) return;

        cc.enabled = false;
        transform.position = destination;
        cc.enabled = true;

        lastDashTime = Time.time;
    }

    Vector3 FindDashDestination(Vector3 dashDir)
    {
        int steps = 10; // how many pts to check along dash path
        float stepSize = dashDistance / steps;
        Vector3 lastValid = transform.position;

        for (int i = 1; i <= steps; i++)
        {
            Vector3 checkPos = transform.position + dashDir * (stepSize * i);

            if (IsWalkable(checkPos))
                lastValid = checkPos;
            else
                break; // stop first invalid step
        }

        return lastValid;
    }

    bool IsWalkable(Vector3 pos)
    {
        RaycastHit[] hits = Physics.RaycastAll(pos + Vector3.up * 2f, Vector3.down, 4f);

        foreach (RaycastHit hit in hits)
        {
            if (hit.collider == playerCollider) continue; // skip self
            return hit.collider.CompareTag("WALKABLE PLAYER FLOOR");
        }

        return false;
    }

    // ----------------


    void Rotate()
    {
        Gamepad pad = Gamepad.current;
        if (pad != null)
        {
            Vector2 stick = pad.rightStick.ReadValue();
            if (stick.magnitude > stickDeadzone)
            {
                isGamepad = true;
                RotateController(stick);
                return;
            }
        }

        if (isGamepad) return; // make sure isnt controller
        RotateMOUSE();
    }



    void RotateMOUSE()
    {
        if (Mouse.current == null) return;

        Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
        Plane groundPlane = new Plane(Vector3.up, transform.position);

        if (groundPlane.Raycast(ray, out float distance))
        {
            Vector3 lookPoint = ray.GetPoint(distance);
            lookPoint.y = transform.position.y;
            Vector3 dir = lookPoint - transform.position;
            if (dir.sqrMagnitude > 0.01f)
                transform.rotation = Quaternion.LookRotation(dir);
        }
    }

    void RotateController(Vector2 stick)
    {
        Quaternion targetRot = Quaternion.LookRotation(new Vector3(stick.x, 0f, stick.y));
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, stickRotationSpeed * Time.deltaTime);
    }
}