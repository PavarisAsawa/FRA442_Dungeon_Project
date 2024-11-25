using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

public class LootScript : MonoBehaviour
{
    public enum LootType
    {
        Health,
        Ammo,
        maxHealth,
        Speed
    }

    public GameObject lootEffect;
    // Start is called before the first frame update
    private AudioSource lootSound;
    [Range(0f, 1f)] public float volume = 1f;
    [SerializeField] private LootType loot;
    [SerializeField] private int BuffValue;
    void Start()
    {
        lootSound = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnTriggerEnter(Collider other) // ใช้ OnTriggerEnter หากเป็น 3D
    {
        if (other.CompareTag("Player")) // ตรวจสอบว่า GameObject ที่ชนมี Tag เป็น "Player"
        {
            Debug.Log("Item collected!");
            GameObject audioPlayer = new GameObject("AudioPlayer");
            AudioSource audioSource = audioPlayer.AddComponent<AudioSource>();
            audioSource.clip = lootSound.clip;
            audioSource.volume = volume;
            audioSource.Play();

            Player player = other.GetComponent<Player>();

            switch (loot)
            {
                case LootType.Health:
                    Debug.Log("Health Loot Collected!");
                    // เพิ่ม Health ให้ Player
                    // PlayerHealth.Instance.AddHealth(10); // ตัวอย่างการเรียกฟังก์ชันเพิ่ม Health
                    if(player.playerHealth+BuffValue >= player.playerMaxHealth) player.playerHealth = player.playerMaxHealth;
                    else player.playerHealth += BuffValue;
                    break;

                case LootType.Ammo:
                    Debug.Log("Ammo Loot Collected!");
                    // เพิ่ม Ammo ให้ Player
                    // PlayerInventory.Instance.AddAmmo(20); // ตัวอย่างการเพิ่ม Ammo
                    player.AttackDamage += 10;
                    break;

                case LootType.maxHealth:
                    Debug.Log("maxHealth Loot Collected!");
                    // เพิ่ม Shield ให้ Player
                    // PlayerShield.Instance.AddShield(15); // ตัวอย่างการเพิ่ม Shield
                    player.playerMaxHealth += BuffValue;
                    // player.playerHealth = player.playerMaxHealth;
                    break;

                case LootType.Speed:
                    Debug.Log("Speed Loot Collected!");
                    // เพิ่ม Shield ให้ Player
                    // PlayerShield.Instance.AddShield(15); // ตัวอย่างการเพิ่ม Shield
                    player.runSpeed += 1f;
                    player.sprintSpeed += 1f;
                    break;

                default:
                    Debug.LogWarning("Unknown LootType!");
                    break;
            }
            PlayEffect();
            Destroy(audioPlayer, lootSound.clip.length); // ลบ GameObject หลังเสียงจบ
            Destroy(gameObject); // ทำลายไอเท็มทันที
        }
    }
    private void PlayEffect()
    {
        StartCoroutine("EffectLoop");
    }
    IEnumerator EffectLoop()
    {
        float loopTimeLimit = 0.7f;
        GameObject effectPlayer = (GameObject) Instantiate(lootEffect, transform.position, transform.rotation);
        yield return new WaitForSeconds(loopTimeLimit);
        Destroy (effectPlayer);
    }
}
