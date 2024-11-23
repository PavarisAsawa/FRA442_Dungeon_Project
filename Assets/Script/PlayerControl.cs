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
    public Transform cameraTransform; // กล้องที่จะใช้ในการคำนวณทิศทาง

    private Vector3 velocity = Vector3.zero;
    private float gravity = -9.81f;
    private bool isGrounded;
    public bool isAttacking = false;
    private bool isDead = false;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    public float hitCooldown = 1.5f; // Duration of the cooldown period
    private bool isHit = false; // Flag to track if hit animation is playing
    public GameObject gameOverCanvasPrefab; // Assign this in the Inspector
    private  AudioSource hitSound;
    
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
        hitSound = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (isDead) return;  // หยุดการทำงานหากตัวละครตายแล้ว

        // Check if the player is grounded
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = 0.0f;
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

                // คำนวณมุมทิศทางจากกล้อง
                float targetAngle = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
                Quaternion targetRotation = Quaternion.Euler(0, targetAngle, 0);

                // หมุนตัวละครไปตามทิศทางเป้าหมายทันที
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);

                // คำนวณทิศทางการเคลื่อนที่ใหม่จากการหมุน
                Vector3 move = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;
                characterController.Move(move * currentSpeed * Time.deltaTime);
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
            hitSound.Play();
            StartCoroutine(AttackRoutine());
        }
    }

    IEnumerator AttackRoutine()
    {
        yield return new WaitForSeconds(0.5f);
        isAttacking = false;
    }

    // ฟังก์ชันสำหรับการรับความเสียหายและเรียกใช้อนิเมชันโดนตี
    public void PlayerTakeDamage(float damage)
    {
        if (isDead || isHit) return;  // Skip if dead or already in hit animation or invincible

        player.playerHealth -= damage;
        isHit = true;  // Set the hit flag to prevent multiple triggers
        animator.SetTrigger("isHit");  // Trigger the hit animation
        Debug.Log("Player takes damage, playing hit animation.");

        if (player.playerHealth <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(HitCooldownRoutine());
        }
    }

    // Cooldown coroutine to reset the isHit flag after a short period and apply invincibility
    IEnumerator HitCooldownRoutine()
    {
        yield return new WaitForSeconds(hitCooldown); // รอให้อนิเมชันการโดนตีเล่นจนจบ

        // ตรวจสอบว่าอนิเมชัน "isHit" เล่นเสร็จสมบูรณ์ก่อนรีเซ็ต
        while (animator.GetCurrentAnimatorStateInfo(0).IsName("Hit") && animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
        {
            yield return null; // รอจนกว่าอนิเมชันจะจบ
        }

        animator.ResetTrigger("isHit"); // รีเซ็ตทริกเกอร์ isHit
        Debug.Log("Hit animation completed.");

        // ระยะเวลาปลอดภัยหลังจากโดนตี
        yield return new WaitForSeconds(3.0f); // ปลอดภัยเป็นเวลา 3 วินาที
        isHit = false;  // Reset the flag to allow a new hit animation
        Debug.Log("Invincibility period ended, player can take damage again.");
    }
    
    void Die()
    {
        isDead = true;
        animator.SetBool("isRunning", false);
        animator.SetBool("isSprint", false);
        animator.SetBool("isHit", false);
        animator.SetBool("isDead", true);

        // หยุดการอัปเดตความเร็วเพื่อไม่ให้แรงโน้มถ่วงมีผล
        velocity = Vector3.zero;

        // หยุดการเคลื่อนที่ของ CharacterController
        characterController.Move(Vector3.zero);

        // แสดง Game Over Canvas
        if (gameOverCanvasPrefab != null)
        {
            Instantiate(gameOverCanvasPrefab); // สร้าง Game Over Canvas ขึ้นมา
        }
        else
        {
            Debug.LogWarning("Game Over Canvas Prefab is not assigned in the Inspector!");
        }
    }
}