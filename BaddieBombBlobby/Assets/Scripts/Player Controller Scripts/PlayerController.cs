using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    public Rigidbody2D rb;
    private Vector2 moveDirection;
    private Animator animator;
   
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        ;
    }

    void Update()
    {
        rb.linearVelocity = moveDirection * speed;  
    }

    public void Move(InputAction.CallbackContext context)
    {
        animator.SetBool("isWalking", true);
        if (context.canceled)
        {
            animator.SetBool("isWalking", false);
            animator.SetFloat("LastInputX", moveDirection.x);
            animator.SetFloat("LastInputY", moveDirection.y);
        }

        moveDirection = context.ReadValue<Vector2>().normalized;
        animator.SetFloat("InputX", moveDirection.x);
        animator.SetFloat("InputY", moveDirection.y);
    }
}
