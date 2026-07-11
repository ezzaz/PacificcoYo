using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class Dialogos : MonoBehaviour
{
    [Header("UI Elements (Deactivated for 3D Text)")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private TMP_Text characterNameText;
    [SerializeField] private Image rightCharacterImage;
    [SerializeField] private Image leftCharacterImage;
    [SerializeField] private GameObject rightPanel;
    [SerializeField] private GameObject leftPanel;
    public AudioSource audioSource;
    public bool parteAbierto;
    public bool esCinematica;

    [Header("Dialogue Data")]
    public List<DialogueLine> dialogueLines = new List<DialogueLine>();

    private bool isPlayerInRange;
    private bool didDialogueStart;
    public int lineIndex;
    private float typingTime = 0.04f;

    // 3D floating text references
    private GameObject active3DTextObject;
    private TextMeshPro active3DText;
    private string simulatedText = ""; // Keeps dialogueText.text logic working in memory

    private void Start()
    {
        // Force hide 2D canvas UI panels so only 3D text is visible
        Hide2DPanels();

        if (parteAbierto)
        {
            StartDialogue();
        }
    }

    private void Hide2DPanels()
    {
        if (dialoguePanel != null) dialoguePanel.SetActive(false);
        if (rightPanel != null) rightPanel.SetActive(false);
        if (leftPanel != null) leftPanel.SetActive(false);
    }

    public bool IsDialogueActive()
    {
        return didDialogueStart;
    }

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            if (didDialogueStart)
            {
                // If the text is fully typed, advance. If not, autocomplete instantly.
                if (simulatedText == dialogueLines[lineIndex].text)
                {
                    NextDialogueLine();
                }
                else
                {
                    StopAllCoroutines();
                    simulatedText = dialogueLines[lineIndex].text;
                    if (active3DText != null)
                    {
                        active3DText.text = simulatedText;
                        active3DText.color = new Color(active3DText.color.r, active3DText.color.g, active3DText.color.b, 1f);
                    }
                }
            }
            else if (!esCinematica && isPlayerInRange)
            {
                StartDialogue();
            }
        }
    }

    public void StartDialogue()
    {
        didDialogueStart = true;
        Hide2DPanels();

        lineIndex = 0;
        StartCoroutine(ShowLine());
    }

    private void NextDialogueLine()
    {
        // Let the current text object float up and fade out on its own (handled by FloatingText3D)
        active3DTextObject = null;
        active3DText = null;

        lineIndex++;
        if (lineIndex < dialogueLines.Count)
        {
            StartCoroutine(ShowLine());
        }
        else
        {
            didDialogueStart = false;
        }
    }

    private IEnumerator ShowLine()
    {
        Hide2DPanels();

        // 1. Play dialogue sound
        if (audioSource != null && dialogueLines[lineIndex].nextDialogueSound != null)
        {
            audioSource.PlayOneShot(dialogueLines[lineIndex].nextDialogueSound);
        }

        // 2. Create a new Edith Finch-style 3D floating text object
        if (active3DTextObject != null)
        {
            // Abandon previous to let it float off
            active3DTextObject = null;
        }

        active3DTextObject = new GameObject("FloatingDialogue_3D");
        active3DText = active3DTextObject.AddComponent<TextMeshPro>();
        
        // Exquisite Edith Finch look & feel
        active3DText.alignment = TextAlignmentOptions.Center;
        active3DText.fontSize = 5.5f;
        active3DText.color = new Color(1f, 0.95f, 0.7f, 0f); // Soft vanilla
        active3DText.outlineWidth = 0.16f;
        active3DText.outlineColor = new Color(0f, 0f, 0f, 0.85f);

        // Add drifting physics
        FloatingText3D drift = active3DTextObject.AddComponent<FloatingText3D>();
        drift.textToShow = dialogueLines[lineIndex].text;
        drift.followCamera = true; // Floating floating right on player's side
        drift.displayDuration = 99f; // Keep alive until we manually destroy/advance it
        drift.typingSpeed = typingTime;
        drift.followOffset = new Vector3(0f, 0.3f, 3.2f); // Centered, slightly lower, beautiful position
        drift.driftDirection = new Vector3(0.02f, 0.1f, -0.02f); // very gentle float

        simulatedText = "";
        dialogueText.text = ""; // Keep references happy

        // Type character-by-character to mirror in our simulated text
        foreach (char ch in dialogueLines[lineIndex].text)
        {
            simulatedText += ch;
            dialogueText.text = simulatedText;
            yield return new WaitForSecondsRealtime(typingTime);
        }
    }

    // 3D triggers
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isPlayerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isPlayerInRange = false;
        }
    }
}