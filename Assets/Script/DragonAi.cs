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
    public AudioClip DragonRoarSound;
    public GameObject DragonFireBall;
    [HideInInspector] public Boolean playerNear = false;
    private Boolean isDead = false;
    public AudioClip DragonDeadSound;
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
        Debug.Log(state);
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
        // เล่นเสียง Dragon Roar
        if (DragonRoarSound != null)
        {
            GameObject tempAudioSource = new GameObject("TempAudioSource"); // สร้าง GameObject ชั่วคราว
            tempAudioSource.transform.position = transform.position; // กำหนดตำแหน่ง
            AudioSource audioSource = tempAudioSource.AddComponent<AudioSource>(); // เพิ่ม AudioSource
            audioSource.clip = DragonRoarSound;
            audioSource.volume = 0.5f; // ระดับเสียง
            audioSource.Play();
            Destroy(tempAudioSource, DragonRoarSound.length); // ลบ GameObject หลังเสียงเล่นจบ
        }
    }

    public void DragonEat()
    {
        Vector3 box = new Vector3(4f, 4f, 4f);
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
        // if (state != DragonState.Init || state != DragonState.Sick || state != DragonState.Dead) Face2Player();
        if (!isDead) Face2Player();

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

    public void DragonTakeDamage(float damamge)
    {
        dragonHealth -= damamge;
        if (dragonHealth <= 0)
        {
            DragonDie();
        }
    }
    void DragonDie()
    {
        isDead = true;
        state = DragonState.Dead;
        animator.SetBool("Die", true);
        if (DragonDeadSound != null)
        {
            GameObject tempAudioSource = new GameObject("TempAudioSource2"); // สร้าง GameObject ชั่วคราว
            tempAudioSource.transform.position = transform.position; // กำหนดตำแหน่ง
            AudioSource audioSource = tempAudioSource.AddComponent<AudioSource>(); // เพิ่ม AudioSource
            audioSource.clip = DragonDeadSound;
            audioSource.volume = 0.5f; // ระดับเสียง
            audioSource.Play();
            Destroy(tempAudioSource, DragonDeadSound.length); // ลบ GameObject หลังเสียงเล่นจบ
        }
    }

    public void DragonFire()
    {
        if (DragonRoarEffect != null)
        {
            Vector3 spawnPosition = transform.position + transform.forward * 2f; // ด้านหน้ามังกร
            spawnPosition.y = 2f; // เพิ่มความสูงในแกน Y

            // สร้าง fireball ที่ตำแหน่งที่ปรับแล้ว
            GameObject fireball = Instantiate(DragonFireBall, spawnPosition, Quaternion.identity);
            fireball.transform.localScale = new Vector3(2f, 2f, 2f);
            // กำหนดให้ fireball มีความเร็ว
            Rigidbody rb = fireball.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 fireDirection = transform.forward; // ทิศทางการยิง (ไปข้างหน้า)
                float fireSpeed = 80f; // ความเร็ว
                rb.velocity = fireDirection * fireSpeed;
            }
            else
            {
                Debug.LogWarning("DragonFireBall is missing Rigidbody!");
            }
            // ทำลาย fireball หลังจาก 1 วินาที
            Destroy(fireball, 5f);
        }
    }

}
