using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController), typeof(Animator))]
public class PlayerControl : MonoBehaviour
{
    private CharacterController characterController;
    private Player player;
    private Animator animator;

    private float currentSpeed;

    private Vector3 velocity = Vector3.zero;
    private float gravity = -9.81f;
    private bool isGrounded;
    public bool isAttacking = false;
    private bool isDead = false;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        player = GetComponent<Player>();  // เพิ่มการกำหนดค่า player

        if (player == null)
        {
            Debug.LogError("Player component not found!");
        }
        currentSpeed = player.runSpeed;
    }

    void Update()
    {
        if (isDead) return;  // หยุดการทำงานหากตัวละครตายแล้ว

        // Check if the player is grounded
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // Handle movement and attack
        if (!isAttacking)
        {
            float horizontalMovement = Input.GetAxisRaw("Horizontal");
            float verticalMovement = Input.GetAxisRaw("Vertical");

            Vector3 moveDirection = new Vector3(horizontalMovement, 0, verticalMovement).normalized;

            if (moveDirection.magnitude >= 0.1f)
            {
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    currentSpeed = player.sprintSpeed;
                    animator.SetBool("isSprint", true);
                    animator.SetBool("isRunning", false);
                }
                else
                {
                    currentSpeed = player.runSpeed;
                    animator.SetBool("isSprint", false);
                    animator.SetBool("isRunning", true);
                }

                transform.rotation = Quaternion.LookRotation(moveDirection);
                Vector3 move = transform.forward * currentSpeed * Time.deltaTime;
                characterController.Move(move);
            }
            else
            {
                animator.SetBool("isRunning", false);
                animator.SetBool("isSprint", false);
            }
        }

        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);

        if (Input.GetMouseButtonDown(0) && !isAttacking)
        {
            animator.SetTrigger("Attack");
            isAttacking = true;  // เริ่มโจมตี
            StartCoroutine(AttackRoutine());
        }
    }

    IEnumerator AttackRoutine()
    {
        yield return new WaitForSeconds(0.5f);
        isAttacking = false;
    }

    // ฟังก์ชันสำหรับการรับความเสียหายและเรียกใช้อนิเมชันโดนตี
    public void TakeDamage(float damage)
    {
        if (isDead) return;

        player.playerHealth -= damage;

        // Trigger the hit reaction animation
        animator.SetTrigger("isHit");

        if (player.playerHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;
        animator.SetBool("isDead", true);
        characterController.enabled = false;
    }
}