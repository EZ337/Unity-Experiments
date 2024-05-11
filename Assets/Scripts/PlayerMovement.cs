using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private Vector2 moveDir;

    [SerializeField] private Rigidbody2D rb;

    [Header("Movement Settings")]
    [SerializeField] private float maxSpeed = 30.0f;
    [SerializeField] private float accelTime = 0.8f;
    [SerializeField] private float decelTime = 0.5f;

    [Header("Jump Settings")]
    [SerializeField] private float jumpForce = 10.0f;


    private float moveAcceleration;
    private float moveDeceleration;
    private bool isMoving;

    private void Start()
    {
        moveAcceleration = maxSpeed / accelTime;
        moveDeceleration = maxSpeed / decelTime;
    }

    private void FixedUpdate()
    {
        HandleMovement();
        //Debug.Log(rb.velocity);
    }

    private void HandleMovement()
    {
        if (moveDir.x != 0)
        {
            Accelerate(moveDir);
        }
        else if (rb.velocity.x != 0.0f && isMoving)
        {
            Decelerate(-(rb.velocity));
        }
    }

    /// <summary>
    /// Decelerate the movement.
    /// </summary>
    /// <param name="direction">The opposite direction of the x velocity</param>
    private void Decelerate(Vector2 direction)
    {
        if (Mathf.Abs(rb.velocity.x) > 0.01f)
            rb.AddForce(new Vector2(moveDeceleration * direction.x, 0.0f), ForceMode2D.Force);
        else
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
            isMoving = false;
        }
    }

    /// <summary>
    /// Accelerates the movement. Clamped at maxSpeeed
    /// </summary>
    /// <param name="direction">Direction to accelerate in</param>
    private void Accelerate(Vector2 direction)
    {
        if (Mathf.Abs(rb.velocity.x) < maxSpeed)
            rb.AddForce(new Vector2(direction.x * moveAcceleration, 0.0f), ForceMode2D.Force);
        else
            rb.velocity = new(maxSpeed * Mathf.Sign(direction.x), rb.velocity.y);

        isMoving = true;
    }


    public void Move(InputAction.CallbackContext move)
    {
        if (move.performed)
        {
            moveDir = move.ReadValue<Vector2>();
        }
        else if (move.canceled)
        {
            moveDir = Vector2.zero;
        }
    }

    public void Jump(InputAction.CallbackContext jump)
    {
        Debug.Log("Called Jump");
        if (jump.performed)
        {
            Debug.Log("Jump Performed");
            rb.AddForce(new(0, jumpForce), ForceMode2D.Impulse);
        }
    }



#if UNITY_EDITOR // Recalculate acceleration if changed in editor
    /// <summary>
    /// Recalculates the acceleration if we change a value in the editor
    /// </summary>
    private void OnValidate()
    {
        moveAcceleration = maxSpeed / accelTime;
        moveDeceleration = maxSpeed / decelTime;
    }
#endif
}
