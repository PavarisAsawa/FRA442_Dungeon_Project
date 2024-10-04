using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController), typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    private CharacterController characterController;
    private Animator animator;
    
    [SerializeField]
    private float movementSpeed, rotationSpeed, hitspeed;
    private Vector3 movementDirection = Vector3.zero;
    private Vector3 velocity = Vector3.zero;
    private float gravity = -9.81f;
    private bool isGrounded;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Input for movement
        float horizontalMovement = Input.GetAxisRaw("Horizontal");
        float verticalMovement = Input.GetAxisRaw("Vertical");

        // Determine movement direction
        Vector3 moveDirection = new Vector3(horizontalMovement, 0, verticalMovement).normalized;

        // Check if we have movement input
        if (moveDirection.magnitude >= 0.1f)
        {
            // Calculate the target angle based on movement input and rotate the character
            float targetAngle = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref rotationSpeed, 0.1f);
            transform.rotation = Quaternion.Euler(0, angle, 0);

            // Move the character in the direction of movement
            Vector3 move = transform.forward * movementSpeed * Time.deltaTime;
            characterController.Move(move);

            // Update running animation
            animator.SetBool("isRunning", true);
        }
        else
        {
            // Stop running animation
            animator.SetBool("isRunning", false);
        }

        // Apply gravity
        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);

        // Attack animation trigger on left mouse click
        if (Input.GetMouseButtonDown(0))
        {
            animator.SetTrigger("Attack");
        }
    }
}
