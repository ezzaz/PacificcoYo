using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameplayCinematicController : MonoBehaviour
{
    public static GameplayCinematicController Instance;

    public enum LevelType
    {
        Minijuego1,
        Minijuego2,
    }

    [Header("Current Level Configuration")]
    public LevelType currentLevel;
    public float delayBeforeStart = 0.5f;

    [Header("Minijuego 2 Settings (Fishing)")]
    public int fishRequiredToCatch = 12;
    private int fishCaughtCount = 0;

   

    private Dialogos dialogosSystem;
    private PlayerMovement playerMove;
    private PlayerLook playerLook;
    private WaterBoat boatControls;

    // Camera Cinematic State
    private Camera mainCamera;
    private Transform cameraParent;
    private Vector3 originalLocalCamPos;
    private Quaternion originalLocalCamRot;

    [Header("Customizable Camera Sweeps (Empty GameObjects)")]
    public Transform startCinematicAnchor;
    public Transform endCinematicAnchor;

    private bool dialogueFinishedStarting = false;
    private bool isPlayingStartCinematic = false;
    private bool isPlayingEndCinematic = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        StartCoroutine(SetupLevelRoutine());
    }

    private IEnumerator SetupLevelRoutine()
    {
        yield return new WaitForSeconds(delayBeforeStart);

        // Find Dialogue System
        dialogosSystem = FindFirstObjectByType<Dialogos>();
        if (dialogosSystem == null)
        {
            // Try to find canvas or instantiate prefab if missing
            GameObject canvasPrefab = Resources.Load<GameObject>("Canvas");
            if (canvasPrefab != null)
            {
                GameObject canvasInst = Instantiate(canvasPrefab);

                dialogosSystem = canvasInst.GetComponentInChildren<Dialogos>();
            }
            else
            {
                // Fallback: try to find any existing Canvas in the scene
                Canvas existingCanvas = FindFirstObjectByType<Canvas>();
                if (existingCanvas != null)
                {
                    dialogosSystem = existingCanvas.GetComponentInChildren<Dialogos>();
                }
            }
        }

        // Find Player components
        GameObject playerObj = GameObject.Find("Player");
        if (playerObj != null)
        {
            playerMove = playerObj.GetComponent<PlayerMovement>();
            playerLook = playerObj.GetComponent<PlayerLook>();

            // Get original camera parent and transform
            mainCamera = playerObj.GetComponentInChildren<Camera>();
            if (mainCamera != null)
            {
                cameraParent = mainCamera.transform.parent;
                originalLocalCamPos = mainCamera.transform.localPosition;
                originalLocalCamRot = mainCamera.transform.localRotation;
            }
        }

        // Find Boat controls (for Minijuego 2 where player drives the boat)
        GameObject boatObj = GameObject.Find("Boat");
        if (boatObj != null)
        {
            boatControls = boatObj.GetComponent<WaterBoat>();
            if (mainCamera == null)
            {
                mainCamera = boatObj.GetComponentInChildren<Camera>();
                if (mainCamera != null)
                {
                    cameraParent = mainCamera.transform.parent;
                    originalLocalCamPos = mainCamera.transform.localPosition;
                    originalLocalCamRot = mainCamera.transform.localRotation;
                }
            }
        }

        // Play level introduction cinematic dialogues!
        PlayStartDialogue();
    }

    private void PlayStartDialogue()
    {
        if (dialogosSystem == null)
        {
            Debug.LogWarning("Dialogue system missing at startup. Direct level initialization.");
            isPlayingStartCinematic = false;
            EnableControls();
            OnStartCinematicFinished();
            return;
        }

        isPlayingStartCinematic = true;

        dialogosSystem.dialogueLines.Clear();

        if (currentLevel == LevelType.Minijuego1)
        {
            // Minijuego 1: sin limitaciones, el jugador puede moverse libremente
        }
        else if (currentLevel == LevelType.Minijuego2)
        {
            DisableControls();

            if (mainCamera != null)
            {
                mainCamera.transform.parent = null;

                // Low sweeping angle from the ocean waves looking up at the boat
                Vector3 cinematicStartPos = new Vector3(140f, 1f, 320f);
                Quaternion cinematicStartRot = Quaternion.Euler(-5f, 25f, 0f);

                if (cameraParent != null)
                {
                    Vector3 cinematicTargetPos = cameraParent.TransformPoint(originalLocalCamPos);
                    Quaternion cinematicTargetRot = cameraParent.rotation * originalLocalCamRot;
                }
                else
                {
                    Vector3 cinematicTargetPos = new Vector3(170.80f, 2.0f, 363.20f);
                    Quaternion cinematicTargetRot = Quaternion.identity;
                }

                mainCamera.transform.position = cinematicStartPos;
                mainCamera.transform.rotation = cinematicStartRot;
            }
        }

        dialogosSystem.StartDialogue();
        dialogueFinishedStarting = true;
    }

    private void AddLine(string charName, string text, bool isRight)
    {
        DialogueLine line = new DialogueLine();
        line.characterName = charName;
        line.text = text;
        line.isRightSpeaker = isRight;
        dialogosSystem.dialogueLines.Add(line);
    }

    private void Update()
    {
        if (!dialogueFinishedStarting) return;

        // Detect when cinematic dialogues finish
        if (isPlayingStartCinematic && (dialogosSystem == null || !dialogosSystem.IsDialogueActive()))
        {
            isPlayingStartCinematic = false;

            // Restore camera to player/boat parent
            if (mainCamera != null && cameraParent != null)
            {
                mainCamera.transform.parent = cameraParent;
                mainCamera.transform.localPosition = originalLocalCamPos;
                mainCamera.transform.localRotation = originalLocalCamRot;
            }

            if (currentLevel == LevelType.Minijuego2)
            {
                EnableControls();
            }

            OnStartCinematicFinished();
        }
        else if (isPlayingEndCinematic && (dialogosSystem == null || !dialogosSystem.IsDialogueActive()))
        {
            isPlayingEndCinematic = false;
            OnEndCinematicFinished();
        }
    }

    private bool IsDialogueActive()
    {
        if (dialogosSystem != null)
        {
            return dialogosSystem.lineIndex < dialogosSystem.dialogueLines.Count;
        }
        return false;
    }

    private void OnStartCinematicFinished()
    {
        Debug.Log("Start Cinematic Finished. Gameplay starts!");

        if (currentLevel == LevelType.Minijuego2)
        {
            GameObject boatObj = GameObject.Find("Boat");
            if (boatObj != null && boatObj.GetComponent<FishingMinigame>() == null)
            {
                boatObj.AddComponent<FishingMinigame>();
            }

            if (mainCamera != null && mainCamera.GetComponent<BoatCameraFollow>() == null)
            {
                mainCamera.gameObject.AddComponent<BoatCameraFollow>();
            }
        }

    }

   

    public int GetFishCaughtCount()
    {
        return fishCaughtCount;
    }

  

    public void NotifyFishCaught()
    {
        fishCaughtCount++;
        Debug.Log("Fish caught progress: " + fishCaughtCount + "/" + fishRequiredToCatch);
        if (fishCaughtCount >= fishRequiredToCatch)
        {
            StartCoroutine(FinishMinigameWithCinematic());
        }
    }



    public void NotifyMinijuego1Complete()
    {
        StartCoroutine(FinishMinigameWithCinematic());
    }

    public void NotifyMinijuego4Complete()
    {
        StartCoroutine(FinishMinigameWithCinematic());
    }

    private IEnumerator FinishMinigameWithCinematic()
    {
        yield return new WaitForSeconds(0.5f);

        isPlayingEndCinematic = true;
        DisableControls();

        // Separate camera for end panoramic shot
        if (mainCamera != null)
        {
            // Destroy BoatCameraFollow so it doesn't fight the cinematic end-sweep positions!
            BoatCameraFollow bcf = mainCamera.GetComponent<BoatCameraFollow>();
            if (bcf != null)
            {
                Destroy(bcf);
            }

            mainCamera.transform.parent = null;

            if (endCinematicAnchor != null)
            {
                mainCamera.transform.position = endCinematicAnchor.position;
                mainCamera.transform.rotation = endCinematicAnchor.rotation;
            }
            else
            {
                // Fallbacks
                if (currentLevel == LevelType.Minijuego1)
                {
                    mainCamera.transform.position = new Vector3(20f, 15f, 100f);
                    mainCamera.transform.rotation = Quaternion.Euler(12f, 25f, 0f);
                }
                else if (currentLevel == LevelType.Minijuego2)
                {
                    mainCamera.transform.position = new Vector3(150f, 12f, 340f);
                    mainCamera.transform.rotation = Quaternion.Euler(10f, 40f, 0f);
                }
            }
        }

        // Failsafe: if dialogosSystem is null, try to load it
        if (dialogosSystem == null)
        {
            dialogosSystem = FindFirstObjectByType<Dialogos>();
            if (dialogosSystem == null)
            {
                GameObject canvasPrefab = Resources.Load<GameObject>("Canvas");
                if (canvasPrefab != null)
                {
                    GameObject canvasInst = Instantiate(canvasPrefab);
                    dialogosSystem = canvasInst.GetComponentInChildren<Dialogos>();
                }
            }
        }

        if (dialogosSystem != null)
        {
            dialogosSystem.dialogueLines.Clear();

            if (currentLevel == LevelType.Minijuego1)
            {
                AddLine("Carlos", "¡Excelente! He reparado las partes dañadas del barco.", false);
                AddLine("Carlos", "El barco se siente firme y listo para zarpar hacia las tranquilas aguas.", false);
                AddLine("Carlos", "Naveguemos un rato y busquemos bancos de peces.", false);
            }
            else if (currentLevel == LevelType.Minijuego2)
            {
                AddLine("Carlos", "¡Excelente pesca! Con estos peces tenemos más que suficiente para cenar rico y fresco.", false);
                AddLine("Carlos", "Regresemos al muelle para celebrar un hermoso día.", false);
            }

            dialogosSystem.StartDialogue();
        }
        else
        {
            Debug.LogWarning("Dialogue system missing during level complete. Loading next level immediately.");
            OnEndCinematicFinished();
        }
    }

    private void OnEndCinematicFinished()
    {
        EnableControls();

        // Restore camera before changing scene
        if (mainCamera != null && cameraParent != null)
        {
            mainCamera.transform.parent = cameraParent;
            mainCamera.transform.localPosition = originalLocalCamPos;
            mainCamera.transform.localRotation = originalLocalCamRot;
        }

        if (currentLevel == LevelType.Minijuego1)
        {
            SceneManager.LoadScene("Minijuego2");
        }
        else if (currentLevel == LevelType.Minijuego2)
        {
            SceneManager.LoadScene("Creditos");
        }
    }

    private void DisableControls()
    {
        if (playerMove != null) playerMove.enabled = false;
        if (playerLook != null) playerLook.enabled = false;
        if (boatControls != null) boatControls.enabled = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void EnableControls()
    {

        if (playerMove != null) playerMove.enabled = true;
        if (playerLook != null) playerLook.enabled = true;
        if (boatControls != null) boatControls.enabled = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
