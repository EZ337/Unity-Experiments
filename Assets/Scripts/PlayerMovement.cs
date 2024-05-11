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
    [SerializeField] private float fallMult = 2.5f;
    [SerializeField] private float lowJumpMult = 2.0f;


    private float moveAcceleration;
    private float moveDeceleration;
    private bool isMoving;


    private void Start()
    {
        // Calculate time to reach and leave maxSpeed
        moveAcceleration = maxSpeed / accelTime;
        moveDeceleration = maxSpeed / decelTime;
    }

    private void FixedUpdate()
    {
        // Process HorizontalMovement
        Move();
    }

    /// <summary>
    /// Handles the horizontal movement of the player
    /// </summary>
    private void Move()
    {
        if (moveDir.x != 0)
        {
            Accelerate(moveDir); // Accelerate in the input direction
            FlipSprite(); // Flip the sprite because we are moving
        }
        else if (Mathf.Abs(rb.velocity.x) > 0.0f)
        {
            Decelerate(-(rb.velocity)); // No input so 
        }

    }

    /// <summary>
    /// Accelerates the movement. Clamped at maxSpeeed
    /// </summary>
    /// <param name="direction">Direction to accelerate in</param>
    private void Accelerate(Vector2 direction)
    {
        if (Mathf.Abs(rb.velocity.x) < maxSpeed) // Constantly add force if we haven't reached max speed
            rb.AddForce(new Vector2(direction.x * moveAcceleration, 0.0f), ForceMode2D.Force);
        else // If we are >= maxSpeed, clamp velocity to maxSpeed
            rb.velocity = new(maxSpeed * Mathf.Sign(direction.x), rb.velocity.y);

        // We are moving. Might update for y
        isMoving = true; 
    }

    /// <summary>
    /// Decelerate the movement. Stops at 0
    /// </summary>
    /// <param name="direction">The opposite direction of the x velocity</param>
    private void Decelerate(Vector2 direction)
    {
        if (Mathf.Abs(rb.velocity.x) > 0.01f) // If moving, slow us down to full stop by decelTime
            rb.AddForce(new Vector2(moveDeceleration * direction.x, 0.0f), ForceMode2D.Force);
        else // Stop us completely
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
            isMoving = false; // Update moving to false
        }
    }

    /// <summary>
    /// Flips the sprite based on movement direction
    /// </summary>
    private void FlipSprite()
    {
        // Flip the sprite based on the direction of movement
        transform.localScale = new Vector2(Mathf.Sign(rb.velocity.x), transform.localScale.y);
    }


    #region Input Callbacks

    public void MoveAction(InputAction.CallbackContext move)
    {
        // Might need more conditions.
        if (move.performed)
        {
            moveDir = move.ReadValue<Vector2>();
        }
        else if (move.canceled) // Set moveDir to zero if no input
        {
            moveDir = Vector2.zero;
        }
    }

    public void JumpAction(InputAction.CallbackContext jump)
    {
        Debug.Log("Implement Jump Action");
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

    #endregion

}
