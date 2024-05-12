using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private Vector2 moveDir;

    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator animator;

    [Header("Movement Settings")]
    [SerializeField] private float maxSpeed = 30.0f;
    [SerializeField] private float accelTime = 0.8f;
    [SerializeField] private float decelTime = 0.5f;

    [Header("Jump Settings")]
    [SerializeField] private float jumpForce = 10.0f;
    [SerializeField] private float fallMult = 2.5f;
    [SerializeField] private float jumpTime = 0.2f;
    [SerializeField] private float maxFallSpeed = 50.0f;
    [SerializeField] private float heavyFallThreshold;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayers;


    private float moveAcceleration;
    private float moveDeceleration;
    private bool isMoving;
    private bool isJumping;
    private bool isFalling;
    private bool heavyLand;
    private bool isGrounded;
    private float jumpDur;

    // Animator Hashes
    int animRun = Animator.StringToHash("isRunning");
    int animJump = Animator.StringToHash("isJumping");
    int animFall = Animator.StringToHash("isFalling");
    int animLand = Animator.StringToHash("heavyLand");


    private void Start()
    {
        // Calculate time to reach and leave maxSpeed
        moveAcceleration = maxSpeed / accelTime;
        moveDeceleration = maxSpeed / decelTime;
        jumpDur = jumpTime;
    }

    private void FixedUpdate()
    {
        // Process HorizontalMovement
        Move();
        Fall();
        IsGrounded();
        if (isJumping)
            Jump();

        Debug.Log(rb.velocity);
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

        animator.SetBool(animRun, isMoving);

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

    private void Jump()
    {
        if (jumpDur < 0)
        {
            CancelJump();
        }

        // If we are in the air and we are still holding the jump
        else if (isJumping && jumpDur > 0.0f)
        {
            //rb.AddForce(transform.up * jumpForce, ForceMode2D.Force);
            jumpDur -= Time.fixedDeltaTime;
            isJumping = true;
        }
    }

    /// <summary>
    /// Cancels the jump
    /// </summary>
    private void CancelJump()
    {
        if (rb.velocity.y > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0);
        }

        isJumping = false;
        jumpDur = jumpTime;
        animator.SetBool(animJump, isJumping);
    }

    /// <summary>
    /// Function to handle falling. MoveSpeed increases the longer you fall
    /// </summary>
    private void Fall()
    {
        if (rb.velocity.y < 0.0f)
        {
            isFalling = true;
            animator.SetBool(animFall, isFalling);
            
            // Increase fallSpeed the longer we fall
            Vector2 newVal = (Vector2) (fallMult * Physics2D.gravity.y * Time.fixedDeltaTime * transform.up) + rb.velocity;
            if (newVal.y < maxFallSpeed)
            {
                // Clamp fallSpeed
                rb.velocity = new Vector2(rb.velocity.x, maxFallSpeed);
                Debug.Log("Max Fall Speed reached");
            }
            else
            {
                // Sets heavyLand to true if we're falling at about 1/3 of of our maxFallSpeed
                if (rb.velocity.y < heavyFallThreshold)
                {
                    heavyLand = true;
                }

                rb.velocity = newVal;
            }
        }
        else if (isFalling && IsGrounded())
        {
            isFalling = false;

            animator.SetBool(animFall, isFalling);
            // Trigger heavyLand anim like HollowKnight
            if (heavyLand)
            {
                animator.SetTrigger(animLand);
                // TODO: Disable movement for a few second
                heavyLand = false;
            }

            // Reset jump duration
            jumpDur = jumpTime;
        }
    }

    /// <summary>
    /// Grounded check
    /// </summary>
    /// <returns>true if grounded. False otherwise</returns>
    private bool IsGrounded()
    {
        // Ground check
        if (Physics2D.OverlapCapsule(groundCheck.position, new(1, 0.25f), CapsuleDirection2D.Horizontal, 0f, groundLayers))
        {
            jumpDur = jumpTime;
            isGrounded = true;
            return true;
        }

        isGrounded = false;
        return false;
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
        if (IsGrounded() && jump.performed)
        {
            isJumping = true;
            rb.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);
            animator.SetBool(animJump, isJumping);
        }

        else if (jump.canceled && isJumping) // Check if isJumping is true before canceling
        {
            CancelJump();
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

    #endregion

}
