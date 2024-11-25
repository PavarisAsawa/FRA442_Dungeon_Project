using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    private const float Y_ANGLE_MIN = -50.0f;
    private const float Y_ANGLE_MAX = 50.0f;

    public Transform lookAt;
    public Transform camTransform;

    private Camera cam;

    [SerializeField]
    private float distance = 20.0f;
    [SerializeField]
    private LayerMask collisionLayers; // Layers to check for collisions
    [SerializeField]
    private float collisionBuffer = 0.5f; // Buffer to prevent clipping
    private float currentX = 0.0f;
    private float currentY = 0.0f;
    private float sensivityX = 4.0f;
    private float sensivityY = 1.0f;

    private void Start()
    {
        camTransform = transform;
        cam = Camera.main;

        // Lock and hide the cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        currentX += Input.GetAxis("Mouse X") * sensivityX;
        currentY -= Input.GetAxis("Mouse Y") * sensivityY;

        currentY = Mathf.Clamp(currentY, Y_ANGLE_MIN, Y_ANGLE_MAX);
        
        // Zoom in/out using scroll wheel
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            distance += 0.4f;
        }

        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            distance -= 0.4f;
        }
    }

    private void LateUpdate()
    {
        Vector3 dir = new Vector3(0, 0, -distance);
        Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);

        // Desired camera position based on rotation
        Vector3 desiredCameraPosition = lookAt.position + rotation * dir;

        // Raycast to check for collisions
        Vector3 targetPosition;
        RaycastHit hit;
        if (Physics.Raycast(lookAt.position, (desiredCameraPosition - lookAt.position).normalized, out hit, distance, collisionLayers))
        {
            // Adjust position to avoid clipping, with a buffer
            targetPosition = lookAt.position + (rotation * dir.normalized * (hit.distance - collisionBuffer));
        }
        else
        {
            // Use desired position if no obstacles
            targetPosition = desiredCameraPosition;
        }

        // Smoothly move the camera to the target position
        camTransform.position = Vector3.Lerp(camTransform.position, targetPosition, Time.deltaTime * 10f);

        // Make the camera look at the target
        camTransform.LookAt(lookAt.position);
    }
}
