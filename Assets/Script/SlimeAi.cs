
using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

// SlimeAnimationState { Idle,Walk,Jump,Attack,Damage}

public class SlimeAi : MonoBehaviour
{
    public Face faces;
    public GameObject SmileBody;
    public SlimeAnimationState currentState;

    public Animator animator;
    public NavMeshAgent agent;
    public int damType;

    private bool move;
    private Material faceMaterial;
    private Vector3 originPos;

    public enum WalkType { Patroll, ToOrigin, ExploreRandom, Chassing }
    private WalkType walkType;
    public float maxHealth = 100f;
    public float slimeHealth = 100f;
    private bool isTakingDamage = false;
    public float AttackDamage = 10;

    // Vision System Variable
    public float viewRadius = 10f;  // ระยะการมองเห็น
    public float attackingViewRadius = 15f;
    [Range(0, 360)] public float viewAngle = 120f;  // มุมมอง (FOV)
    public LayerMask targetMask;    // เลเยอร์ของวัตถุที่ต้องการตรวจสอบ
    public LayerMask obstacleMask;  // เลเยอร์ของวัตถุที่เป็นสิ่งกีดขวาง

    public List<Transform> visibleTargets = new List<Transform>(); // วัตถุที่มองเห็นได้

    [HideInInspector]
    public Transform targetTransform;

    [HideInInspector]
    public bool playerDetected;

    // Existing variables
    private bool canAttack = true;  // Control attack cooldown
    public float attackCooldown = 1.0f;  // Cooldown duration in seconds
    
    // Track previous behavior
    private WalkType previousWalkType;

    void Start()
    {
        originPos = transform.position;
        faceMaterial = SmileBody.GetComponent<Renderer>().materials[1];
        walkType = WalkType.ExploreRandom;
        StartCoroutine("FindTargetsWithDelay", 0.2f);  // ค้นหาทุกๆ 0.2 วินาที
    }

    void SetFace(Texture tex)
    {
        faceMaterial.SetTexture("_MainTex", tex);
    }
    void Update()
    {


        switch (currentState)
        {
            case SlimeAnimationState.Idle:

                if (animator.GetCurrentAnimatorStateInfo(0).IsName("Idle")) return;
                StopAgent();
                SetFace(faces.Idleface);
                break;

            case SlimeAnimationState.Walk:

                if (animator.GetCurrentAnimatorStateInfo(0).IsName("Walk")) return;

                agent.isStopped = false;
                agent.updateRotation = true;

                if (walkType == WalkType.ToOrigin)
                {
                    agent.SetDestination(originPos);
                    // Debug.Log("WalkToOrg");
                    SetFace(faces.WalkFace);
                    // agent reaches the destination
                    if (agent.remainingDistance < agent.stoppingDistance)
                    {
                        walkType = WalkType.Patroll;

                        //facing to camera
                        transform.rotation = Quaternion.identity;

                        currentState = SlimeAnimationState.Idle;
                    }

                }
                //Patroll
                else if (walkType == WalkType.ExploreRandom)
                {
                    float threshold = 2.0f; // ค่า threshold สำหรับตรวจสอบระยะทางใกล้เป้าหมาย
                    if (agent.remainingDistance <= threshold && !agent.pathPending)
                    {
                        // Slime เดินถึงจุดหมายแล้วหรือใกล้พอ

                        // ตรวจสอบว่า Animation การเดินกำลังเล่นอยู่หรือไม่และดูว่า Animation จบหรือยัง
                        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
                        if (stateInfo.IsName("Walk") && stateInfo.normalizedTime < 1.0f)
                        {
                            // ถ้า Animation การเดินยังไม่จบ (normalizedTime < 1.0f) ให้รอ
                            return;
                        }

                        // ถ้า Animation จบแล้ว เริ่มสุ่มจุดหมายใหม่
                        Vector3 randomDestination;
                        float minDistance = 10.0f; // กำหนดระยะห่างขั้นต่ำที่ต้องการ
                        if (RandomPoint(transform.position, 25.0f, minDistance, out randomDestination)) // Range 25.0f เป็นระยะของการเดินสุ่ม
                        {
                            // ตั้งค่าเส้นทางใหม่
                            agent.SetDestination(randomDestination);
                            SetFace(faces.WalkFace); // เปลี่ยนใบหน้าเมื่อเริ่มเดินใหม่
                            // ควบคุมการเปลี่ยนแปลง Animation โดยอิงจากความเร็วของ agent
                            animator.SetFloat("Speed", agent.velocity.magnitude); // ใช้พารามิเตอร์ Speed แทนการใช้ Trigger
                        }
                    }
                }
                else if (walkType == WalkType.Chassing && targetTransform != null)
                {
                    // ไล่ตามเป้าหมาย
                    agent.SetDestination(targetTransform.position);

                    // ถ้า Slime วิ่งไล่ตามถึงเป้าหมาย ให้เปลี่ยนไป Attack
                    if (agent.remainingDistance < agent.stoppingDistance + 4.0f)
                    {
                        currentState = SlimeAnimationState.Attack;
                    }
                }
                else
                {
                    // agent reaches the destination
                    if (agent.remainingDistance < agent.stoppingDistance)
                    {
                        currentState = SlimeAnimationState.Idle;
                    }

                }
                // set Speed parameter synchronized with agent root motion moverment
                animator.SetFloat("Speed", agent.velocity.magnitude);
                break;

            case SlimeAnimationState.Jump:

                if (animator.GetCurrentAnimatorStateInfo(0).IsName("Jump")) return;

                StopAgent();
                SetFace(faces.jumpFace);
                animator.SetTrigger("Jump");

                //Debug.Log("Jumping");
                break;

            case SlimeAnimationState.Attack:

                if (animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
                {
                    currentState = SlimeAnimationState.Walk;
                    walkType = WalkType.Chassing;
                    return;
                }
                StopAgent();
                SetFace(faces.attackFace);
                previousWalkType = walkType; // Save the previous walk type
                DealDamageToPlayer();
                animator.SetTrigger("Attack");
                break;

            case SlimeAnimationState.Damage:

                // Do nothing when animtion is playing
                if (animator.GetCurrentAnimatorStateInfo(0).IsName("Damage0")
                     || animator.GetCurrentAnimatorStateInfo(0).IsName("Damage1")
                     || animator.GetCurrentAnimatorStateInfo(0).IsName("Damage2")) return;

                // StopAgent();
                animator.SetTrigger("Damage");
                animator.SetInteger("DamageType", damType);
                SetFace(faces.damageFace);

                //Debug.Log("Take Damage");
                break;

        }

    }


    private void StopAgent()
    {
        agent.isStopped = true;
        animator.SetFloat("Speed", 0);
        agent.updateRotation = false;
    }
    // Animation Event
    public void AlertObservers(string message)
    {

        if (message.Equals("AnimationDamageEnded"))
        {
            // When Animation ended check distance between current position and first position 
            //if it > 1 AI will back to first position 

            if (message.Equals("AnimationDamageEnded"))
            {
                // หลังจากอนิเมชันการโดนตีจบลง ให้ Slime กลับไปเดินสุ่มต่อ
                walkType = WalkType.ExploreRandom;
                currentState = SlimeAnimationState.Walk;
            }
            else currentState = SlimeAnimationState.Idle;

            //Debug.Log("DamageAnimationEnded");
        }

        if (message.Equals("AnimationAttackEnded"))
        {
            if (!playerDetected)
            {
                walkType = previousWalkType;  // Restore the previous behavior
            }
            currentState = SlimeAnimationState.Walk;
        }

        if (message.Equals("AnimationJumpEnded"))
        {
            currentState = SlimeAnimationState.Idle;
        }
    }

    void OnAnimatorMove()
    {
        // apply root motion to AI
        Vector3 position = animator.rootPosition;
        position.y = agent.nextPosition.y;
        transform.position = position;
        agent.nextPosition = transform.position;
    }

    bool RandomPoint(Vector3 center, float range, float minDistance, out Vector3 result)
    {
        // ลองสุ่มจุดใน Sphere และตรวจสอบว่ามันอยู่ห่างจาก center อย่างน้อย minDistance
        for (int i = 0; i < 30; i++) // จำกัดการสุ่มไม่เกิน 30 ครั้ง เพื่อป้องกัน infinite loop
        {
            Vector3 randomPoint = center + Random.insideUnitSphere * range; // random point in a sphere 
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas))
            {
                // ตรวจสอบว่าจุดที่สุ่มมาอยู่ห่างจากตำแหน่งปัจจุบันมากกว่า minDistance หรือไม่
                if (Vector3.Distance(center, hit.position) >= minDistance)
                {
                    result = hit.position;
                    return true;
                }
            }
        }

        // ถ้าไม่สามารถสุ่มจุดที่อยู่ห่างพอได้หลังจาก 30 ครั้ง ให้ส่งค่า Vector3.zero กลับไป
        result = Vector3.zero;
        return false;
    }
    public void SlimeTakeDamage(float damageAmount)
    {
        // ลดค่า HP ของ Slime
        slimeHealth -= damageAmount;

        // แสดงอนิเมชันการโดนตี
        animator.SetTrigger("Damage");
        animator.SetInteger("DamageType", 1);

        // ตรวจสอบว่า Slime ตายหรือไม่
        if (slimeHealth <= 0)
        {
            Die();
        }
        else
        {
            // รีเซ็ตการเคลื่อนไหวหลังจากการโจมตีจบ
            walkType = WalkType.ExploreRandom;
            currentState = SlimeAnimationState.Walk;
        }
    }


    void Die()
    {
        // กำหนดให้ Slime ตาย
        Destroy(gameObject);  // ทำลาย GameObject เมื่อ Slime ตาย
    }
    IEnumerator FindTargetsWithDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            FindVisibleTargets();  // ค้นหาวัตถุที่มองเห็น
        }
    }

    void FindVisibleTargets()
    {
        visibleTargets.Clear();  // ล้างรายการวัตถุที่มองเห็น

        // ตรวจสอบสถานะ ถ้าเป็น Attacking ให้ใช้ attackingViewRadius และมุมมอง 360 องศา
        float currentViewRadius = (currentState == SlimeAnimationState.Attack) ? attackingViewRadius : viewRadius;
        float currentViewAngle = (currentState == SlimeAnimationState.Attack) ? 360f : viewAngle;

        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, currentViewRadius, targetMask);
        playerDetected = false;

        foreach (Collider target in targetsInViewRadius)
        {
            Transform targetTF = target.transform;
            Vector3 dirToTarget = (targetTF.position - transform.position).normalized;

            // ตรวจสอบว่าตัวละครอยู่ในมุมมองหรือไม่
            if (Vector3.Angle(transform.forward, dirToTarget) < currentViewAngle / 2)
            {
                float distanceToTarget = Vector3.Distance(transform.position, targetTF.position);

                // ตรวจสอบว่ามีสิ่งกีดขวางหรือไม่
                if (!Physics.Raycast(transform.position, dirToTarget, distanceToTarget, obstacleMask))
                {
                    visibleTargets.Add(targetTF);  // ถ้าไม่มีสิ่งกีดขวาง ถือว่ามองเห็น
                    if (target.CompareTag("Player"))
                    {
                        playerDetected = true;
                        targetTransform = targetTF;
                        currentState = SlimeAnimationState.Walk;
                        walkType = WalkType.Chassing;
                        // Debug.Log("check");
                    }
                }
            }
        }

        // ถ้าไม่เจอ Player และอยู่ในโหมด Attacking ให้กลับไปโหมด Patrol
        if (!playerDetected)
        {
            walkType = WalkType.ExploreRandom;
        }
    }
    public void DealDamageToPlayer()
    {
        if (!canAttack) return; // Skip if attack is in cooldown
        
        canAttack = false; // Prevent consecutive attacks
        Vector3 boxHalfExtents = new Vector3(0.5f, 0.5f, 0.5f);
        RaycastHit[] hits = Physics.BoxCastAll(transform.position, boxHalfExtents, transform.forward, Quaternion.identity, 1.0f);
        if (hits.Length > 0)
        {
            foreach (RaycastHit hit in hits)
            {
                if (hit.collider.CompareTag("Player"))
                {
                    PlayerControl player = hit.collider.GetComponent<PlayerControl>();
                    if (player != null)
                    {
                        player.PlayerTakeDamage(AttackDamage);
                    }
                }
            }
        }
        StartCoroutine(AttackCooldownCoroutine());  // Start cooldown coroutine
    }

    private IEnumerator AttackCooldownCoroutine()
    {
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }
}
