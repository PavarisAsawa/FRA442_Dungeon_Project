using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
    private Player player;  // อ้างถึง Player เพื่อใช้ค่าความเสียหายจากผู้เล่น
    private PlayerControl playerControl;

    private void Start()
    {
        // ตรวจสอบว่าตัวแปร playerControl ถูกกำหนดค่าหรือยัง ถ้ายังให้ค้นหาใน Scene
        if (playerControl == null)
        {
            playerControl = FindObjectOfType<PlayerControl>();  // ค้นหา PlayerControl อัตโนมัติ
            if (playerControl == null)
            {
                Debug.LogError("PlayerControl not found in the scene.");
            }
        }

        // ตรวจสอบว่าตัวแปร player ถูกกำหนดค่าหรือยัง ถ้ายังให้ค้นหาใน Scene
        if (player == null)
        {
            player = FindObjectOfType<Player>();  // ค้นหา Player อัตโนมัติ
            if (player == null)
            {
                Debug.LogError("Player not found in the scene.");
            }
        }
    }

    // ฟังก์ชันนี้จะถูกเรียกเมื่อดาบชนกับ Slime
    private void OnTriggerEnter(Collider other)
    {
        // ตรวจสอบว่าดาบชนกับ Slime และผู้เล่นกำลังโจมตีอยู่หรือไม่
        if (other.gameObject.CompareTag("Slime") && playerControl != null && playerControl.isAttacking)
        {
            // เข้าถึงสคริปต์ของ Slime แล้วเรียกฟังก์ชันเพื่อให้ Slime ได้รับความเสียหาย
            SlimeAi slime = other.gameObject.GetComponent<SlimeAi>();
            if (slime != null)
            {
                // ตรวจสอบว่าค่า player ไม่เป็น null ก่อนที่จะเข้าถึงค่าความเสียหาย
                if (player != null)
                {
                    // ส่งค่าความเสียหายจากผู้เล่นไปให้ Slime
                    slime.SlimeTakeDamage(player.AttactDamage);
                }
                else
                {
                    Debug.LogError("Player component is not assigned.");
                }
            }
        }
    }
}
