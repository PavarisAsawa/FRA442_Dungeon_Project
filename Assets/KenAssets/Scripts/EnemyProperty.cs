using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyState {
    Idle,   // ตัวละครอยู่เฉยๆ
    Patrol, // ตัวละครกำลังเดิน
    Attacking,  // ตัวละครกำลังโจมตี
    Dead    // ตัวละครตาย
}
public class EnemyProperty : MonoBehaviour
{
    [SerializeField] private EnemyState State;
    // // Start is called before the first frame update
    void Awake() 
    {

    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }



    public EnemyState getState()
    {
        return State;
    }

    public void setState(EnemyState InputState)
    {
        State = InputState;
        Debug.Log("Enemy state changed to: " + State);
    }
}
