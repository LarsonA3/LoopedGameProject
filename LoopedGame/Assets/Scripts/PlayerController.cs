using UnityEngine;
using UnityEngine.InputSystem;



public class TopDownController : MonoBehaviour
{


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


    void Trytodash()
    {
        if (Time.time < lastDashTime + dashCooldown) return;

        Vector3 dashDir = new Vector3(moveInput.x, 0f, moveInput.y).normalized;
        if (dashDir == Vector3.zero) dashDir = transform.forward;

        cc.enabled = false;
        transform.position += dashDir * dashDistance;
        cc.enabled = true;

        lastDashTime = Time.time;
    }


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