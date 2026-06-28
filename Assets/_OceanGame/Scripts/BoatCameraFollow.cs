using UnityEngine;
using UnityEngine.InputSystem;

public class BoatCameraFollow : MonoBehaviour
{
    [Header("Follow Settings")]
    [SerializeField] private Transform target; // The boat
    [SerializeField] private float distance = 8f;
    [SerializeField] private float height = 4f;
    [SerializeField] private float smoothSpeed = 5f;
    [SerializeField] private float minHeightAboveWater = 1.5f;

    [Header("Orbit Settings")]
    [SerializeField] private float mouseSensitivity = 0.15f;
    [SerializeField] private float minPitch = -10f;
    [SerializeField] private float maxPitch = 60f;

    private float currentYaw = 0f;
    private float currentPitch = 15f;
    private Waves oceanWaves;

    private void Start()
    {
        oceanWaves = FindFirstObjectByType<Waves>();
        if (target == null)
        {
            GameObject boat = GameObject.Find("Boat");
            if (boat != null)
            {
                target = boat.transform;
            }
        }

        // Initialize camera yaw to align with boat's initial forward direction
        if (target != null)
        {
            currentYaw = target.eulerAngles.y;
        }

        // Lock cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void LateUpdate()
    {
        if (target == null) return;

        // Process mouse orbit inputs
        if (Mouse.current != null)
        {
            Vector2 mouseDelta = Mouse.current.delta.ReadValue();
            currentYaw += mouseDelta.x * mouseSensitivity;
            currentPitch -= mouseDelta.y * mouseSensitivity;
            currentPitch = Mathf.Clamp(currentPitch, minPitch, maxPitch);
        }

        // Calculate rotation and target position
        Quaternion rotation = Quaternion.Euler(currentPitch, currentYaw, 0);
        Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
        Vector3 targetPos = rotation * negDistance + target.position;
        targetPos.y += height;

        // Prevent camera from going underwater
        if (oceanWaves == null)
        {
            oceanWaves = FindFirstObjectByType<Waves>();
        }

        if (oceanWaves != null)
        {
            float waterHeight = oceanWaves.GetHeight(targetPos);
            if (targetPos.y < waterHeight + minHeightAboveWater)
            {
                targetPos.y = waterHeight + minHeightAboveWater;
            }
        }
        else
        {
            // Default water level fallback
            if (targetPos.y < minHeightAboveWater)
            {
                targetPos.y = minHeightAboveWater;
            }
        }

        // Smoothly move position and rotate to look at target
        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * smoothSpeed);
        
        // Look slightly above target pivot to frame the boat nicely
        Vector3 lookAtTarget = target.position + Vector3.up * 1f;
        transform.rotation = Quaternion.LookRotation(lookAtTarget - transform.position);
    }
}