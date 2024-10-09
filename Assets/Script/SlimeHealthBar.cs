using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlimeHealthBar : MonoBehaviour
{
    public Slider healthBarSlider;  // อ้างถึง UI Health Bar
    public Transform slime;         // ตัวละคร Slime
    public Vector3 offset;          // การปรับตำแหน่งของ Health Bar จาก Slime
    public Camera mainCamera;       // กล้องหลัก

    private SlimeAi slimeAi;        // อ้างถึงสคริปต์ SlimeAi เพื่อดึงข้อมูลสุขภาพ

    void Start()
    {
        slimeAi = slime.GetComponent<SlimeAi>();
        if (slimeAi == null)
        {
            Debug.LogError("SlimeAi script not found on the slime!");
        }

        // หา Camera หลักถ้าไม่ได้เซ็ตไว้
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        // กำหนดค่าความจุสูงสุดของ Health Bar ตามค่าพลังชีวิตสูงสุด
        healthBarSlider.maxValue = slimeAi.maxHealth;
    }

    void Update()
    {
        // ติดตามตำแหน่งของ Slime โดยบวก offset เพื่อให้แถบสุขภาพลอยเหนือ Slime
        Vector3 targetPosition = slime.position + offset;

        // อัปเดตตำแหน่งของ Health Bar แบบลื่นไหล
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 10f);

        // ทำให้ Health Bar หันเข้าหากล้องเสมอ
        transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.forward, mainCamera.transform.rotation * Vector3.up);

        // ปรับค่าของ Slider ตามสุขภาพของ Slime
        healthBarSlider.value = slimeAi.slimeHealth;  // ค่า Health Bar อัปเดตตามพลังชีวิตปัจจุบัน
    }
}
