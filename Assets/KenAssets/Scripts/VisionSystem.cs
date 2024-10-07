using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class VisionSystem : MonoBehaviour
{
    public float viewRadius = 10f;  // ระยะการมองเห็น
    public float attackingViewRadius = 15f;
    [Range(0, 360)] public float viewAngle = 120f;  // มุมมอง (FOV)
    public LayerMask targetMask;    // เลเยอร์ของวัตถุที่ต้องการตรวจสอบ
    public LayerMask obstacleMask;  // เลเยอร์ของวัตถุที่เป็นสิ่งกีดขวาง

    public List<Transform> visibleTargets = new List<Transform>(); // วัตถุที่มองเห็นได้
    private EnemyProperty bhv;

    [HideInInspector]
    public Transform targetTransform;

    [HideInInspector]
    public bool playerDetected;


    void Start()
    {
        bhv = GetComponent<EnemyProperty>();
        StartCoroutine("FindTargetsWithDelay", 0.2f); // ทำการค้นหาทุกๆ 0.2 วินาที
    }

    void Update()
    {

    }

    IEnumerator FindTargetsWithDelay(float delay)
    {
        while (true)
        {
            Debug.Log("Searching for targets...");
            yield return new WaitForSeconds(delay);
            FindVisibleTargets();  // ค้นหาวัตถุที่มองเห็น
        }
    }

    void FindVisibleTargets()
    {
        visibleTargets.Clear();  // ล้างรายการวัตถุที่มองเห็น

        // ตรวจสอบสถานะ ถ้าเป็น Attacking ให้ใช้ attackingViewRadius และมุมมอง 360 องศา
        float currentViewRadius = (bhv.getState() == EnemyState.Attacking) ? attackingViewRadius : viewRadius;
        float currentViewAngle = (bhv.getState() == EnemyState.Attacking) ? 360f : viewAngle;  // เปลี่ยนมุมมองเป็น 360 องศาในโหมด Attacking

        // ค้นหาวัตถุที่อยู่ในระยะการมองเห็น
        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, currentViewRadius, targetMask);

        playerDetected = false;

        foreach (Collider target in targetsInViewRadius)
        {
            Transform targetTF = target.transform;
            Vector3 dirToTarget = (targetTF.position - transform.position).normalized;

            // ตรวจสอบว่าตัวละครอยู่ในมุมมองหรือไม่ (ถ้าเป็น 360 องศา จะมองเห็นได้รอบตัว)
            if (Vector3.Angle(transform.forward, dirToTarget) < currentViewAngle / 2)
            {
                float distanceToTarget = Vector3.Distance(transform.position, targetTF.position);

                // ยิง Raycast เพื่อเช็คว่าวัตถุถูกบดบังโดยสิ่งกีดขวางหรือไม่
                if (!Physics.Raycast(transform.position, dirToTarget, distanceToTarget, obstacleMask))
                {
                    visibleTargets.Add(targetTF); // ถ้าไม่ถูกบดบัง ถือว่ามองเห็น
                                                  // ตรวจสอบว่าคือ Player หรือไม่
                    if (target.CompareTag("Player"))
                    {
                        playerDetected = true;
                        bhv.setState(EnemyState.Attacking);
                        targetTransform = targetTF;
                    } // ตรวจจับ Player
                }
            }
        }

        // ถ้าอยู่ในโหมด Attacking แต่ไม่เจอ Player ให้กลับไปโหมด Patrol
        if (bhv.getState() == EnemyState.Attacking && !playerDetected)
        {
            bhv.setState(EnemyState.Patrol); // กลับไปโหมด Patrol
        }
    }

    // ฟังก์ชันช่วยในการวาดมุมมองใน Scene
    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    // ใช้แสดงผลใน Editor ว่ามุมมองเป็นอย่างไร
    void OnDrawGizmos()
    {
        // ตรวจสอบสถานะ ถ้าเป็น Attacking ให้ใช้ attackingViewRadius และมุมมอง 360 องศา
        float currentViewRadius = (bhv.getState() == EnemyState.Attacking) ? attackingViewRadius : viewRadius;
        float currentViewAngle = (bhv.getState() == EnemyState.Attacking) ? 360f : viewAngle;  // มุมมอง 360 องศาในโหมด Attacking

        Gizmos.color = (bhv.getState() == EnemyState.Attacking) ? Color.red : Color.blue;

        if (currentViewAngle < 360f)
        {
            Gizmos.color = Color.yellow;
            int segments = 50; // จำนวน segment ที่จะใช้ในการวาดขอบกรวย (จำนวน segment มากขึ้นจะทำให้กรวยเรียบขึ้น)

            Vector3 lastPoint = Vector3.zero;
            for (int i = 0; i <= segments; i++)
            {
                // คำนวณมุมแต่ละ segment สำหรับกรวยมุมมอง
                float angle = (-currentViewAngle / 2) + (currentViewAngle / (float)segments) * i;
                Vector3 direction = DirFromAngle(angle, false);

                // หากไม่ใช่จุดแรก ให้เชื่อมต่อเส้นระหว่างจุด
                if (i > 0)
                {
                    Gizmos.DrawLine(transform.position + lastPoint * currentViewRadius, transform.position + direction * currentViewRadius);
                }

                // เก็บจุดสุดท้ายสำหรับใช้ในรอบถัดไป
                lastPoint = direction;
            }

            // เชื่อมต่อเส้นจากจุดสุดท้ายกลับไปยังขอบด้านตรงข้าม
            // Gizmos.DrawLine(transform.position + lastPoint * currentViewRadius, transform.position + DirFromAngle(-currentViewAngle / 2, false) * currentViewRadius);

            // วาดเส้นขอบของกรวยที่เชื่อมจากจุดศูนย์กลางไปขอบกรวยทางซ้ายและขวา
            Gizmos.DrawLine(transform.position, transform.position + DirFromAngle(-currentViewAngle / 2, false) * currentViewRadius); // ขอบซ้าย
            Gizmos.DrawLine(transform.position, transform.position + DirFromAngle(currentViewAngle / 2, false) * currentViewRadius);  // ขอบขวา
        }

        else
        {
            // Gizmos.DrawWireSphere(transform.position, currentViewRadius);
            Gizmos.color = Color.yellow;
            int segments = 50; // จำนวน segment ที่ใช้ในการวาดวงกลม (จำนวน segment มากขึ้นจะทำให้วงกลมเรียบขึ้น)
            // วาดเส้นโค้งเป็นวงกลม 360 องศา
            Vector3 lastPoint = Vector3.zero;
            for (int i = 0; i <= segments; i++)
            {
                // คำนวณมุมแต่ละ segment
                float angle = (i / (float)segments) * 360f;
                Vector3 direction = DirFromAngle(angle, false);

                // หากไม่ใช่จุดแรก ให้เชื่อมต่อเส้นระหว่างจุด
                if (i > 0)
                    Gizmos.DrawLine(transform.position + lastPoint * currentViewRadius, transform.position + direction * currentViewRadius);
                // เก็บจุดสุดท้ายสำหรับใช้ในรอบถัดไป
                lastPoint = direction;
            }
            // เชื่อมต่อเส้นจากจุดสุดท้ายกลับไปจุดแรก
            Gizmos.DrawLine(transform.position + lastPoint * currentViewRadius, transform.position + DirFromAngle(0, false) * currentViewRadius);
        }

        // วาดเส้นไปยังวัตถุที่มองเห็นได้ (เช่น Player)
        Gizmos.color = Color.red;
        foreach (Transform visibleTarget in visibleTargets)
        {
            Gizmos.DrawLine(transform.position, visibleTarget.position);
        }
    }
}