using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI; //important


public class AttackScript : MonoBehaviour
{
    private NavMeshAgent agent;
    private EnemyProperty bhv;
    private VisionSystem vision;
    // Start is called before the first frame update
    void Start()
    {
        bhv = GetComponent<EnemyProperty>();
        vision = GetComponent<VisionSystem>();
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        EnemyState currentState = bhv.getState(); // ตรวจสอบสถานะปัจจุบัน

        // ตรวจสอบว่าศัตรูอยู่ในสถานะ Attacking
        if (currentState == EnemyState.Attacking)
        {
            // ตรวจสอบว่ามีเป้าหมาย (targetTransform) มั้ย
            if (vision.targetTransform != null)
            {
                // สั่งให้ NavMeshAgent เคลื่อนไปยังตำแหน่งเป้าหมาย
                agent.SetDestination(vision.targetTransform.position);
            }
        }
    }
}
