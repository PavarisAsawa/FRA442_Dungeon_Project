using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonFireballScript : MonoBehaviour
{
    [SerializeField] private float FireBallDamage = 75f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // ตรวจสอบว่า Tag ของวัตถุที่ชนคือ "Player"
        {
            PlayerControl player = other.GetComponent<PlayerControl>();
            player.PlayerTakeDamage(FireBallDamage);
        }
    }
}
