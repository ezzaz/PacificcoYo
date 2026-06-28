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
        Minijuego3
    }

    [Header("Current Level Configuration")]
    public LevelType currentLevel;
    public float delayBeforeStart = 0.5f;

    [Header("Minijuego 2 Settings (Fishing)")]
    public int fishRequiredToCatch = 3;
    private int fishCaughtCount = 0;

    [Header("Minijuego 3 Settings (Fish Basket)")]
    public int fishRequiredInBasket = 3;
    private int fishInBasketCount = 0;
    public GameObject fishPrefab; // We'll spawn these automatically if not null

    private Dialogos dialogosSystem;
    private PlayerMovement playerMove;
    private PlayerLook playerLook;
    private WaterBoat boatControls;

    // Camera Cinematic State
    private Camera mainCamera;
    private Transform cameraParent;
    private Vector3 originalLocalCamPos;
    private Quaternion originalLocalCamRot;

    private bool dialogueFinishedStarting = false;
    private bool isPlayingStartCinematic = false;
    private bool isPlayingEndCinematic = false;

    // Cinematic interpolation values
    private Vector3 cinematicStartPos;
    private Quaternion cinematicStartRot;
    private Vector3 cinematicTargetPos;
    private Quaternion cinematicTargetRot;
    private float cinematicProgress = 0f;
    private float cinematicDuration = 10f; // total camera fly-in time

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
        DisableControls();

        dialogosSystem.dialogueLines.Clear();

        if (currentLevel == LevelType.Minijuego1)
        {
            AddLine("Carlos", "El barco está algo dañado... No podré salir al mar así.", false);
            AddLine("Carlos", "Debo buscar las dos cajas de repuesto cerca de la cabaña y colocarlas en las zonas del barco para repararlo.", false);
            AddLine("Carlos", "Presionaré [E] para agarrar las cajas y llevarlas hasta el barco.", false);

            // Configure camera sweep: Start high in the sky looking at the beautiful reshaped island
            if (mainCamera != null)
            {
                mainCamera.transform.parent = null; // detach
                
                // High panorama showing the cabin, mountain, and ocean
                cinematicStartPos = new Vector3(-80f, 90f, -120f);
                cinematicStartRot = Quaternion.Euler(22f, 40f, 0f);
                
                // Target player's exact starting eyes
                if (cameraParent != null)
                {
                    cinematicTargetPos = cameraParent.TransformPoint(originalLocalCamPos);
                    cinematicTargetRot = cameraParent.rotation * originalLocalCamRot;
                }
                else
                {
                    cinematicTargetPos = new Vector3(44f, 42.14f + 0.62f, -10f + 0.57f);
                    cinematicTargetRot = Quaternion.Euler(0, 150f, 0);
                }

                mainCamera.transform.position = cinematicStartPos;
                mainCamera.transform.rotation = cinematicStartRot;
                cinematicProgress = 0f;
                cinematicDuration = 12f; // grand 12 seconds fly-in
            }
        }
        else if (currentLevel == LevelType.Minijuego2)
        {
            AddLine("Carlos", "¡Qué hermoso está el mar hoy! El cielo se ve de un azul espectacular.", false);
            AddLine("Carlos", "Es el momento perfecto para buscar bancos de peces en el océano con el barco.", false);
            AddLine("Carlos", "Naveguemos usando [W/A/S/D], y cuando estemos en el mar, presionemos [F] para lanzar la caña de pescar.", false);
            AddLine("Carlos", "¡Atrapemos 3 peces para el almuerzo!", false);

            if (mainCamera != null)
            {
                mainCamera.transform.parent = null;
                
                // Low sweeping angle from the ocean waves looking up at the boat
                cinematicStartPos = new Vector3(140f, 1f, 320f);
                cinematicStartRot = Quaternion.Euler(-5f, 25f, 0f);
                
                if (cameraParent != null)
                {
                    cinematicTargetPos = cameraParent.TransformPoint(originalLocalCamPos);
                    cinematicTargetRot = cameraParent.rotation * originalLocalCamRot;
                }
                else
                {
                    cinematicTargetPos = new Vector3(170.80f, 2.0f, 363.20f);
                    cinematicTargetRot = Quaternion.identity;
                }

                mainCamera.transform.position = cinematicStartPos;
                mainCamera.transform.rotation = cinematicStartRot;
                cinematicProgress = 0f;
                cinematicDuration = 10f;
            }
        }
        else if (currentLevel == LevelType.Minijuego3)
        {
            AddLine("Carlos", "Llegamos de vuelta al muelle con la pesca fresca.", false);
            AddLine("Carlos", "Ahora debo colocar los 3 pescados dentro de la cesta del barco para guardarlos bien.", false);
            AddLine("Carlos", "Puedo tomarlos con [E] y dejarlos caer dentro de la cesta.", false);

            if (mainCamera != null)
            {
                mainCamera.transform.parent = null;
                
                // Sweeping shot showing dock and basket
                cinematicStartPos = new Vector3(235f, 5f, 175f);
                cinematicStartRot = Quaternion.Euler(15f, 45f, 0f);
                
                if (cameraParent != null)
                {
                    cinematicTargetPos = cameraParent.TransformPoint(originalLocalCamPos);
                    cinematicTargetRot = cameraParent.rotation * originalLocalCamRot;
                }
                else
                {
                    cinematicTargetPos = new Vector3(251.14f, 1.5f, 182.15f);
                    cinematicTargetRot = Quaternion.identity;
                }

                mainCamera.transform.position = cinematicStartPos;
                mainCamera.transform.rotation = cinematicStartRot;
                cinematicProgress = 0f;
                cinematicDuration = 8f;
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

        // Perform camera cinematic LERPing during start cinematic
        if (isPlayingStartCinematic && mainCamera != null)
        {
            // Update targets dynamically in case player moves during setup (just to be safe)
            if (cameraParent != null)
            {
                cinematicTargetPos = cameraParent.TransformPoint(originalLocalCamPos);
                cinematicTargetRot = cameraParent.rotation * originalLocalCamRot;
            }

            cinematicProgress += Time.deltaTime / cinematicDuration;
            float t = Mathf.Clamp01(cinematicProgress);
            
            // Smooth custom step for slow cinematic deceleration
            t = Mathf.SmoothStep(0f, 1f, t);

            mainCamera.transform.position = Vector3.Lerp(cinematicStartPos, cinematicTargetPos, t);
            mainCamera.transform.rotation = Quaternion.Slerp(cinematicStartRot, cinematicTargetRot, t);
        }

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

            EnableControls();
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

            // Attach the specialized follow/rotation camera script so it stays above water and is fully orbitable
            if (mainCamera != null && mainCamera.GetComponent<BoatCameraFollow>() == null)
            {
                mainCamera.gameObject.AddComponent<BoatCameraFollow>();
            }
        }
        else if (currentLevel == LevelType.Minijuego3)
        {
            SpawnFishOnDeck();
        }
    }

    private void SpawnFishOnDeck()
    {
        GameObject boatObj = GameObject.Find("Boat");
        if (boatObj == null) return;

        Vector3[] spawnOffsets = new Vector3[]
        {
            new Vector3(0.5f, 0.8f, -0.5f),
            new Vector3(-0.5f, 0.8f, 0f),
            new Vector3(0f, 0.8f, -1.2f)
        };

        for (int i = 0; i < spawnOffsets.Length; i++)
        {
            GameObject fishInstance;
            if (fishPrefab != null)
            {
                fishInstance = Instantiate(fishPrefab, boatObj.transform.TransformPoint(spawnOffsets[i]), boatObj.transform.rotation);
            }
            else
            {
                fishInstance = GameObject.CreatePrimitive(PrimitiveType.Cube);
                fishInstance.name = "Pescado_Fresco_" + (i + 1);
                fishInstance.transform.position = boatObj.transform.TransformPoint(spawnOffsets[i]);
                fishInstance.transform.rotation = boatObj.transform.rotation;
                fishInstance.transform.localScale = new Vector3(0.5f, 0.2f, 0.25f);
                fishInstance.GetComponent<Renderer>().sharedMaterial.color = new Color(0.4f, 0.7f, 1f);
            }

            fishInstance.tag = "Box";
            
            Rigidbody rb = fishInstance.GetComponent<Rigidbody>();
            if (rb == null) rb = fishInstance.AddComponent<Rigidbody>();
            rb.mass = 1.0f;
            rb.isKinematic = false;

            if (fishInstance.GetComponent<Collider>() == null)
            {
                fishInstance.AddComponent<BoxCollider>();
            }

            if (fishInstance.GetComponent<ObjectGrabbable>() == null)
            {
                fishInstance.AddComponent<ObjectGrabbable>();
            }
        }
    }

    public int GetFishCaughtCount()
    {
        return fishCaughtCount;
    }

    public int GetFishInBasketCount()
    {
        return fishInBasketCount;
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

    public void NotifyFishPlacedInBasket()
    {
        fishInBasketCount++;
        Debug.Log("Fish placed progress: " + fishInBasketCount + "/" + fishRequiredInBasket);
        if (fishInBasketCount >= fishRequiredInBasket)
        {
            StartCoroutine(FinishMinigameWithCinematic());
        }
    }

    public void NotifyMinijuego1Complete()
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
            else if (currentLevel == LevelType.Minijuego3)
            {
                mainCamera.transform.position = new Vector3(230f, 8f, 170f);
                mainCamera.transform.rotation = Quaternion.Euler(8f, 50f, 0f);
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
                AddLine("Carlos", "¡Excelente pesca! Con estos 3 pescados tenemos más que suficiente para cenar rico y fresco.", false);
                AddLine("Carlos", "Regresemos al muelle para guardarlos a salvo.", false);
            }
            else if (currentLevel == LevelType.Minijuego3)
            {
                AddLine("Carlos", "¡Listo! Todos los pescados están a salvo en la cesta.", false);
                AddLine("Carlos", "Ha sido un día maravilloso de paz, contemplando la inmensidad del océano.", false);
                AddLine("Carlos", "Es hora de descansar y disfrutar de una hermosa noche.", false);
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
            SceneManager.LoadScene("Minijuego3");
        }
        else if (currentLevel == LevelType.Minijuego3)
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