using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float moveSpeed = 3.5f;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] Vector2 playerInput = Vector2.zero;
    [SerializeField] bool lastDirLeft = false;

    public enum FacingDirection
    {
        left, right
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // The input from the player needs to be determined and
        // then passed in the to the MovementUpdate which should
        // manage the actual movement of the character.

        playerInput = Vector2.zero;

        if (Input.GetKey(KeyCode.A))
        {
            playerInput.x -= moveSpeed;
        }
        if (Input.GetKey(KeyCode.D))
        {
            playerInput.x += moveSpeed;
        }
        
        MovementUpdate(playerInput);
        IsWalking();
        GetFacingDirection();
    }

    private void MovementUpdate(Vector2 playerInput)
    {
        rb.position += playerInput * Time.deltaTime;
    }

    public bool IsWalking()
    {
        if (rb.linearVelocity != Vector2.zero)
        {
            return true;
        }
        else return false;
    }
    public bool IsGrounded()
    {
        if (rb.linearVelocityY == 0)
        {
            return true;
        }
        else return false;
    }

    public FacingDirection GetFacingDirection()
    {
        if (playerInput.x != 0)
        {
            if (playerInput.x > 0)
            {
                lastDirLeft = false;
                return FacingDirection.right;
            }
            else
            {
                lastDirLeft = true;
                return FacingDirection.left;
            }
        }
        else if (lastDirLeft)
        {
            return FacingDirection.left;
        }
        else return FacingDirection.right;
    }
}
