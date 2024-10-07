
using UnityEngine;
using UnityEngine.AI;

// SlimeAnimationState { Idle,Walk,Jump,Attack,Damage}

public class SlimeAi : MonoBehaviour
{
    public Face faces;
    public GameObject SmileBody;
    public SlimeAnimationState currentState;

    public Animator animator;
    public NavMeshAgent agent;
    public Transform[] waypoints;
    public int damType;

    private bool move;
    private Material faceMaterial;
    private Vector3 originPos;

    public enum WalkType { Patroll, ToOrigin, ExploreRandom }
    private WalkType walkType;

    void Start()
    {
        originPos = transform.position;
        faceMaterial = SmileBody.GetComponent<Renderer>().materials[1];
        walkType = WalkType.ExploreRandom;
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

                if (animator.GetCurrentAnimatorStateInfo(0).IsName("Attack")) return;
                StopAgent();
                SetFace(faces.attackFace);
                animator.SetTrigger("Attack");

                // Debug.Log("Attacking");

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

            float distanceOrg = Vector3.Distance(transform.position, originPos);
            if (distanceOrg > 1f)
            {
                walkType = WalkType.ToOrigin;
                currentState = SlimeAnimationState.Walk;
            }
            else currentState = SlimeAnimationState.Idle;

            //Debug.Log("DamageAnimationEnded");
        }

        if (message.Equals("AnimationAttackEnded"))
        {
            currentState = SlimeAnimationState.Idle;
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
}
