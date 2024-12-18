using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float playerMaxHealth = 100f;
    public float playerHealth = 100f;
    public float AttackDamage = 20f;
    public float runSpeed = 10;
    public float sprintSpeed = 15;

    public void AdjustMaxHealth(float newMaxHealth)
    {
        playerMaxHealth = newMaxHealth;

        // Ensure current health doesn't exceed the new max
        if (playerHealth > playerMaxHealth)
        {
            playerHealth = playerMaxHealth;
        }
    }
     public void DealDamageToSlime()
    {
        // ค้นหา Slime ที่ Player โจมตี (ใช้วิธีอ้างอิงจากการชนหรืออื่นๆ)
        // ตัวอย่างนี้เป็นแบบเรียกจากการชนที่ดาบของ Player ทำกับ Slime
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 2.0f))
        {
            if (hit.collider.CompareTag("Slime"))
            {
                SlimeAi slime = hit.collider.GetComponent<SlimeAi>();
                if (slime != null)
                {
                    // ลด HP ของ Slime
                    slime.SlimeTakeDamage(AttackDamage);
                }
            }
        }
    }
    
}
