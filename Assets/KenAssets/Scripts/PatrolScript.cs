using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI; //important

//if you use this code you are contractually obligated to like the YT video
public class PatrolScript : MonoBehaviour //don't forget to change the script name if you haven't
{
    private NavMeshAgent agent;
    private EnemyProperty bhv;
    private VisionSystem vision;
    public float range; //radius of sphere

    public Transform centrePoint; //centre of the area the agent wants to move around in
    //instead of centrePoint you can set it as the transform of the agent if you don't care about a specific area
    private bool isWaiting = false;
    private EnemyState previousState;
    private EnemyState currentState;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        bhv = GetComponent<EnemyProperty>();
        vision = GetComponent<VisionSystem>();

    }


    void Update()
    {
        EnemyState currentState = bhv.getState(); // ตรวจสอบสถานะปัจจุบัน
        if (previousState == EnemyState.Attacking && currentState == EnemyState.Patrol)
        {
            agent.ResetPath(); // ยกเลิกการติดตามเป้าหมาย
        }

        if (currentState == EnemyState.Patrol)
        {
            if (agent.remainingDistance <= agent.stoppingDistance) // ตรวจสอบว่าตัวละครถึงจุดหมายแล้วหรือยัง
            {
                Vector3 point;
                if (RandomPoint(centrePoint.position, range, out point)) //pass in our centre point and radius of area
                {
                    Debug.DrawRay(point, Vector3.up, Color.blue, 1.0f); //so you can see with gizmos
                    agent.SetDestination(point); // ตั้งค่าจุดหมายใหม่
                }
            }
        }
        previousState = currentState; // เก็บสถานะปัจจุบันเป็นสถานะก่อนหน้า
    }

    bool RandomPoint(Vector3 center, float range, out Vector3 result)
    {
        Vector3 randomPoint = center + Random.insideUnitSphere * range; //random point in a sphere 
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas))
        {
            result = hit.position;
            return true;
        }

        result = Vector3.zero;
        return false;
    }
}