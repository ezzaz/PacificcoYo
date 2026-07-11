using System.Collections;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshPro))]
public class FloatingText3D : MonoBehaviour
{
    [Header("Text Settings")]
    public string textToShow = "";
    public float typingSpeed = 0.04f;
    public float displayDuration = 4.0f;
    public bool lookAtCamera = true;

    [Header("Animation Settings")]
    public bool followCamera = false;
    public Vector3 followOffset = new Vector3(1.5f, 0.5f, 3.5f);
    public Vector3 driftDirection = new Vector3(0f, 0.15f, 0.02f);
    public float scaleSpeed = 1.2f;

    [Header("Ghostly Breeze Effect")]
    public bool dissolveOnCollision = true;
    public float dissolveSpeed = 2f;

    private TextMeshPro tmp;
    private Camera mainCam;
    private float elapsed = 0f;
    private bool isDissolving = false;
    private Vector3 currentVelocity = Vector3.zero;

    private void Awake()
    {
        tmp = GetComponent<TextMeshPro>();
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.fontSize = 5f;
        tmp.color = new Color(1f, 0.95f, 0.7f, 0f);
        tmp.outlineWidth = 0.15f;
        tmp.outlineColor = new Color(0f, 0f, 0f, 0.8f);

        // Setup trigger for collision dissolve
        SphereCollider sc = gameObject.AddComponent<SphereCollider>();
        sc.isTrigger = true;
        sc.radius = 1.5f; 
    }

    private void Start()
    {
        mainCam = Camera.main;
        if (mainCam == null) mainCam = FindFirstObjectByType<Camera>();

        if (followCamera && mainCam != null)
        {
            transform.position = mainCam.transform.TransformPoint(followOffset);
            transform.rotation = mainCam.transform.rotation;
        }

        StartCoroutine(AnimateTextRoutine());
    }

    private void Update()
    {
        if (mainCam == null || isDissolving) return;

        if (followCamera)
        {
            Vector3 targetPos = mainCam.transform.TransformPoint(followOffset) + (mainCam.transform.rotation * driftDirection * elapsed);
            transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * 3f);
            transform.rotation = Quaternion.Slerp(transform.rotation, mainCam.transform.rotation, Time.deltaTime * 4f);
        }
        else
        {
            transform.position += driftDirection * Time.deltaTime;

            if (lookAtCamera)
            {
                Vector3 directionToCam = mainCam.transform.position - transform.position;
                directionToCam.y = 0;
                if (directionToCam != Vector3.zero)
                {
                    transform.rotation = Quaternion.LookRotation(-directionToCam);
                }
            }
        }

        elapsed += Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (dissolveOnCollision && other.CompareTag("Player") && !isDissolving)
        {
            StartCoroutine(GhostlyBreezeDissolve());
        }
    }

    private IEnumerator GhostlyBreezeDissolve()
    {
        isDissolving = true;
        StopAllCoroutines();

        float fadeDuration = 1.0f;
        float timer = 0f;
        Vector3 initialPos = transform.position;
        Vector3 breezeDir = (transform.position - mainCam.transform.position).normalized + Vector3.up;
        Vector3 initialScale = transform.localScale;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float t = timer / fadeDuration;

            // Move as if blown by a ghostly breeze
            transform.position += breezeDir * (Time.deltaTime * dissolveSpeed);
            
            // Fade alpha
            tmp.color = new Color(tmp.color.r, tmp.color.g, tmp.color.b, Mathf.Lerp(1f, 0f, t));
            
            // Scale up and distort
            transform.localScale = initialScale * Mathf.Lerp(1f, 2.5f, t);

            yield return null;
        }

        Destroy(gameObject);
    }

    private IEnumerator AnimateTextRoutine()
    {
        tmp.text = "";
        string currentText = "";
        foreach (char ch in textToShow)
        {
            currentText += ch;
            tmp.text = currentText;
            tmp.color = new Color(tmp.color.r, tmp.color.g, tmp.color.b, Mathf.Min(tmp.color.a + 0.2f, 1f));
            yield return new WaitForSeconds(typingSpeed);
        }

        tmp.color = new Color(tmp.color.r, tmp.color.g, tmp.color.b, 1f);
        yield return new WaitForSeconds(displayDuration);

        if (!isDissolving)
        {
            StartCoroutine(GhostlyBreezeDissolve());
        }
    }
}