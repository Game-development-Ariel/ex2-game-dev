using UnityEngine;
using UnityEngine.InputSystem;

// Top-down movement. The body (sprite + flashlight) turns to face the walking direction,
// so the flashlight shows what the character is looking at.
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float turnSpeed = 720f;
    [SerializeField] private Transform body;

    private Rigidbody2D rb;
    private Vector2 moveInput;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        moveInput = ReadMoveInput();

        if (body != null && moveInput.sqrMagnitude > 0.01f)
        {
            // The sprite is drawn facing up, so up (+Y) is "forward".
            float angle = Mathf.Atan2(moveInput.y, moveInput.x) * Mathf.Rad2Deg - 90f;
            Quaternion facing = Quaternion.Euler(0f, 0f, angle);
            body.rotation = Quaternion.RotateTowards(body.rotation, facing, turnSpeed * Time.deltaTime);
        }
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = moveInput * moveSpeed;
    }

    private static Vector2 ReadMoveInput()
    {
        Vector2 input = Vector2.zero;

        Keyboard keyboard = Keyboard.current;
        if (keyboard != null)
        {
            if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed) input.y += 1f;
            if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed) input.y -= 1f;
            if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed) input.x += 1f;
            if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed) input.x -= 1f;
        }

        Gamepad gamepad = Gamepad.current;
        if (gamepad != null)
        {
            input += gamepad.leftStick.ReadValue();
        }

        return Vector2.ClampMagnitude(input, 1f);
    }
}
