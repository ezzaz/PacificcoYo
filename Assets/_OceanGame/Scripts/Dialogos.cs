using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class Dialogos : MonoBehaviour
{
    [Header("UI Elements")]
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
    private float typingTime = 0.05f;

    private void Start()
    {
        if (parteAbierto)
        {
            StartDialogue();
        }
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
                if (dialogueText.text == dialogueLines[lineIndex].text)
                {
                    NextDialogueLine();
                }
                else
                {
                    StopAllCoroutines();
                    dialogueText.text = dialogueLines[lineIndex].text;
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
        dialoguePanel.SetActive(true);

        lineIndex = 0;
        StartCoroutine(ShowLine());
    }

    private void NextDialogueLine()
    {
        lineIndex++;
        if (lineIndex < dialogueLines.Count)
        {
            StartCoroutine(ShowLine());
        }
        else
        {
            didDialogueStart = false;
            leftPanel.SetActive(false);
            rightPanel.SetActive(false);
            dialoguePanel.SetActive(false);
        }
    }

    private IEnumerator ShowLine()
    {
        // Set character name and image
        characterNameText.text = dialogueLines[lineIndex].characterName;

        if (dialogueLines[lineIndex].isRightSpeaker)
        {
            rightPanel.SetActive(true);
            if (audioSource != null && dialogueLines[lineIndex].nextDialogueSound != null)
            {
                audioSource.PlayOneShot(dialogueLines[lineIndex].nextDialogueSound);
            }
            leftPanel.SetActive(false);
            if (rightCharacterImage != null)
            {
                rightCharacterImage.sprite = dialogueLines[lineIndex].characterImage;
            }
        }
        else
        {
            leftPanel.SetActive(true);
            rightPanel.SetActive(false);
            if (leftCharacterImage != null)
            {
                leftCharacterImage.sprite = dialogueLines[lineIndex].characterImage;
            }
            if (audioSource != null && dialogueLines[lineIndex].nextDialogueSound != null)
            {
                audioSource.PlayOneShot(dialogueLines[lineIndex].nextDialogueSound);
            }
        }

        dialogueText.text = string.Empty;

        foreach (char ch in dialogueLines[lineIndex].text)
        {
            dialogueText.text += ch;
            yield return new WaitForSecondsRealtime(typingTime);
        }
    }

    // 3D triggers (for Minijuego 1 and 3)
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

    // 2D triggers (legacy / fallback)
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