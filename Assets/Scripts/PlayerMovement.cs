using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    //public Controller controller;
    [SerializeField] public float maxMoveSpeed;
    [SerializeField] public float movementAcceleration;
    [SerializeField] public float groundLinearDrag;
    [SerializeField] public float jumpForce;
    private Rigidbody2D rb;
    private float horizontalDirection;
    private bool changingDirection => (rb.velocity.x > 0f && horizontalDirection < 0f) || (rb.velocity.x < 0f && horizontalDirection > 0f);

    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundRaycastLength;
    [SerializeField] private float airLinearDrag;

    [SerializeField] private float fallMultiplier;
    [SerializeField] private float lowJumpFallMultiplier;
    private bool onGround;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    void Update()
    {
        horizontalDirection = GetInput().x;
        if (Input.GetButtonDown("Jump") && onGround)
            Jump();
    }
    private Vector2 GetInput()
    {
        return new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    }
    
    private void MoveCharacter()
    {
        rb.AddForce(new Vector2(horizontalDirection, 0f) * movementAcceleration);

        if (Mathf.Abs(rb.velocity.x) > maxMoveSpeed) 
            rb.velocity = new Vector2(Mathf.Sign(rb.velocity.x) * maxMoveSpeed, rb.velocity.y);
    }

    private void ApplyGroundLinearDrag()
    {
        if (Mathf.Abs(horizontalDirection) < 0.4f || changingDirection)
        {
            rb.drag = groundLinearDrag;
        } 
        else
        {
            rb.drag = 0f;
        }
    }

    private void ApplyAirLinearDrag()
    {
        rb.drag = airLinearDrag;
    }

    private void FallMultiplier()
    {
        if (rb.velocity.y < 0) 
        {
            rb.gravityScale = fallMultiplier;
        }
        else if (rb.velocity.y > 0 && !Input.GetButton("Jump")) 
        {
            rb.gravityScale = lowJumpFallMultiplier;
        }
        else 
        {
            rb.gravityScale = 1f;
        }
    }

    void FixedUpdate()
    {
        CheckCollisions();
        MoveCharacter();
        ApplyGroundLinearDrag();
    }

    void Jump()
    {
            rb.velocity = new Vector2(rb.velocity.x, 0f);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    private void CheckCollisions()
    {
        onGround = Physics2D.Raycast(transform.position * groundRaycastLength, Vector2.down, groundRaycastLength, groundLayer);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * groundRaycastLength);
    }
}
