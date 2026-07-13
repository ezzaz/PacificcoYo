using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class TextTrigger : MonoBehaviour
{
    [Header("Text Content")]
    [TextArea(2, 5)] public string textMessage = "Funciona...";

    [Header("Text Configuration")]
    public Transform textAnchor; 
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
        GameObject textGo = new GameObject("Text3D");
       
        
            if (textAnchor != null)
            {
                textGo.transform.position = textAnchor.position;
                textGo.transform.rotation = textAnchor.rotation;
            }
          
        

        FloatingText3D ft = textGo.AddComponent<FloatingText3D>();
        ft.textToShow = textMessage;
        ft.displayDuration = displayDuration;
        ft.typingSpeed = typingSpeed;
        
       
    }
}