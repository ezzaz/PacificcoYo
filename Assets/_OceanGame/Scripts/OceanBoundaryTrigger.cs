using UnityEngine;

public class OceanBoundaryTrigger : MonoBehaviour
{
    [Header("Boundary Settings")]
    public float maxDistance = 220f; // radius of allowable ocean space from boat start
    public Vector3 centerPosition = new Vector3(170f, 0f, 363f); // Starting area of Boat

    [Header("Warning Settings")]
    public float warningCooldown = 6f;

    private float lastWarningTime = -99f;
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        // Capture initial boat position as center if center is close to zero
        if (centerPosition == Vector3.zero)
        {
            centerPosition = new Vector3(transform.position.x, 0f, transform.position.z);
        }
    }

    private void Update()
    {
        Vector3 boatPos2D = new Vector3(transform.position.x, 0f, transform.position.z);
        Vector3 centerPos2D = new Vector3(centerPosition.x, 0f, centerPosition.z);

        float distance = Vector3.Distance(boatPos2D, centerPos2D);
        if (distance > maxDistance)
        {
            // 1. Physically nudge the boat back towards center
            Vector3 pushDirection = (centerPos2D - boatPos2D).normalized;
            if (rb != null)
            {
                // Force a reversal of velocity
                rb.AddForce(pushDirection * 15f, ForceMode.VelocityChange);
                rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, Vector3.zero, Time.deltaTime * 6f);
            }

            // 2. Trigger Edith Finch style floating warning
            if (Time.time - lastWarningTime > warningCooldown)
            {
                lastWarningTime = Time.time;
                SpawnBoundaryWarning();
            }
        }
    }

    private void SpawnBoundaryWarning()
    {
        GameObject textGo = new GameObject("Boundary_Warning_3D");
        FloatingText3D ft = textGo.AddComponent<FloatingText3D>();
        ft.textToShow = "Llegaste al borde del mar abierto.\nCarlos prefiere mantenerse cerca de las zonas de pesca.";
        ft.followCamera = true;
        ft.displayDuration = 4.0f;
    }
}