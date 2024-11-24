using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class DragonAi : MonoBehaviour
{
    public enum DragonState { Init, Idle, Fly, Sick, Death, Eat, Dead }
    public float maxHealth = 500f;
    public float dragonHealth = 500f;
    private float AttackDamage = 75;
    // Start is called before the first frame update
    private Animator animator;

    private GameObject player;    // เลเยอร์ของวัตถุที่ต้องการตรวจสอบ
    // [SerializeField] private int AnimationInput = 0; 
    private DragonState state = DragonState.Init;
    public GameObject DragonRoarEffect;
    [HideInInspector] public Boolean playerNear = false;
    void Start()
    {
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player");
        // ตรวจสอบว่าพบ GameObject หรือไม่
        if (player != null) Debug.Log("Player found: " + player.name);
        else Debug.LogWarning("Player not found! Make sure the Player object has the correct Tag.");
    }

    // Update is called once per frame
    void Update()
    {
        CheckPlayer();

    }

    void Face2Player()
    {
        if (player != null) // ตรวจสอบว่า Player ไม่เป็น null
        {
            Vector3 direction = player.transform.position - transform.position;
            direction.y = 0; // ล็อคแกน Y เพื่อป้องกันการหันขึ้น/ลง
            direction.Normalize();
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
            Vector3 forward = transform.forward; // ทิศทางที่ตัวละครกำลังหันหน้า
            float dotProduct = Vector3.Dot(forward, direction);
        }
    }
    public void setState(DragonState set)
    {
        state = set;
    }
    public DragonState getState()
    {
        return state;
    }

    public void DragonRoar()
    {
        if (DragonRoarEffect != null)
        {
            GameObject roar = Instantiate(DragonRoarEffect, transform.position, Quaternion.identity);
            Destroy(roar, 1.5f);
        }
    }

    public void DragonEat()
    {
        Vector3 box = new Vector3(2f, 2f, 3f);
        RaycastHit[] hits = Physics.BoxCastAll(transform.position, box, transform.forward, Quaternion.identity, 3f);
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
    }

    void CheckPlayer()
    {
        if (state != DragonState.Init || state != DragonState.Sick || state != DragonState.Dead) Face2Player();

        playerNear = false; // ตั้งค่าเริ่มต้นเป็น false
        float detectionRadius = 6f; // ระยะที่ต้องการตรวจจับ
        Collider[] nearbyObjects = Physics.OverlapSphere(transform.position, detectionRadius); // ตรวจจับวัตถุในระยะ
        foreach (Collider collider in nearbyObjects)
        {
            if (collider.CompareTag("Player"))
            {
                playerNear = true;
                break; // ออกจากลูปเมื่อเจอ Player
            }
        }
    }

}
