using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CharacterState {
    Idle,   // ตัวละครอยู่เฉยๆ
    Walking, // ตัวละครกำลังเดิน
    Running, // ตัวละครกำลังวิ่ง
    Attacking, // ตัวละครกำลังโจมตี
    Dead    // ตัวละครตาย
}

public class PlayerProperty : MonoBehaviour
{
    CharacterController cc;
    public Transform cameraTransform;
    float pitch = 0f;
    // Start is called before the first frame update

    [SerializeField] private float MaxHealth = 100;
    public float Health = 100;
    public float Shield = 0;
    public float Speed = 0;
    // [SerializeField]
    [SerializeField] private CharacterState State = CharacterState.Idle;
    // Start is called before the first frame update
    public float turnSmoothTime = 0.1f;
    private float turnSmoothVelocity;  // Smooth rotation velocity

    void Start()
    {
        cc = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
    }
    // Update is called once per frame
    void Update()
    {
        MovePlayer();
    }
    void MovePlayer()
    {
    // รับค่าจากคีย์บอร์ด (WASD หรือปุ่มลูกศร)
    Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
    move = Vector3.ClampMagnitude(move, 1f);  // จำกัดความเร็วการเคลื่อนที่

    // แปลงทิศทางการเคลื่อนที่ให้สัมพันธ์กับกล้อง
        if (move.magnitude >= 0.1f)
        {
            // คำนวณมุมที่จะหมุนตามกล้อง
            float targetAngle = Mathf.Atan2(move.x, move.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0, angle, 0);

            // คำนวณทิศทางการเคลื่อนที่หลังหมุนตามมุมกล้อง
            Vector3 forward = cameraTransform.forward;
            Vector3 right = cameraTransform.right;

            // ทำให้การเคลื่อนที่สัมพันธ์กับกล้อง โดยกำจัดค่า Y ให้อยู่ในระนาบเดียวกัน
            forward.y = 0;
            right.y = 0;

            forward.Normalize();
            right.Normalize();

            // สร้างทิศทางการเคลื่อนที่ตามมุมมองกล้อง
            Vector3 moveDirection = forward * move.z + right * move.x;
            cc.Move(moveDirection.normalized * Speed * Time.deltaTime);  // เคลื่อนที่ตัวละคร
        }
    }
}
