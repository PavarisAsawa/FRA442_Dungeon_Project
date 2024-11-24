using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CharacterController), typeof(Animator))]
public class PlayerControl : MonoBehaviour
{
    private CharacterController characterController;
    private Player player;
    private Animator animator;

    private float currentSpeed;
    public Transform cameraTransform; // Camera used for direction calculation

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

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        player = GetComponent<Player>();

        if (player == null)
        {
            Debug.LogError("Player component not found!");
        }
        currentSpeed = player.runSpeed;
        hitSound = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (isDead) return;

        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = 0.0f;
        }

        if (!isAttacking)
        {
            HandleMovement();
        }

        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);

        if (Input.GetMouseButtonDown(0) && !isAttacking)
        {
            animator.SetTrigger("Attack");
            isAttacking = true;  // เริ่มโจมตี
            hitSound.Play();
            isAttacking = true;
            StartCoroutine(AttackRoutine());
        }
    }

    private void HandleMovement()
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

            float targetAngle = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
            Quaternion targetRotation = Quaternion.Euler(0, targetAngle, 0);

            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);

            Vector3 move = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;
            characterController.Move(move * currentSpeed * Time.deltaTime);
        }
        else
        {
            animator.SetBool("isRunning", false);
            animator.SetBool("isSprint", false);
        }
    }

    IEnumerator AttackRoutine()
    {
        yield return new WaitForSeconds(0.5f);
        isAttacking = false;
    }

    public void PlayerTakeDamage(float damage)
    {
        if (isDead || isHit) return;

        player.playerHealth -= damage;
        isHit = true;
        animator.SetTrigger("isHit");

        if (player.playerHealth <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(HitCooldownRoutine());
        }
    }

    IEnumerator HitCooldownRoutine()
    {
        yield return new WaitForSeconds(hitCooldown);

        while (animator.GetCurrentAnimatorStateInfo(0).IsName("Hit") &&
               animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
        {
            yield return null;
        }

        animator.ResetTrigger("isHit");
        yield return new WaitForSeconds(1.0f);
        isHit = false;
    }

    void Die()
    {
        isDead = true;
        animator.SetBool("isRunning", false);
        animator.SetBool("isSprint", false);
        animator.SetBool("isHit", false);
        animator.SetBool("isDead", true);

        velocity = Vector3.zero;
        characterController.Move(Vector3.zero);

        GameManagerScript.Instance.ShowGameOver(); // Show Game Over screen
    }

}



