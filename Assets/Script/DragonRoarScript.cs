using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonRoarScript : MonoBehaviour
{
    public GameObject DragonRoarEffect; // Prefab สำหรับเอฟเฟกต์ Roar

    void Start()
    {

    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // ตรวจสอบว่า Tag ของวัตถุที่ชนคือ "Player"
        {
            PlayerControl player = other.GetComponent<PlayerControl>();
            player.PlayerTakeDamage(50f);
        }
    }
}
