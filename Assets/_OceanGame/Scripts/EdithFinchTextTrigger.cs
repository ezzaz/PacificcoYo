using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class EdithFinchTextTrigger : MonoBehaviour
{
    [Header("Text Content")]
    [TextArea(2, 5)] public string textMessage = "Inserta tu texto aquí...";

    [Header("Text Configuration")]
    public bool followCamera = false;
    public Transform textAnchor; // If assigned, text spawns here. If not, uses offset.
    public Vector3 textSpawnOffset = new Vector3(0f, 2.5f, 0f); 
    public float displayDuration = 4.5f;
    public float typingSpeed = 0.04f;

    [Header("Trigger Settings")]
    public bool triggerOnlyOnce = true;
    
    private bool hasTriggered = false;

    private void Awake()
    {
        BoxCollider bc = GetComponent<BoxCollider>();
        bc.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasTriggered && triggerOnlyOnce) return;

        if (other.CompareTag("Player"))
        {
            SpawnFloatingText();
            hasTriggered = true;
        }
    }

    public void SpawnFloatingText()
    {
        GameObject textGo = new GameObject("EdithFinch_Text3D");
        
        if (followCamera)
        {
            textGo.transform.position = transform.position; 
        }
        else
        {
            // If anchor exists, use it. Otherwise use offset.
            if (textAnchor != null)
            {
                textGo.transform.position = textAnchor.position;
                textGo.transform.rotation = textAnchor.rotation;
            }
            else
            {
                textGo.transform.position = transform.position + textSpawnOffset;
            }
        }

        FloatingText3D ft = textGo.AddComponent<FloatingText3D>();
        ft.textToShow = textMessage;
        ft.followCamera = followCamera;
        ft.displayDuration = displayDuration;
        ft.typingSpeed = typingSpeed;
        
        // If it's a world-space text (not following cam), don't look at cam if anchor is set? 
        // No, keep lookAtCamera = true by default as in FloatingText3D.
    }
}